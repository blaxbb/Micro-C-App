using GoogleVisionBarCodeScanner;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
            GoogleVisionBarCodeScanner.Methods.SetSupportBarcodeFormat(BarcodeFormats.All);
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

            await Device.InvokeOnMainThreadAsync(() =>
            {
                List<string> toRemove = new List<string>();

                foreach (var kvp in BarcodeInfo)
                {
                    var info = kvp.Value;

                    if (DateTime.Now - info.LastScanned > TimeSpan.FromMilliseconds(500))
                    {
                        toRemove.Add(kvp.Key);
                        continue;
                    }

                    SetPosition(info);

                    if (info.Item != null)
                    {
                        SetGUIValues(info);
                    }
                    else
                    {
                        info.NameLabel.Text = $"Loading... {info.Text}";
                    }
                }

                foreach (var remove in toRemove)
                {
                    var info = BarcodeInfo[remove];
                    grid.Children.Remove(info.OuterFrame);
                    BarcodeInfo.Remove(remove);
                }
            });

            await Task.Delay(1000 / 30);
        }

        private void SetPosition(RealtimeBarcodeInfo info)
        {
            var maxX = grid.Width - 225;
            var maxY = grid.Height - 210;

            double clamp(double value, double max) => value > max ? max : value;

            info.CurrentPosition = new Point(
                                                clamp(Lerp(info.CurrentPosition.X, info.TargetPosition.X, .3d), maxX),
                                                clamp(Lerp(info.CurrentPosition.Y, info.TargetPosition.Y, .3d), maxY)
                                            );

            info.Grid.Margin = new Thickness(info.CurrentPosition.X, info.CurrentPosition.Y, 0, 0);
        }

        private void SetGUIValues(RealtimeBarcodeInfo info)
        {
            const int MAX_NAME_LENGTH = 36;
            info.NameLabel.Text = info.Item.Name.Length > MAX_NAME_LENGTH ? info.Item.Name.Substring(0, MAX_NAME_LENGTH) : info.Item.Name;
            info.SkuLabel.Text = info.Item.SKU;
            info.PriceLabel.Text = info.Item.Price.ToString("$0.00");
            info.StockLabel.Text = $"{info.Item.Stock} in stock";
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
            GoogleVisionBarCodeScanner.Methods.ToggleFlashlight();
        }

        private async void CameraView_OnDetected(object sender, GoogleVisionBarCodeScanner.OnDetectedEventArg e)
        {
            Device.BeginInvokeOnMainThread(() => GoogleVisionBarCodeScanner.Methods.SetIsScanning(true));

            List<GoogleVisionBarCodeScanner.BarcodeResult> barcodes = e.BarcodeResults;

            foreach (var barcode in barcodes)
            {
                var filtered = SearchView.FilterBarcodeResult(barcode);
                if (string.IsNullOrWhiteSpace(filtered))
                {
                    continue;
                }

                barcode.DisplayValue = filtered;

                RealtimeBarcodeInfo info;
                Point target = new Point(barcode.Points[0].x * grid.Width, barcode.Points[0].y * grid.Height);

                if (!BarcodeInfo.ContainsKey(barcode.DisplayValue))
                {
                    //found new barcode

                    info = new RealtimeBarcodeInfo()
                    {
                        Text = barcode.DisplayValue,
                        CurrentPosition = target,
                        LastScanned = DateTime.Now
                    };
                    BarcodeInfo[barcode.DisplayValue] = info;

                    await Device.InvokeOnMainThreadAsync(() =>
                    {
                        CreateGUIForBarcodeInfo(info);
                    });
                }
                else
                {
                    info = BarcodeInfo[barcode.DisplayValue];
                }

                info.LastScanned = DateTime.Now;
                info.TargetPosition = target;
            }
        }

        private void CreateGUIForBarcodeInfo(RealtimeBarcodeInfo info)
        {
            /*
             * Create GUI for new barcode
             * 
             * Frame - transparent, fully covers camera view
             *  -Grid  - margin is set on this to adjust position
             *   -Label - Content
             */
            Application.Current.Resources.TryGetValue("PrimaryColor", out var color);
            var primaryColor = (Color)color;
            Application.Current.Resources.TryGetValue("PrimaryTextColor", out color);
            var textColor = (Color)color;

            var frame = new Frame()
            {
                Padding = 0,
                BackgroundColor = Color.Transparent,
                InputTransparent = true
            };

            var barcodeGrid = new Grid()
            {
                BackgroundColor = primaryColor,
                Opacity = .85d,
                Margin = new Thickness(0, 0, 0, 0),
                WidthRequest = ITEM_WIDTH,
                MinimumWidthRequest = ITEM_WIDTH,
                HeightRequest = ITEM_HEIGHT,
                MinimumHeightRequest = ITEM_HEIGHT,
                HorizontalOptions = LayoutOptions.Start,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(15),
                InputTransparent = false
            };

            barcodeGrid.GestureRecognizers.Add(new TapGestureRecognizer()
            {
                Command = new Command(async () =>
                {
                    if (info.Item != null)
                    {
                        await Xamarin.Essentials.Browser.OpenAsync($"https://microcenter.com{info.Item.URL}", Xamarin.Essentials.BrowserLaunchMode.SystemPreferred);
                    }
                })
            });

            barcodeGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

            barcodeGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(2, GridUnitType.Star) });
            barcodeGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });
            barcodeGrid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Star });

            frame.Content = barcodeGrid;

            Label buildLabel() => new Label()
            {
                TextColor = textColor,
                FontSize = 16,
                VerticalOptions = LayoutOptions.Center,
                VerticalTextAlignment = TextAlignment.Center,
                InputTransparent = true,
            };

            info.NameLabel = buildLabel();
            info.NameLabel.MaxLines = 2;
            barcodeGrid.Children.Add(info.NameLabel);
            Grid.SetRow(info.NameLabel, 0);
            Grid.SetColumn(info.NameLabel, 0);

            var stack = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                InputTransparent = true
            };
            barcodeGrid.Children.Add(stack);
            Grid.SetRow(stack, 1);
            Grid.SetColumn(stack, 0);

            info.SkuLabel = buildLabel();
            info.SkuLabel.HorizontalOptions = LayoutOptions.StartAndExpand;
            stack.Children.Add(info.SkuLabel);

            info.PriceLabel = buildLabel();
            stack.Children.Add(info.PriceLabel);

            info.StockLabel = buildLabel();
            barcodeGrid.Children.Add(info.StockLabel);
            Grid.SetRow(info.StockLabel, 2);
            Grid.SetColumn(info.StockLabel, 0);

            info.OuterFrame = frame;
            info.Grid = barcodeGrid;

            grid.Children.Add(frame);
            Grid.SetRow(frame, 1);

        }

        private double Lerp(double current, double target, double dt)
        {
            return (1 - dt) * current + (dt * target);
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
            [CallerMemberName] string propertyName = "")
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }

    public class RealtimeBarcodeInfo
    {
        public string Text { get; set; }
        public Point CurrentPosition { get; set; }
        public Point TargetPosition { get; set; }
        public DateTime LastScanned { get; set; }

        public Frame OuterFrame { get; set; }
        public Grid Grid { get; set; }
        public Label NameLabel { get; set; }
        public Label SkuLabel { get; set; }
        public Label PriceLabel { get; set; }
        public Label StockLabel { get; set; }

        public Item Item { get; set; }
    }
}