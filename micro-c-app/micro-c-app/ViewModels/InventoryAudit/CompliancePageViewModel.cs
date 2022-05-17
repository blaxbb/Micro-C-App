using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class CompliancePageViewModel : BaseInventoryViewModel
    {
        private ObservableCollection<ComplianceReport> items;

        public ObservableCollection<ComplianceReport> Items { get => items; set => SetProperty(ref items, value); }

        public CompliancePageViewModel() : base()
        {
            Items = new ObservableCollection<ComplianceReport>();
        }

        protected override async Task Load()
        {
            try
            {
                IsLoading = true;
                Items?.Clear();

                var items = await Get<List<ComplianceReport>>(Type, "Compliance");

                foreach (var item in items)
                {
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

    public class ComplianceReport
    {
        public InventoryLocation Location { get; set; }
        public List<Item> SkuFailures { get; set; }
        public List<Item> BrandFailures { get; set; }
    }
}
