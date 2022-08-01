using micro_c_app;
using micro_c_app.Models;
using micro_c_app.ViewModels;
using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryView : KeyboardPage, INotifyPropertyChanged
    {

        public InventoryLocation CurrentLocation { get => currentLocation; set => SetProperty(ref currentLocation, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }

        public int TotalProducts => Scans == null ? 0 : Scans.Sum(kvp => kvp.Value.Count);
        public int TotalSections => Scans == null ? 0 : Scans.Keys.Count;

        Dictionary<string, List<string>> Scans = new Dictionary<string, List<string>>();
        List<string> FailedSearches = new List<string>();
        public ObservableCollection<InventorySearchingStatus> Searching { get; set; }
        HttpClient client;

        private InventoryLocation currentLocation;
        private string statusText;

        string? previousFailedSku;

        public const string SCAN_LOCATION_TEXT = "Scan a location tag.";
        public const string SCAN_PRODUCT_TEXT = "Scan a product.";
        public const string SCAN_SUBMIT_SUCCESS_TEXT = "Submitted inventory results.\rScan a location tag.";

        public const string LOCATION_TRACKER_BASEURL = "https://location.bbarrett.me";

        string ManualText = "";

        CancellationTokenSource cts = new CancellationTokenSource();
        Task ticker;

        public InventoryView()
        {
            StatusText = SCAN_LOCATION_TEXT;
            Searching = new ObservableCollection<InventorySearchingStatus>();

            BindingContext = this;
            client = new HttpClient();
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(GoogleVisionBarCodeScanner.BarcodeFormats.All);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);

            InitializeComponent();
        }


        public override void OnKeyUp(string text)
        {
            ManualText = ManualText + text;
        }

        public override void OnEnter()
        {
            HandleText(ManualText);
            ManualText = "";
            base.OnEnter();
        }

        protected override void OnDisappearing()
        {
            cts.Cancel();
        }

        protected override void OnAppearing()
        {
            camera.RequestedFPS = 15;
            Scans = RestoreState.Instance.InventoryScans ?? new Dictionary<string, List<string>>();
            currentLocation = null;
            StatusText = SCAN_LOCATION_TEXT;
            ScansUpdated();
            GoogleVisionBarCodeScanner.Methods.SetIsBarcodeScanning(true);
            camera.IsEnabled = true;

            cts = new CancellationTokenSource();
            var token = cts.Token;
            ticker = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    foreach (var status in Searching.ToList())
                    {
                        if (status.Failed || status.Success)
                        {
                            status.ElapsedTime += 1000;
                            if (status.ElapsedTime > 5000)
                            {
                                var _ = Device.InvokeOnMainThreadAsync(() =>
                                {
                                    Searching.Remove(status);
                                    OnPropertyChanged(nameof(Searching));
                                });
                            }
                        }
                    }
                    await Task.Delay(1000, token);
                }
            }, token);
        }

        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnBarcodeDetectedEventArg e)
        {
            List<GoogleVisionBarCodeScanner.BarcodeResult> barcodes = e.BarcodeResults;
            foreach (var barcode in barcodes)
            {
                HandleText(barcode.Value);
            }
            await Task.Delay(2000);
            GoogleVisionBarCodeScanner.Methods.SetIsBarcodeScanning(true);
        }

        private async Task HandleText(string text)
        {
            if (IsLocationIdentifier(text))
            {
                previousFailedSku = null;
                var location = text;
                if (location == CurrentLocation?.Identifier)
                {
                    return;
                }
                try
                {
                    var response = await client.GetAsync($"{LOCATION_TRACKER_BASEURL}/api/Locations/{location}");
                    if (!response.IsSuccessStatusCode)
                    {
                        return;
                    }

                    var textResponse = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(textResponse))
                    {
                        return;
                    }

                    CurrentLocation = JsonConvert.DeserializeObject<InventoryLocation>(textResponse);
                    StatusText = SCAN_PRODUCT_TEXT;
                    if (SettingsPage.Vibrate())
                    {
                        Xamarin.Essentials.Vibration.Vibrate();
                    }
                }
                catch (Exception ex)
                {
                    return;
                }
            }
            else if (IsClearanceIdentifier(text))
            {
                if (CurrentLocation == null)
                {
                    return;
                }

                var status = new InventorySearchingStatus()
                {
                    Text = text
                };
                status.Success = true;
                Searching.Add(status);

                if (!Scans.ContainsKey(CurrentLocation.Identifier))
                {
                    Scans[CurrentLocation.Identifier] = new List<string>();
                }

                if (Scans[CurrentLocation.Identifier].Contains(text))
                {
                    if (SettingsPage.Vibrate())
                    {
                        Xamarin.Essentials.Vibration.Vibrate();
                    }
                }
                else
                {
                    Scans[CurrentLocation.Identifier].Add(text);
                    ScansUpdated();
                    if (SettingsPage.Vibrate())
                    {
                        Xamarin.Essentials.Vibration.Vibrate();
                    }
                }
            }
            else if (CurrentLocation != null)
            {
                var filtered = SearchView.FilterBarcodeResult(text);
                if (!string.IsNullOrWhiteSpace(filtered))
                {
                    //StatusText = $"{SCAN_SEARCHING_TEXT} {text}";
                    var status = new InventorySearchingStatus()
                    {
                        Text = filtered
                    };
                    Searching.Add(status);

                    var item = await FindItem(filtered);

                    if (item == null)
                    {
                        status.Failed = true;
                        //StatusText = $"{SCAN_FAILED_TEXT} {filtered}";
                        if(Regex.IsMatch(filtered, "\\d{6}"))
                        {
                            previousFailedSku = filtered;
                        }
                        return;
                    }

                    status.Success = true;

                    previousFailedSku = null;

                    if (!Scans.ContainsKey(CurrentLocation.Identifier))
                    {
                        Scans[CurrentLocation.Identifier] = new List<string>();
                    }

                    if (Scans[CurrentLocation.Identifier].Contains(item.SKU))
                    {
                        //StatusText = $"Already scanned {item.SKU} - {item.Name}";
                        if (SettingsPage.Vibrate())
                        {
                            Xamarin.Essentials.Vibration.Vibrate();
                        }
                    }
                    else
                    {
                        Scans[CurrentLocation.Identifier].Add(item.SKU);
                        ScansUpdated();
                        //StatusText = $"{SCAN_SUCCESS_TEXT} {item.SKU} - {item.Name}";
                        if (SettingsPage.Vibrate())
                        {
                            Xamarin.Essentials.Vibration.Vibrate();
                        }
                    }
                }
            }
        }

        private async Task<Item?> FindItem(string text)
        {
            if (FailedSearches.Contains(text))
            {
                return default;
            }
            var cached = App.SearchCache.Get(text);
            if (cached != null)
            {
                return cached;
            }


            var storeId = SettingsPage.StoreID();
            var results = await Search.LoadEnhanced(text, storeId, "");
            if (results.Items.Count == 1)
            {
                var item = results.Items[0];

                //
                //Load all items from category so cache is hot
                //
                App.SearchCache.Add(item);
                var catResults = await Search.LoadEnhanced("", storeId, BuildComponent.CategoryFilterForType(item.ComponentType));
                foreach (var res in catResults.Items)
                {
                    App.SearchCache.Add(res);
                }

                return item;
            }
            else
            {
                FailedSearches.Add(text);
                return default;
            }
        }

        public static bool IsLocationIdentifier(string text)
        {
            return Regex.IsMatch(text, "^\\d{3}-.*-.*$");
        }

        bool IsClearanceIdentifier(string text)
        {
            return Regex.IsMatch(text, "^CL\\d{5,}$");
        }

        private void ScansUpdated()
        {
            OnPropertyChanged(nameof(TotalProducts));
            OnPropertyChanged(nameof(TotalSections));
            RestoreState.Instance.InventoryScans = Scans;
            RestoreState.Save();
        }

        public async void ManualAddClicked(object sender, EventArgs e)
        {
            var manual = await Shell.Current.DisplayPromptAsync("Manual Entry", "Enter a SKU or location ID to manually add it to the scan collection.", initialValue: previousFailedSku);
            previousFailedSku = null;
            if (manual != null)
            {
                if (IsLocationIdentifier(manual))
                {
                    var response = await client.GetAsync($"{LOCATION_TRACKER_BASEURL}/api/Locations/{manual}");
                    if (!response.IsSuccessStatusCode)
                    {
                        await Shell.Current.DisplayAlert("Error", "Manual location entry did not retrieve any location from server!", "Ok");
                        return;
                    }

                    var textResponse = await response.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(textResponse))
                    {
                        await Shell.Current.DisplayAlert("Error", "Manual location entry did not retrieve any location from server!", "Ok");
                        return;
                    }

                    CurrentLocation = JsonConvert.DeserializeObject<InventoryLocation>(textResponse);
                    StatusText = SCAN_PRODUCT_TEXT;
                }
                else if (IsClearanceIdentifier(manual))
                {
                    if (CurrentLocation == null)
                    {
                        return;
                    }

                    var status = new InventorySearchingStatus()
                    {
                        Text = manual
                    };
                    status.Success = true;
                    Searching.Add(status);

                    if (!Scans.ContainsKey(CurrentLocation.Identifier))
                    {
                        Scans[CurrentLocation.Identifier] = new List<string>();
                    }

                    if (!Scans[CurrentLocation.Identifier].Contains(manual))
                    {
                        Scans[CurrentLocation.Identifier].Add(manual);
                        ScansUpdated();
                    }

                    if (SettingsPage.Vibrate())
                    {
                        Xamarin.Essentials.Vibration.Vibrate();
                    }
                }
                else if(Regex.IsMatch(manual, "^\\d{6}$"))
                {
                    if(CurrentLocation == null || string.IsNullOrWhiteSpace(CurrentLocation.Identifier))
                    {
                        await Shell.Current.DisplayAlert("Error", "Location must be set before manually entering a SKU.", "Ok");
                        return;
                    }

                    var status = new InventorySearchingStatus()
                    {
                        Text = manual
                    };
                    status.Success = true;
                    Searching.Add(status);

                    if (!Scans.ContainsKey(CurrentLocation.Identifier))
                    {
                        Scans[CurrentLocation.Identifier] = new List<string>();
                    }

                    if (!Scans[CurrentLocation.Identifier].Contains(manual))
                    {
                        Scans[CurrentLocation.Identifier].Add(manual);
                        ScansUpdated();
                    }

                    if (SettingsPage.Vibrate())
                    {
                        Xamarin.Essentials.Vibration.Vibrate();
                    }
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Manual entry not input corrcetly, must be Location ID or 6 digit SKU!", "Ok");
                }
            }
        }

        public async void ReviewClicked(object sender, EventArgs e)
        {
            GoogleVisionBarCodeScanner.Methods.SetIsBarcodeScanning(false);
            camera.IsEnabled = false;

            var vm = new InventoryReviewViewModel()
            {
                Scans = Scans,
            };
            vm.ScansUpdated += (sender, scans) =>
            {
                Scans = scans;
                ScansUpdated();
            };
            var page = new InventoryReviewPage()
            {
                BindingContext = vm
            };

            await Shell.Current.Navigation.PushAsync(page);
        }

        public async void SubmitClicked(object sender, EventArgs e)
        {
            const string REPLACE_ACTION = "Replace Items";
            const string ADD_ACTION = "Add Items";
            const string REPLACE_METHOD = "replace";
            const string ADD_METHOD = "add";
            var result = await Shell.Current.DisplayActionSheet("Submit inventory", "Cancel", null, REPLACE_ACTION, ADD_ACTION);
            string? method = result switch
            {
                REPLACE_ACTION => REPLACE_METHOD,
                ADD_ACTION => ADD_METHOD,
                _ => default
            };

            if(result == REPLACE_ACTION)
            {
                var confirm = await Shell.Current.DisplayAlert("Confirm", "This will remove all items previously submitted to the sections scanned.  This is irreversible!", "Confirm", "Cancel");
                if(!confirm)
                {
                    return;
                }
            }

            if (!string.IsNullOrWhiteSpace(method))
            {
                var success = await Submit(method);
                if (success)
                {
                    StatusText = SCAN_SUBMIT_SUCCESS_TEXT;
                    ReviewClicked(sender, e);
                }
            }
        }

        async Task<bool> Submit(string method)
        {
            if (Scans == null || Scans.Count == 0 || !Scans.Values.Any(l => l.Count > 0))
            {
                return true;
            }

            bool error = false;
            foreach (var kvp in Scans)
            {
                var result = await Submit(kvp.Key, kvp.Value, method);
                if (!result)
                {
                    error = true;
                }
            }

            return !error;
        }

        async Task<bool> Submit(string location, List<string> skus, string method = "add")
        {
            try
            {
                var json = JsonConvert.SerializeObject(skus);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{LOCATION_TRACKER_BASEURL}/api/Entries/bulk/{method}/{location}", content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e, location, string.Join(", ", skus), method);
            }

            return false;
        }

        public static async Task<InventoryLocation?> GetLocation(HttpClient client, string location)
        {
            try
            {
                var response = await client.GetAsync($"{LOCATION_TRACKER_BASEURL}/api/Locations/{location}");
                if (!response.IsSuccessStatusCode)
                {
                    return default;
                }

                var textResponse = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(textResponse))
                {
                    return default;
                }

                return JsonConvert.DeserializeObject<InventoryLocation>(textResponse);
            }
            catch(Exception e)
            {
                AnalyticsService.TrackError(e);
            }

            return default;
        }

        public static async Task<List<InventoryEntry>?> GetLocationEntries(HttpClient client, string location)
        {
            try
            {
                var response = await client.GetAsync($"{LOCATION_TRACKER_BASEURL}/api/Entries/location/{location}");
                if (!response.IsSuccessStatusCode)
                {
                    return default;
                }

                var textResponse = await response.Content.ReadAsStringAsync();
                if (string.IsNullOrWhiteSpace(textResponse))
                {
                    return default;
                }

                return JsonConvert.DeserializeObject<List<InventoryEntry>>(textResponse);
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
            }

            return default;
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public class InventorySearchingStatus : INotifyPropertyChanged
    {
        private string? text;
        private bool failed;
        private bool success;
        private bool searching;

        public string? Text { get => text; set => SetProperty(ref text, value); }
        public bool Failed { get => failed; set { Searching = false; SetProperty(ref failed, value); } }
        public bool Success { get => success; set { Searching = false; SetProperty(ref success, value); } }
        public bool Searching { get => searching; set => SetProperty(ref searching, value); }
        public float ElapsedTime { get; set; }

        public InventorySearchingStatus()
        {
            Searching = true;
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "",
            Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}