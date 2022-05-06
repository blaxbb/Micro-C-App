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

namespace micro_c_app.ViewModels.InventoryAudit
{
    public class UnknownLocationPageViewModel : BaseInventoryViewModel
    {
        private ObservableCollection<Item> items;
        public ObservableCollection<Item> Items { get => items; set => SetProperty(ref items, value); }

        public UnknownLocationPageViewModel() : base()
        {
            Items = new ObservableCollection<Item>();
        }

        protected override async Task Load()
        {
            try
            {
                IsLoading = true;
                Items?.Clear();

                var items = await Get<List<Item>>(Type, "UnknownLocation");

                foreach (var item in items)
                {
                    var trimmedStock = item.Stock.Replace("+", null);
                    if (int.TryParse(trimmedStock, out int stock))
                    {
                        item.Quantity = stock;
                    }
                    item.Price = item.Quantity * item.Price;

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
}
