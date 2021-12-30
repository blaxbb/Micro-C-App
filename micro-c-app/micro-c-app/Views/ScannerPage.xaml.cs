using GoogleVisionBarCodeScanner;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public bool SerialMode => CurrentScanMode == ScanMode.Serial;

        private ScanMode CurrentScanMode { get => currentScanMode; set { currentScanMode = value; OnPropertyChanged(nameof(SerialMode)); OnPropertyChanged(nameof(CurrentScanMode)); } }

        public Command<string> RemoveSerialCommand { get; }

        enum ScanMode
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

            //scanner.Options = new MobileBarcodeScanningOptions()
            //{
            //    AutoRotate = false,
            //    TryHarder = true,
            //    PossibleFormats = new List<BarcodeFormat>() {
            //            BarcodeFormat.CODE_128,
            //            BarcodeFormat.UPC_A
            //        },
            //    UseNativeScanning = true,
            //    DelayBetweenContinuousScans = 1000,
            //};

            //scanner.OnScanResult += (result) =>
            //{
            //    OnScanResult?.Invoke(result);
            //};
            //scanner.IsScanning = true;
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
                        if(result != null && !string.IsNullOrWhiteSpace(result.Value))
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

        private bool TryAddSerial(string serial)
        {
            if (string.IsNullOrWhiteSpace(serial))
            {
                return false;
            }

            if(LastItem?.Item == null || LastItem.Serials.Count >= LastItem.Item.Quantity)
            {
                return false;
            }

            LastItem.Serials.Add(serial);
            LastItem.Serials = LastItem.Serials.ToList();
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
            if(LastItem != null && LastItem.Item != null)
            {
                LastItem.Item.Quantity++;
            }
        }

        private void SerialClicked(object sender, EventArgs e)
        {
            if(CurrentScanMode == ScanMode.Serial)
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
            if(index >= 0)
            {
                LastItem.Serials.RemoveAt(index);
                LastItem.Serials = LastItem.Serials.ToList();
            }
        }
    }
}