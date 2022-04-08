using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryView : ContentPage, INotifyPropertyChanged
    {

        public InventoryLocation CurrentLocation { get => currentLocation; set => SetProperty(ref currentLocation, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }

        public int TotalProducts => Scans == null ? 0 : Scans.Sum(kvp => kvp.Value.Count);
        public int TotalSections => Scans == null ? 0 : Scans.Keys.Count;

        Dictionary<string, List<string>> Scans = new Dictionary<string, List<string>>();
        List<string> FailedSearches = new List<string>();
        HttpClient client;

        private InventoryLocation currentLocation;
        private string statusText;

        public const string SCAN_LOCATION_TEXT = "Scan a location tag.";
        public const string SCAN_PRODUCT_TEXT = "Scan a product tag.";
        public const string SCAN_SEARCHING_TEXT = "Searching for product...";
        public const string SCAN_CACHING_TEXT = "Caching similar products.";
        public const string SCAN_FAILED_TEXT = "Failed to find product for";
        public const string SCAN_SUCCESS_TEXT = "Scanned product";
        public const string SCAN_SUBMIT_SUCCESS_TEXT = "Submitted inventory results.";

        public InventoryView()
        {
            StatusText = SCAN_LOCATION_TEXT;
            BindingContext = this;
            client = new HttpClient();
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(GoogleVisionBarCodeScanner.BarcodeFormats.All);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            InitializeComponent();
        }

        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnBarcodeDetectedEventArg e)
        {
            List<GoogleVisionBarCodeScanner.BarcodeResult> barcodes = e.BarcodeResults;
            foreach (var barcode in barcodes)
            {
                if (IsLocationIdentifier(barcode.Value))
                {
                    var location = barcode.Value;
                    if (location == CurrentLocation?.Identifier)
                    {
                        continue;
                    }
                    try
                    {
                        var response = await client.GetAsync($"http://192.168.1.160:64198/api/Locations/{location}");
                        if (!response.IsSuccessStatusCode)
                        {
                            continue;
                        }

                        var textResponse = await response.Content.ReadAsStringAsync();
                        if (string.IsNullOrWhiteSpace(textResponse))
                        {
                            continue;
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
                        continue;
                    }
                }
                else if (CurrentLocation != null)
                {
                    var text = SearchView.FilterBarcodeResult(barcode);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        StatusText = $"{SCAN_SEARCHING_TEXT} {text}";
                        var item = await FindItem(text);
                        if (item == null)
                        {
                            StatusText = $"{SCAN_FAILED_TEXT} {text}";
                            continue;
                        }

                        if (!Scans.ContainsKey(CurrentLocation.Identifier))
                        {
                            Scans[CurrentLocation.Identifier] = new List<string>();
                        }

                        if (Scans[CurrentLocation.Identifier].Contains(item.SKU))
                        {
                            StatusText = $"Already scanned {item.Name}";
                            if (SettingsPage.Vibrate())
                            {
                                Xamarin.Essentials.Vibration.Vibrate();
                            }
                        }
                        else
                        {
                            Scans[CurrentLocation.Identifier].Add(item.SKU);
                            ScansUpdated();
                            StatusText = $"{SCAN_SUCCESS_TEXT} {item.Name}";
                            if (SettingsPage.Vibrate())
                            {
                                Xamarin.Essentials.Vibration.Vibrate();
                            }
                        }
                    }
                }
            }

            await Task.Delay(2000);
            Device.BeginInvokeOnMainThread(() => GoogleVisionBarCodeScanner.Methods.SetIsBarcodeScanning(true));
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

        bool IsLocationIdentifier(string text)
        {
            return Regex.IsMatch(text, "\\d{3}-.*-.*");
        }

        private void ScansUpdated()
        {
            OnPropertyChanged(nameof(TotalProducts));
            OnPropertyChanged(nameof(TotalSections));
        }

        public void ResetClicked(object sender, EventArgs e)
        {
            CurrentLocation = null;
            Scans.Clear();
            ScansUpdated();
            OnPropertyChanged(nameof(Scans));
        }

        public async void SubmitClicked(object sender, EventArgs e)
        {
            const string REPLACE_ACTION = "Replace Items";
            const string ADD_ACTION = "Add Items";
            var result = await Shell.Current.DisplayActionSheet("Submit inventory", "Cancel", REPLACE_ACTION, ADD_ACTION);
            string? method = result switch
            {
                REPLACE_ACTION => "replace",
                ADD_ACTION => "add",
                _ => default
            };
            if (!string.IsNullOrWhiteSpace(method))
            {
                var success = await Submit(method);
                if (success)
                {
                    StatusText = SCAN_SUBMIT_SUCCESS_TEXT;
                    ResetClicked(sender, e);
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
                var response = await client.PostAsync($"http://192.168.1.160:64198/api/Entries/bulk/{method}/{location}", content);
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
}