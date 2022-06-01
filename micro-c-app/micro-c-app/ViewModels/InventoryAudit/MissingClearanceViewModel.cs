using micro_c_lib.Models;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class MissingClearanceViewModel : BaseInventoryViewModel
    {
        public ObservableCollection<MissingClearanceInfo> Items { get; set; }
        public const string Method = "Clearance";
        public MissingClearanceViewModel() : base()
        {
            Items = new ObservableCollection<MissingClearanceInfo>();
            Title = "Clearance";
        }

        protected override async Task Load()
        {
            try
            {
                IsLoading = true;
                Items?.Clear();

                var items = await Get<List<MissingClearanceInfo>>(Type, Method);

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

    public class MissingClearanceInfo
    {
        public Item Item { get; set; }
        public List<ClearanceInfo> MissingClearance { get; set; }
    }
}