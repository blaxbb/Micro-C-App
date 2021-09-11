using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RealtimePriceView : ContentView
    {
        private RealtimeBarcodeInfo BarcodeInfo;
        public RealtimePriceView()
        {
            InitializeComponent();
            frame.Background = Brush.Gray;
            BindingContext = this;
        }

        public void Update(RealtimeBarcodeInfo info, List<RealtimePriceInfo> priceInfo)
        {
            BarcodeInfo = info;
            if(info == null)
            {
                return;
            }

            if(info.Item == null)
            {
                nameLabel.Text = $"Loading... {info.Text}";
                skuLabel.Text = "";
                stockLabel.Text = "";
                priceLabel.Text = "";
            }
            else
            {
                if (priceInfo != null && priceInfo.Count > 0)
                {
                    int maxIndex = -1;
                    double maxValue = priceInfo[0].Size;
                    for(int i = 1; i < priceInfo.Count; i++)
                    {
                        if(priceInfo[i].Size > maxValue && priceInfo[i].Price > .4 && priceInfo[i].Price < 1000000)
                        {
                            maxIndex = i;
                            maxValue = priceInfo[i].Size;
                        }
                    }
                    if (maxIndex > -1)
                    {
                        //scanned text probably doesn't have a decimal place
                        if (info.Item.Price == priceInfo[maxIndex].Price || info.Item.Price == (priceInfo[maxIndex].Price / 100))
                        {
                            frame.Background = Brush.Green;
                        }
                        else
                        {
                            frame.Background = Brush.Red;
                        }
                    }
                }

                priceLabel.Text = $"${info.Item.Price}";
                nameLabel.Text = info.Item.Name;
                skuLabel.Text = info.Item.SKU;
                if(info.Item.ClearanceItems.Count > 0)
                {
                    stockLabel.Text = $"{info.Item.Stock} ({info.Item.ClearanceItems.Count} CL) in stock";
                }
                else
                {
                    stockLabel.Text = $"{info.Item.Stock} in stock";
                }
            }
        }

        private void TapGestureRecognizer_Tapped(object sender, EventArgs e)
        {
            Device.InvokeOnMainThreadAsync(async () =>
            {
                if (BarcodeInfo != null && BarcodeInfo.Item != null)
                {
                    var result = await Shell.Current.DisplayActionSheet(BarcodeInfo.Item.Name, "Cancel", null, new string[] {
                        "Product Info",
                        "Webpage",
                        "Add to quote",
                        "Add to build"
                    });
                    if (result == "Webpage")
                    {
                        await Xamarin.Essentials.Browser.OpenAsync($"https://microcenter.com{BarcodeInfo.Item.URL}", Xamarin.Essentials.BrowserLaunchMode.SystemPreferred);
                    }
                }
            });
        }
    }
}