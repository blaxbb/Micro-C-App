using micro_c_app.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
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

        public static readonly BindableProperty ProductFoundProperty = BindableProperty.Create(nameof(ProductFound), typeof(ICommand), typeof(SearchView), null);
        public ICommand ProductFound { get { return (ICommand)GetValue(ProductFoundProperty); } set { SetValue(ProductFoundProperty, value); } }

        public static readonly BindableProperty ErrorProperty = BindableProperty.Create(nameof(Error), typeof(ICommand), typeof(SearchView), null);
        public ICommand Error { get { return (ICommand)GetValue(ErrorProperty); } set { SetValue(ErrorProperty, value); } }


        public SearchView()
        {
            client = new HttpClient();
            InitializeComponent();
            SearchField.ReturnCommand = new Command(() =>
            {
                OnSearchClicked(this, new EventArgs());
            });
        }

        private void OnScanClicked(object sender, EventArgs e)
        {
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
                        SearchField.Text = FilterBarcodeResult(result);
                        OnSearchClicked(this, new EventArgs());
                    });
                };
                await Navigation.PushModalAsync(scanPage);
            });
        }

        private string FilterBarcodeResult(Result result)
        {
            switch (result.BarcodeFormat)
            {
                case BarcodeFormat.CODE_128:
                    return result.Text.Substring(0, 6);
                case BarcodeFormat.UPC_A:
                default:
                    return result.Text;
            }
        }

        private void OnSearchClicked(object sender, EventArgs e)
        {
            var searchValue = SearchField.Text;
            if (string.IsNullOrWhiteSpace(searchValue))
            {
                return;
            }
            var storeId = SettingsPage.StoreID();
            var response = client.GetAsync($"https://www.microcenter.com/search/search_results.aspx?Ntt={searchValue}&storeid={storeId}&Ntk=all").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var body = response.Content.ReadAsStringAsync().Result;

                var matches = Regex.Matches(body, "href=\"/quickView/(\\d{6}/.*?)\"");

                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (matches.Count == 0)
                    {
                        //await DisplayAlert("Scanned Barcode", "Match failed", "OK");
                        Error?.Execute($"Failed to find product with query {searchValue}");
                    }
                    else
                    {
                        if (matches.Count == 1)
                        {
                            var item = await Models.Item.FromId(matches[0].Groups[1].Value);
                            ProductFound?.Execute(item);
                        }
                        else
                        {
                            Error?.Execute($"Error found {matches.Count} results");
                        }
                    }
                });
            }
            else
            {
                Error?.Execute($"webrequest returned error {response.StatusCode.ToString()}");
            }
        }
    }
}