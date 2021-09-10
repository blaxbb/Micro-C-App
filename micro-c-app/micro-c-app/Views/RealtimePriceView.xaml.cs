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
        public RealtimePriceView()
        {
            InitializeComponent();
            BindingContext = this;
        }

        public void Update(RealtimeBarcodeInfo info, List<RealtimePriceInfo> priceInfo)
        {
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
                if(priceInfo != null && priceInfo.Count > 0)
                {
                    int maxIndex = 0;
                    double maxValue = priceInfo[0].Size;
                    for(int i = 1; i < priceInfo.Count; i++)
                    {
                        if(priceInfo[i].Size > maxValue)
                        {
                            maxIndex = i;
                            maxValue = priceInfo[i].Size;
                        }
                    }

                    if(info.Item.Price == priceInfo[maxIndex].Price)
                    {
                        frame.Background = Brush.Green;
                    }
                    else
                    {
                        frame.Background = Brush.Red;
                    }
                    priceLabel.Text = $"${info.Item.Price} - {priceInfo[maxIndex].Price}";
                }
                else
                {
                    priceLabel.Text = $"${info.Item.Price}";
                }

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
    }
}