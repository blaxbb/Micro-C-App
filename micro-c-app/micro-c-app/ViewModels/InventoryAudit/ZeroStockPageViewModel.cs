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

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class ZeroStockPageViewModel : BaseViewModel
    {
        private ComponentType type;
        private ObservableCollection<Tuple<Item, List<InventoryEntry>>> items;
        private bool hasSelectedCategory;
        private bool isLoading;

        public ComponentType Type { get => type; set => SetProperty(ref type, value); }
        public ObservableCollection<Tuple<Item, List<InventoryEntry>>> Items { get => items; set => SetProperty(ref items, value); }

        public ICommand CategoryCommand { get; }
        public bool HasSelectedCategory { get => hasSelectedCategory; set => SetProperty(ref hasSelectedCategory, value); }

        public bool IsLoading { get => isLoading; set => SetProperty(ref isLoading, value); }

        public const string BASE_URL = "https://location.bbarrett.me/api/Audit";

        public ZeroStockPageViewModel()
        {
            Items = new ObservableCollection<Tuple<Item, List<InventoryEntry>>>();
            CategoryCommand = new Command(async () =>
            {
                var result = await Shell.Current.DisplayActionSheet("Select Category", "Cancel", null, Enum.GetNames(typeof(ComponentType)));
                if (result == "Cancel" || result == null)
                {
                    return;
                }
                else
                {
                    if (Enum.TryParse(result, out type))
                    {
                        Type = type;
                        await Load();
                    }
                }
            });
        }

        async Task Load()
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
                OnPropertyChanged(nameof(Type));
                //OnPropertyChanged(nameof(Items));
            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e);
            }
        }
    }
}
