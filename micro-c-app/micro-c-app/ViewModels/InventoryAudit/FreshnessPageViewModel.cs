using micro_c_lib.Models.Inventory;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class FreshnessPageViewModel : BaseInventoryViewModel
    {
        public ObservableCollection<FreshnessInfo> Items { get; }
        public const string Method = "Freshness2";

        public FreshnessPageViewModel() : base()
        {
            Items = new ObservableCollection<FreshnessInfo>();
            Title = "Freshness";
        }

        protected override async Task Load()
        {
            var result = await Get<List<FreshnessInfo>>(Type, Method);
            foreach(var item in result)
            {
                Items?.Add(item);
            }
        }
    }

    public class FreshnessInfo
    {
        public InventoryLocation Location { get; set; }
        public int Freshness { get; set; }
    }
}
