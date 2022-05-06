using micro_c_app.Views.InventoryAudit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryLandingPage : ContentPage
    {
        public List<LandingItem> Items { get; set; }

        public ICommand Clicked { get; }
        public InventoryLandingPage()
        {
            BindingContext = this;
            Items = new List<LandingItem>()
            {
                new LandingItem("Scan", "\uf2db"),
                new LandingItem("Zero Stock", "\uf2db"),
                new LandingItem("Unknown Location", "\uf2db"),
                new LandingItem("Consolidation", "\uf2db"),
                new LandingItem("Freshness", "\uf2db")
            };

            Clicked = new Command<string>(OnClicked);

            InitializeComponent();
        }

        private void OnClicked(string item)
        {
            switch (item)
            {
                case "Scan":
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
                        if (!allowed)
                        {
                            return;
                        }

                        await Navigation.PushAsync(new InventoryView());
                    });
                    break;
                case "Zero Stock":
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        await Navigation.PushAsync(new ZeroStockPage());
                    });
                    break;
                default:
                    Console.WriteLine(item);
                    break;
            }
        }
    }

    public class LandingItem
    {
        public string Name { get; }
        public string Icon { get; }

        public LandingItem(string name, string icon)
        {
            Name = name;
            Icon = icon;
        }
    }
}