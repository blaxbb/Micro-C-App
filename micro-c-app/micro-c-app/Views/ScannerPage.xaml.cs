using GoogleVisionBarCodeScanner;
using MicroCLib.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ScannerPage : ContentPage
    {
        private bool isRunningTask;
        private ProgressInfo progress;
        private BuildComponent lastItem;

        public delegate void ScanResultDelegate(BarcodeResult result);
        public event ScanResultDelegate OnScanResult;

        public bool IsRunningTask { get => isRunningTask; set { isRunningTask = value; OnPropertyChanged(nameof(IsRunningTask)); } }
        public ProgressInfo Progress { get => progress; set { progress = value; OnPropertyChanged(nameof(Progress)); } }

        public BuildComponent LastItem { get => lastItem; set { lastItem = value; OnPropertyChanged(nameof(LastItem)); } }

        ScanMode currentScanMode = ScanMode.Item;
        private bool onlySerialMode;

        public bool SerialMode => CurrentScanMode == ScanMode.Serial;

        public ScanMode CurrentScanMode { get => currentScanMode; set { currentScanMode = value; OnPropertyChanged(nameof(SerialMode)); OnPropertyChanged(nameof(CurrentScanMode)); } }
        public bool OnlySerialMode { get => onlySerialMode; set { onlySerialMode = value; OnPropertyChanged(nameof(OnlySerialMode)); } }

        public Command<string> RemoveSerialCommand { get; }

        public enum ScanMode
        {
            Item,
            Serial
        }

        public ScannerPage()
        {
            InitializeComponent();

            MessagingCenter.Subscribe<SearchView>(this, "LastItemUpdated", (view) =>
            {
                //LastItem = view.LastItem;
                //Debug.WriteLine($"UPDATE: {LastItem?.Name} - {LastItem?.Quantity}");
            });

            Methods.SetIsBarcodeScanning(true);

            scanner2.OnBarcodeDetected += (sender, args) =>
            {
                switch (CurrentScanMode)
                {
                    case ScanMode.Item:
                        OnScanResult?.Invoke(args.BarcodeResults.FirstOrDefault());
                        break;
                    case ScanMode.Serial:
                        var result = args.BarcodeResults.FirstOrDefault();
                        if (result != null && !string.IsNullOrWhiteSpace(result.Value))
                        {
                            var success = TryAddSerial(result.Value);
                            if (success && SettingsPage.Vibrate())
                            {
                                Xamarin.Essentials.Vibration.Vibrate();
                            }
                        }
                        break;
                }
                Device.StartTimer(TimeSpan.FromSeconds(1), () => { Methods.SetIsBarcodeScanning(true); return false; });
            };
            IsRunningTask = false;
            PropertyChanged += ScannerPage_PropertyChanged;

            RemoveSerialCommand = new Command<string>((serial) => RemoveSerial(serial));
        }

        public static async Task ScanSerial(Action<string> callback)
        {
            var scanPage = new ScannerPage()
            {

            };

            // Navigate to our scanner page
            scanPage.OnScanResult += async (result) =>
            {
                if (SettingsPage.Vibrate())
                {
                    Vibration.Vibrate();
                }
                callback.Invoke(result.Value);
            };

            //var navPage = new NavigationPage(scanPage);
            //navPage.ToolbarItems.Add(new ToolbarItem()
            //{
            //    Text = "Cancel",
            //    Command = new Command(async () => { await navigation.PopModalAsync(); })
            //});
            await Shell.Current.Navigation.PushAsync(scanPage);
        }

        private bool TryAddSerial(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
            {
                return false;
            }

            if (LastItem?.Item == null || LastItem.Serials.Count >= LastItem.Item.Quantity)
            {
                return false;
            }

            LastItem.Serials.Add(serial);
            return true;
        }

        private void ScannerPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsRunningTask))
            {
                scanner2.IsEnabled = !IsRunningTask;
                //scanner.IsScanning = !IsRunningTask;
            }
        }

        private void AddAnotherClicked(object sender, EventArgs e)
        {
            if (LastItem != null && LastItem.Item != null)
            {
                LastItem.Item.Quantity++;
            }
        }

        private async void ManualSerialClicked(object sender, EventArgs e)
        {
            Methods.SetIsBarcodeScanning(false);
            var result = await DisplayPromptAsync("Serial Number", "Enter a serial number.");
            TryAddSerial(result);
            Methods.SetIsBarcodeScanning(true);
        }

        private void SerialClicked(object sender, EventArgs e)
        {
            if (CurrentScanMode == ScanMode.Serial)
            {
                CurrentScanMode = ScanMode.Item;
            }
            else
            {
                CurrentScanMode = ScanMode.Serial;
            }
        }

        private void RemoveSerial(string serial)
        {
            var index = LastItem.Serials.IndexOf(serial);
            if (index >= 0)
            {
                LastItem.Serials.RemoveAt(index);
            }
        }
    }
}