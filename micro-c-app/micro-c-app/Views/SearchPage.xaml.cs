using micro_c_app.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.Views
{
    public partial class SearchPage : ContentPage
    {
        HttpClient client;
        public SearchPage()
        {
            client = new HttpClient();
            InitializeComponent();
            StorePicker.SelectedIndexChanged += StorePicker_SelectedIndexChanged;
        }

        private void StorePicker_SelectedIndexChanged(object sender, EventArgs e)
        {

            var storeId = SearchViewModel.Stores[(string)StorePicker.SelectedItem];
            Preferences.Set(SearchViewModel.PREF_SELECTED_STORE, storeId);
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
                            await Navigation.PopAsync();
                            SearchField.Text = FilterBarcodeResult(result);
                            OnSearchClicked(this, new EventArgs());
                        });
                };
                await Navigation.PushAsync(scanPage);
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
            var storeId = Preferences.Get(SearchViewModel.PREF_SELECTED_STORE, "141");
            var response = client.GetAsync($"https://www.microcenter.com/search/search_results.aspx?Ntt={searchValue}&storeid={storeId}&Ntk=all").Result;
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var body = response.Content.ReadAsStringAsync().Result;

                var matches = Regex.Matches(body, "href=\"/quickView/(\\d{6}/.*?)\"");

                Device.BeginInvokeOnMainThread(async () =>
                {
                    if (matches.Count == 0)
                    {
                        await DisplayAlert("Scanned Barcode", "Match failed", "OK");
                    }
                    else
                    {
                        if (matches.Count == 1)
                        {
                            var item = await Models.Item.FromId(matches[0].Groups[1].Value);
                            var detailsPage = new ItemDetailsViewModel() { Item = item };
                            await Navigation.PushAsync(new ItemDetails() { BindingContext = detailsPage });
                                //await DisplayAlert("Scanned Barcode", matches[0].Groups[1].Value, "OK");
                            }
                        else
                        {
                            await DisplayAlert("Scanned Barcode", matches.Count.ToString(), "OK");
                        }
                    }
                });
            }
            else
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await DisplayAlert("Scanned Barcode", response.StatusCode.ToString(), "OK");
                });
            }
        }
    }
}