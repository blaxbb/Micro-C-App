using micro_c_app.ViewModels;
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
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryQuickScan : ContentPage, INotifyPropertyChanged
    {
        private Item item;
        private string statusText;

        public Item Item { get => item; set => SetProperty(ref item, value); }
        public string StatusText { get => statusText; set => SetProperty(ref statusText, value); }

        HttpClient client;

        public event EventHandler<EventArgs> OnSuccess;

        public InventoryQuickScan()
        {
            BindingContext = this;
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(GoogleVisionBarCodeScanner.BarcodeFormats.QRCode);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            client = new HttpClient();
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            camera.RequestedFPS = 30;
            GoogleVisionBarCodeScanner.Methods.SetIsBarcodeScanning(true);
            camera.IsEnabled = true;
        }

        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnBarcodeDetectedEventArg e)
        {
            List<GoogleVisionBarCodeScanner.BarcodeResult> barcodes = e.BarcodeResults;
            foreach (var barcode in barcodes)
            {
                if (InventoryView.IsLocationIdentifier(barcode.Value))
                {
                    var location = barcode.Value;

                    try
                    {
                        await DoSubmitLocation(location);
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }

            await Task.Delay(2000);
            GoogleVisionBarCodeScanner.Methods.SetIsBarcodeScanning(true);
        }

        public async void ManualAddClicked(object sender, EventArgs e)
        {
            var manual = await Shell.Current.DisplayPromptAsync("Manual Entry", "Enter a SKU or location ID to manually add it to the scan collection.");
            if (manual != null && InventoryView.IsLocationIdentifier(manual))
            {
                try
                {
                    await DoSubmitLocation(manual);
                }
                catch (Exception ex)
                {
                    return;
                }
            }
        }

        async Task DoSubmitLocation(string location)
        {
            var response = await client.GetAsync($"{InventoryView.LOCATION_TRACKER_BASEURL}/api/Locations/{location}");
            if (!response.IsSuccessStatusCode)
            {
                StatusText = "Location server connection failure";
                return;
            }

            var textResponse = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(textResponse))
            {
                StatusText = "Invalid location data received from server.";
                return;
            }

            var loc = JsonConvert.DeserializeObject<InventoryLocation>(textResponse);
            if (loc != null)
            {
                await InventoryReviewViewModel.DoSubmit(loc.Identifier, new List<string>() { Item.SKU });

                if (SettingsPage.Vibrate())
                {
                    Xamarin.Essentials.Vibration.Vibrate();
                }

                OnSuccess?.Invoke(this, new EventArgs());

                await Shell.Current.Navigation.PopAsync();
            }
            else
            {
                StatusText = "Invalid location data received from server.";
            }

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