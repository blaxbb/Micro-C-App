using micro_c_app.Views;
using micro_c_lib.Models;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class MissingClearanceViewModel : BaseInventoryViewModel
    {
        public ObservableCollection<MissingClearanceInfo> Items { get; set; }
        public const string Method = "Clearance";

        public ICommand ScanLocation { get; }

        public MissingClearanceViewModel() : base()
        {
            Items = new ObservableCollection<MissingClearanceInfo>();
            Title = "Clearance";

            ScanLocation = new Command<ClearanceInfo>(async (info) =>
            {
                var page = new InventoryQuickScan()
                {
                    Item = new Item()
                    {
                        SKU = info.Id,
                        Name = info.Id
                    }
                };

                bool allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
                if (!allowed)
                {
                    return;
                }
                page.OnSuccess += (object sender, EventArgs e) =>
                {
                    var parent = Items.FirstOrDefault(i => i.MissingClearance.Contains(info));
                    if(parent != null)
                    {
                        parent.MissingClearance.Remove(info);

                        Items.Remove(parent);
                        Items.Add(parent);

                        //
                        // This fixes layout issues from removing item...
                        //
                        var tmp = Items.ToList();
                        Items = new ObservableCollection<MissingClearanceInfo>(tmp);
                    }

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