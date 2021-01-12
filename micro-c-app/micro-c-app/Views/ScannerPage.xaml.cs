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

        public delegate void ScanResultDelegate(BarcodeResult result);
        public event ScanResultDelegate OnScanResult;

        public bool IsRunningTask { get => isRunningTask; set { isRunningTask = value; OnPropertyChanged(nameof(IsRunningTask)); } }
        public ProgressInfo Progress { get => progress; set { progress = value; OnPropertyChanged(nameof(Progress)); } }

        public ScannerPage()
        {
            InitializeComponent();

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

            scanner2.OnDetected += (sender, args) =>
            {
                OnScanResult?.Invoke(args.BarcodeResults.FirstOrDefault());
                Device.StartTimer(TimeSpan.FromSeconds(1), () => { Methods.SetIsScanning(true); return false; });
            };
            IsRunningTask = false;
            PropertyChanged += ScannerPage_PropertyChanged;
        }

        private void ScannerPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IsRunningTask))
            {
                scanner2.IsEnabled = !IsRunningTask;
                //scanner.IsScanning = !IsRunningTask;
            }
        }
    }
}