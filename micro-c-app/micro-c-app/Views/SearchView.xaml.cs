using micro_c_app.Models;
using micro_c_app.ViewModels;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchView : ContentView
    {
        HttpClient client;
        private bool busy;
        public static readonly BindableProperty ProductFoundProperty = BindableProperty.Create(nameof(ProductFound), typeof(ICommand), typeof(SearchView), null);

        public static readonly BindableProperty ErrorProperty = BindableProperty.Create(nameof(Error), typeof(ICommand), typeof(SearchView), null);
        public ICommand ProductFound { get { return (ICommand)GetValue(ProductFoundProperty); } set { SetValue(ProductFoundProperty, value); } }
        public ICommand Error { get { return (ICommand)GetValue(ErrorProperty); } set { SetValue(ErrorProperty, value); } }

        public static readonly BindableProperty CategoryFilterProperty = BindableProperty.Create(nameof(CategoryFilter), typeof(string), typeof(SearchView), "");
        public string CategoryFilter { get { return (string)GetValue(CategoryFilterProperty); } set { SetValue(CategoryFilterProperty, value); } }

        public static readonly BindableProperty AutoPopSearchPageProperty = BindableProperty.Create(nameof(AutoPopSearchPage), typeof(bool), typeof(SearchView), false);
        public bool AutoPopSearchPage { get { return (bool)GetValue(AutoPopSearchPageProperty); } set { SetValue(AutoPopSearchPageProperty, value); } }

        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(string), typeof(SearchView), null);
        public string Orientation { get { return (string)GetValue(OrientationProperty); } set { SetValue(OrientationProperty, value); } }

        public static readonly BindableProperty OrderByProperty = BindableProperty.Create(nameof(OrderBy), typeof(OrderByMode), typeof(SearchView), null);
        public OrderByMode OrderBy { get { return (OrderByMode)GetValue(OrderByProperty); } set { SetValue(OrderByProperty, value); } }
        public enum OrderByMode
        {
            match,
            rating,
            numreviews,
            pricelow,
            pricehigh
        }

        public bool Busy
        {
            get
            {
                return busy;
            }
            set
            {
                busy = value;
                BusyIndicator.IsRunning = Busy;
                BusyIndicator.IsVisible = Busy;
                ScanButton.IsEnabled = !Busy;
                SKUField.IsEnabled = !Busy;
                SearchField.IsEnabled = !Busy;
                SKUSubmitButton.IsEnabled = !Busy;
                SearchSubmitButton.IsEnabled = !Busy;
            }
        }

        public SearchView()
        {
            InitializeComponent();
            Busy = false;
            if (!DesignMode.IsDesignModeEnabled)
            {
                client = new HttpClient();
            }
            SKUField.ReturnCommand = new Command(async () => await OnSubmit(SKUField.Text));
            SearchField.ReturnCommand = new Command(async () => await OnSubmit(SearchField.Text));
            SKUSubmitButton.Command = new Command(async () => await OnSubmit(SKUField.Text));
            SearchSubmitButton.Command = new Command(async () => await OnSubmit(SearchField.Text));
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (string.IsNullOrWhiteSpace(Orientation))
            {
                FlipStack.Orientation = width > height ? StackOrientation.Horizontal : StackOrientation.Vertical;
            }
            else
            {
                FlipStack.Orientation = Orientation == "Horizontal" ? StackOrientation.Horizontal : StackOrientation.Vertical;
            }

        }

        private void OnScanClicked(object sender, EventArgs e)
        {
            var filter = CategoryFilter;
            Console.WriteLine(filter);
            Device.BeginInvokeOnMainThread(async () =>
            {
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat>() {
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.UPC_A
                    },
                    UseNativeScanning = true
                };
                var scanPage = new ZXingScannerPage(options)
                {
                    DefaultOverlayShowFlashButton = true
                };
                // Navigate to our scanner page
                scanPage.OnScanResult += (result) =>
                {
                    // Stop scanning
                    scanPage.IsScanning = false;

                    // Pop the page and show the result
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PopModalAsync();
                        SKUField.Text = FilterBarcodeResult(result);
                        await OnSubmit(SKUField.Text);
                    });
                };

                var navPage = new NavigationPage(scanPage);
                navPage.ToolbarItems.Add(new ToolbarItem()
                {
                    Text = "Cancel",
                    Command = new Command(async () => { await Navigation.PopModalAsync(); })
                });
                await Navigation.PushModalAsync(navPage);
            });
        }

        private string FilterBarcodeResult(Result result)
        {
            switch (result.BarcodeFormat)
            {
                case BarcodeFormat.CODE_128:
                    if(result.Text.Length >= 6)
                    {
                        return result.Text.Substring(0, 6);
                    }
                    return "";
                case BarcodeFormat.UPC_A:
                default:
                    return result.Text;
            }
        }

        public static string GetSearchUrl(string query, string storeId, string categoryFilter, OrderByMode orderBy, int resultsPerPage, int page)
        {
            return $"https://www.microcenter.com/search/search_results.aspx?Ntt={query}&storeid={storeId}&myStore=false&Ntk=all&N={categoryFilter}&sortby={orderBy}&rpp={resultsPerPage}&page={page}";
        }

        public async Task OnSubmit(string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue) && string.IsNullOrWhiteSpace(CategoryFilter))
            {
                return;
            }

            Busy = true;

            await Task.Run(async () =>
            {
                var storeId = SettingsPage.StoreID();
                var response = await client.GetAsync(GetSearchUrl(searchValue, storeId, CategoryFilter, OrderBy, SearchResultsPageViewModel.RESULTS_PER_PAGE, 1));
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var body = response.Content.ReadAsStringAsync().Result;

                    var matches = Regex.Matches(body, "href=\"/quickView/(\\d{6}/.*?)\"");

                    if (matches.Count == 0)
                    {
                        //await DisplayAlert("Scanned Barcode", "Match failed", "OK");
                        DoError($"Failed to find product with query {searchValue}");
                    }
                    else
                    {
                        if (matches.Count == 1)
                        {
                            var item = await Item.FromUrl($"/product/{matches[0].Groups[1].Value}", storeId);
                            DoProductFound(item);

                        }
                        else
                        {
                            var page = new SearchResultsPage()
                            {
                                BindingContext = new SearchResultsPageViewModel()
                                {
                                    SearchQuery = searchValue,
                                    StoreID = storeId,
                                    CategoryFilter = CategoryFilter,
                                    OrderBy = OrderBy,
                                }
                            };
                            page.AutoPop = AutoPopSearchPage;
                            page.ItemTapped += (sender, args) =>
                            {
                                Task.Run(async () =>
                                {
                                    if (args.CurrentSelection.FirstOrDefault() is Item shortItem)
                                    {
                                        var item = await Item.FromUrl(shortItem.URL, storeId);
                                        DoProductFound(item);
                                    }
                                });
                            };

                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                await Shell.Current.Navigation.PushAsync(page);
                            });

                            if (page.BindingContext is SearchResultsPageViewModel vm)
                            {
                                await vm.ParseBody(body);
                            }
                        }
                    }
                }
                else
                {
                    DoError($"webrequest returned error {response.StatusCode}");
                }
            });

            Busy = false;
        }

        private async void DoProductFound(Item item)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ProductFound?.Execute(item);
            });
        }

        private async void DoError(string message)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                Error?.Execute(message);
            });
        }
    }
}