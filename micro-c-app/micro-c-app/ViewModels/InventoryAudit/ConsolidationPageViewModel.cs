using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class ConsolidationPageViewModel : BaseInventoryViewModel
    {
        public ObservableCollection<ConsolidationInfo> Items { get; set; }

        public ConsolidationPageViewModel() : base()
        {
            Items = new ObservableCollection<ConsolidationInfo>();
        }

        protected override async Task Load()
        {
            try
            {
                IsLoading = true;
                Items?.Clear();
                var tmp = new List<ConsolidationInfo>();
                var items = await Get<Dictionary<int, List<Item>>>(Type, "Consolidation");
                var entries = await GetEntries(Type);
                foreach (var kvp in items)
                {
                    var consolidationInfo = new ConsolidationInfo(kvp.Key);
                    foreach (var item in kvp.Value)
                    {
                        if (entries?.ContainsKey(item.SKU) ?? false)
                        {
                            consolidationInfo.Items.Add(new Tuple<Item, List<InventoryEntry>>(item, entries[item.SKU]));
                        }
                    }
                    tmp.Add(consolidationInfo);
                }
                tmp.OrderByDescending(i => i.Count).ToList().ForEach(i => Items?.Add(i));
                HasSelectedCategory = true;
                IsLoading = false;
                OnPropertyChanged(nameof(Items));

            }
            catch(Exception e)
            {
                AnalyticsService.TrackError(e);
            }
        }
    }

    public class ConsolidationInfo
    {
        public int Count { get; set; }
        public List<Tuple<Item, List<InventoryEntry>>> Items { get; set; }

        public ConsolidationInfo(int count)
        {
            Count = count;
            Items = new List<Tuple<Item, List<InventoryEntry>>>();
        }
    }
}
