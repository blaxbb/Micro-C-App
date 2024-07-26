using micro_c_app.Views;
using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Xamarin.Forms;
using System.Windows.Input;

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class UnknownLocationPageViewModel : BaseInventoryViewModel
    {
        private ObservableCollection<UnknownLocationInfo> items;
        public ObservableCollection<UnknownLocationInfo> Items { get => items; set => SetProperty(ref items, value); }

        public ICommand ScanLocation { get; }

        public const string Method = "UnknownLocation2";

        public UnknownLocationPageViewModel() : base()
        {
            Items = new ObservableCollection<UnknownLocationInfo>();
            Title = "Unknown Location";

            ScanLocation = new Command<UnknownLocationInfo>(async (info) =>
            {
                var page = new InventoryQuickScan()
                {
                    Item = info.Item
                };

                bool allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
                if (!allowed)
                {
                    return;
                }
                page.OnSuccess += (object sender, EventArgs e) =>
                {
                    Items.Remove(info);

                    //
                    // This fixes layout issues from removing item...
                    //
                    var tmp = Items.ToList();
                    Items = new ObservableCollection<UnknownLocationInfo>(tmp);
                };

                await Shell.Current.Navigation.PushAsync(page);
            });
        }

        protected override async Task Load()
        {
            try
            {
                IsLoading = true;
                Items?.Clear();

                var items = await Get<List<UnknownLocationInfo>>(Type, Method);

                foreach (var item in items)
                {
                    var trimmedStock = item.Item.Stock.Replace("+", null);
                    if (int.TryParse(trimmedStock, out int stock))
                    {
                        item.Item.Quantity = stock;
                    }
                    item.Item.Price = item.Item.Quantity * item.Item.Price;

                    Items?.Add(item);
                }
                HasSelectedCategory = true;
                IsLoading = false;
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
            }
        }
    }

    public class UnknownLocationInfo
    {
        public Item Item { get; set; }
        public List<InventoryEntry> Previous { get; set; }
    }
}
