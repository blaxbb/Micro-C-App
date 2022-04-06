using micro_c_app.Models.Inventory;
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
        private InventoryLocation currentLocation;

        public InventoryLocation CurrentLocation { get => currentLocation; set => SetProperty(ref currentLocation, value); }

        Dictionary<string, List<string>> Scans = new Dictionary<string, List<string>>();
        List<string> FailedSearches = new List<string>();
        HttpClient client;

        public InventoryView()
        {
            BindingContext = this;
            client = new HttpClient();
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
                        if(string.IsNullOrWhiteSpace(textResponse))
                        {
                            continue;
                        }
                    
                        CurrentLocation = JsonConvert.DeserializeObject<InventoryLocation>(textResponse);
                        statusLabel.Text = $"LOC : {barcode.Value.ToString()}";
                        if (SettingsPage.Vibrate())
                        {
                            Xamarin.Essentials.Vibration.Vibrate();
                        }
                    }
                    catch(Exception ex)
                    {
                        continue;
                    }
                }
                else if(CurrentLocation != null)
                {
                    var text = SearchView.FilterBarcodeResult(barcode);
                    if (!string.IsNullOrWhiteSpace(text))
                    {
                        statusLabel.Text = $"Searching for {text}";
                        var item = await FindItem(text);
                        if(item == null)
                        {
                            statusLabel.Text = $"Failed to find item for {text}";
                            continue;
                        }

                        if (!Scans.ContainsKey(CurrentLocation.Identifier))
                        {
                            Scans[CurrentLocation.Identifier] = new List<string>();
                        }

                        if (Scans[CurrentLocation.Identifier].Contains(text))
                        {
                            statusLabel.Text = $"Already scanned {item.Name}";
                        }
                        else
                        {
                            Scans[CurrentLocation.Identifier].Add(item.SKU);
                            statusLabel.Text = $"Scanned {item.Name}";
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
                statusLabel.Text = "Failed";
                return default;
            }
        }

        bool IsLocationIdentifier(string text)
        {
            return Regex.IsMatch(text, "loc\\d");
        }

        private async void AddSubmitClicked(object sender, EventArgs e)
        {
            await Submit("add");
        }

        private async void ReplaceSubmitClicked(object sender, EventArgs e)
        {
            await Submit("replace");
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

            statusLabel.Text = error ? "Failed to submit" : "Success!";

            return !error;
        }

        async Task<bool> Submit(string location, List<string> skus, string method = "add")
        {
            try
            {
                var json = JsonConvert.SerializeObject(skus);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"http://192.168.1.160:64198/api/Entries/bulk/add/{location}", content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
            }
            catch(Exception e)
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