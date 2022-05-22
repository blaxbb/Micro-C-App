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
        public ObservableCollection<InventoryLocation> Items { get; }
        public FreshnessPageViewModel() : base()
        {
            Items = new ObservableCollection<InventoryLocation>();
            Title = "Freshness";
        }

        protected override async Task Load()
        {
            var result = await Get<List<InventoryLocation>>(Type, "Freshness");
            foreach(var item in result)
            {
                Items?.Add(item);
            }
        }
    }
}
