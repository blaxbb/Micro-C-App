using micro_c_app.ViewModels;
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
        public RealtimeBarcodeInfo BarcodeInfo { get; set; }
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
                buttonStack.IsVisible = false;
            }
            else
            {
                buttonStack.IsVisible = true;
                if (priceInfo != null && priceInfo.Count > 0)
                {
                    int maxIndex = -1;
                    double maxValue = priceInfo[0].Size;
                    for(int i = 1; i < priceInfo.Count; i++)
                    {
                        if(priceInfo[i].Size > maxValue && priceInfo[i].Price > .4 && priceInfo[i].Price < 10000000)
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

        private void InfoButton_Clicked(object sender, EventArgs e)
        {
            Shell.Current.Dispatcher.BeginInvokeOnMainThread(async () =>
            {
                await Shell.Current.GoToAsync($"//SearchPage?search={BarcodeInfo.Item.SKU}");
            });
        }

        private void AddButton_Clicked(object sender, EventArgs e)
        {
            QuotePageViewModel.AddItem(BarcodeInfo.Item.CloneAndResetQuantity());
        }
    }
}