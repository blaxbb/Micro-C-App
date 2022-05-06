using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using micro_c_app.Views;
using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using Newtonsoft.Json;
using Xamarin.Forms;
using static MicroCLib.Models.BuildComponent;
using System.Collections.Generic;
using System.Linq;

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class ZeroStockPageViewModel : BaseInventoryViewModel
    {
        private ObservableCollection<Tuple<Item, List<InventoryEntry>>> items;

        public ObservableCollection<Tuple<Item, List<InventoryEntry>>> Items { get => items; set => SetProperty(ref items, value); }

        public ZeroStockPageViewModel() : base()
        {
            Items = new ObservableCollection<Tuple<Item, List<InventoryEntry>>>();
        }

        protected override async Task Load()
        {
            try
            {
                IsLoading = true;
                Items?.Clear();

                using var client = new HttpClient();
                var store = SettingsPage.StoreID();

                var stockResult = await client.GetStringAsync($"{BASE_URL}/ZeroStock/{store}/{(int)Type}");
                if (string.IsNullOrEmpty(stockResult))
                {
                    return;
                }

                var entriesResult = await client.GetStringAsync($"{BASE_URL}/Entries/{store}/{(int)Type}");

                if (string.IsNullOrWhiteSpace(entriesResult))
                {
                    return;
                }

                var items = JsonConvert.DeserializeObject<List<Item>>(stockResult);
                var entries = JsonConvert.DeserializeObject<Dictionary<string, List<InventoryEntry>>>(entriesResult);

                foreach(var item in items)
                {
                    if (entries?.ContainsKey(item.SKU) ?? false)
                    {
                        Items.Add(new Tuple<Item, List<InventoryEntry>>(item, entries[item.SKU]));
                    }
                    else
                    {
                        Items.Add(new Tuple<Item, List<InventoryEntry>>(item, new List<InventoryEntry>()));
                    }
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
}
