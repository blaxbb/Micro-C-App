using BarcodeScanner.Mobile;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration.iOSSpecific;
using Xamarin.Forms.Xaml;
using Application = Xamarin.Forms.Application;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealtimeScan : ContentPage
    {
        Dictionary<string, RealtimeBarcodeInfo> BarcodeInfo = new Dictionary<string, RealtimeBarcodeInfo>();
        Dictionary<string, RealtimePriceInfo> PriceInfo = new Dictionary<string, RealtimePriceInfo>();
        List<string> FailedSearches = new List<string>();
        bool SearchActive { get; set; }
        private const string FAILED_TEXT = "Failed";
        private const int ITEM_WIDTH = 200;
        private const int ITEM_HEIGHT = 130;

        public RealtimeScan()
        {
            InitializeComponent();
            AnalyticsService.Track("Start AR");
            FailedSearches.Add(FAILED_TEXT);
            BindingContext = this;
            BarcodeScanner.Mobile.Methods.SetSupportBarcodeFormat(BarcodeFormats.All);
            On<Xamarin.Forms.PlatformConfiguration.iOS>().SetUseSafeArea(true);
            Task.Run(ScheduleTick);
        }

        private async Task ScheduleTick()
        {
            await Task.Run(DoTick)
                .ContinueWith(async (task) => await ScheduleTick());
        }
        private async Task DoTick()
        {
            foreach (var kvp in BarcodeInfo)
            {
                var info = kvp.Value;
                if (info.Item == null)
                {
                    if(info.Text == "Failed")
                    {
                        continue;
                    }
                    var cached = App.SearchCache.Get(info.Text);
                    if (cached != null)
                    {
                        info.Item = cached;
                    }
                    else if (!SearchActive)
                    {
                        SearchActive = true;
                        Task.Run(() => FindItem(info));
                    }
                }
            }
            List<string> toRemove = new List<string>();
            foreach (var kvp in PriceInfo)
            {
                var info = kvp.Value;
                if (DateTime.Now - info.LastScanned > TimeSpan.FromMilliseconds(1000))
                {
                    toRemove.Add(kvp.Key);
                }
            }
            toRemove.ForEach(k => PriceInfo.Remove(k));

            await Device.InvokeOnMainThreadAsync(() =>
            {
                List<string> toRemove = new List<string>();

                foreach (var kvp in BarcodeInfo)
                {
                    var info = kvp.Value;

                    if (DateTime.Now - info.LastScanned > TimeSpan.FromMilliseconds(1000))
                    {
                        toRemove.Add(kvp.Key);
                        continue;
                    }

                    if (info.View != null)
                    {
                        info.View.Update(info, PriceInfo.Values.ToList());
                    }
                }

                foreach (var remove in toRemove)
                {
                    var info = BarcodeInfo[remove];
                    priceInfo.Children.Remove(info.View);
                    BarcodeInfo.Remove(remove);
                }
            });

            await Task.Delay(1000/10);
        }

        private async Task FindItem(RealtimeBarcodeInfo info)
        {
            if (FailedSearches.Contains(info.Text))
            {
                info.Text = "Failed";
                SearchActive = false;
                return;
            }

            var storeId = SettingsPage.StoreID();
            var results = await Search.LoadEnhanced(info.Text, storeId, "");
            if(results.Items.Count == 1)
            {
                info.Item = results.Items[0];

                //
                //Load all items from category so cache is hot
                //
                var catResults = await Search.LoadEnhanced("", storeId, BuildComponent.CategoryFilterForType(info.Item.ComponentType));
                foreach(var res in catResults.Items)
                {
                    App.SearchCache.Add(res);
                }
            }
            else
            {
                FailedSearches.Add(info.Text);
                info.Text = "Failed";
            }

            SearchActive = false;
        }


        private async void CancelButton_Clicked(object sender, EventArgs e)
        {
            await Shell.Current.Navigation.PopAsync();
        }

        private void FlashlightButton_Clicked(object sender, EventArgs e)
        {
            //BarcodeScanner.Mobile.Methods.ToggleFlashlight();
        }

        //private async void CameraView_OnDetected(object sender, BarcodeScanner.Mobile.Methods.OnBarcodeDetectedEventArg e)
        //{
        //    Device.BeginInvokeOnMainThread(() => BarcodeScanner.Mobile.Methods.SetIsBarcodeScanning(true));

        //    List<BarcodeScanner.Mobile.BarcodeResult> barcodes = e.BarcodeResults;

        //    foreach (var barcode in barcodes)
        //    {
        //        var filtered = SearchView.FilterBarcodeResult(barcode);
        //        if (string.IsNullOrWhiteSpace(filtered))
        //        {
        //            continue;
        //        }

        //        if(BarcodeInfo.Values.Any(b => b.Item != null && b.Item.SKU == filtered && b.Text != filtered))
        //        {
        //            //We already have this SKU barcode as a UPC barcode, so skip it
        //            continue;
        //        }

        //        barcode.Value = filtered;

        //        RealtimeBarcodeInfo info;
        //        Point target = new Point(barcode.Points[0].x * grid.Width, barcode.Points[0].y * grid.Height);

        //        if (!BarcodeInfo.ContainsKey(barcode.Value))
        //        {
        //            //found new barcode

        //            info = new RealtimeBarcodeInfo()
        //            {
        //                Text = barcode.Value,
        //                LastScanned = DateTime.Now
        //            };
        //            BarcodeInfo[barcode.Value] = info;

        //            await Device.InvokeOnMainThreadAsync(() =>
        //            {
        //                info.View = new RealtimePriceView();
        //                priceInfo.Children.Add(info.View);
        //            });
        //        }
        //        else
        //        {
        //            info = BarcodeInfo[barcode.Value];
        //        }

        //        info.LastScanned = DateTime.Now;
        //    }
        //}

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        //private void CameraView_OnTextDetected(object sender, OnTextDetectedEventArg e)
        //{
        //    foreach(var text in e.TextResults)
        //    {
        //        if (PriceInfo.ContainsKey(text.Value))
        //        {
        //            var item = PriceInfo[text.Value];
        //            item.LastScanned = DateTime.Now;
        //            item.Size = RealtimePriceInfo.GetSize(text.Points);
        //            //Debug.WriteLine($"{item.Price} - {item.Size}");
        //        }
        //        else
        //        {
        //            var match = Regex.Match(text.Value, "^\\$*(\\d+\\.*\\d*)$");
        //            if (match.Success)
        //            {
        //                var price = float.Parse(match.Groups[1].Value);

        //                var info = new RealtimePriceInfo()
        //                {
        //                    Text = text.Value,
        //                    Price = price,
        //                    LastScanned = DateTime.Now,
        //                    Size = RealtimePriceInfo.GetSize(text.Points)
        //                };
        //                PriceInfo.Add(text.Value, info);
        //            }
        //        }
        //    }
        //    //Device.BeginInvokeOnMainThread(async () => {
        //    //    BarcodeScanner.Mobile.Methods.SetIsTextScanning(true);
        //    //});
        //}
    }

    public class RealtimeBarcodeInfo
    {
        public string Text { get; set; }
        public DateTime LastScanned { get; set; }

        public RealtimePriceView View { get; set; }

        public Item Item { get; set; }
    }
    public class RealtimePriceInfo
    {
        public string Text { get; set; }
        public float Price { get; set; }
        public double Size { get; set; }
        public DateTime LastScanned { get; set; }
        public static double GetSize(List<(double x, double y)> points)
        {
            var minX = points.Min(p => p.x);
            var maxX = points.Max(p => p.x);
            var minY = points.Min(p => p.y);
            var maxY = points.Max(p => p.y);
            return (maxX - minX) * (maxY - minY);
        }
    }
}
