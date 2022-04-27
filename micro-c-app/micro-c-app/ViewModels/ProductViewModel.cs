using micro_c_app.Models;
using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ProductViewModel : BaseViewModel
    {
        public List<string> Tabs { get; } = new List<string>() { "Specs", "Location", "Plans", "Clearance" };
        public string ActiveTab { get => activeTab; set => SetProperty(ref activeTab, value); }

        private Item? _item;
        private string activeTab;

        private bool fastView;
        private string _upc;
        private string _picture;

        public bool FastView { get => fastView; set { SetProperty(ref fastView, value); } }

        public Item? Item
        {
            get => _item;
            set
            {
                SetProperty(ref _item, value);
                SetItem();
            }
        }

        public string UPC { get => _upc; set => SetProperty(ref _upc, value); }
        public string Picture { get => _picture; set => SetProperty(ref _picture, value); }

        public ICommand PictureSwipeForward { get; }
        public ICommand PictureSwipeBack { get; }

        public ICommand SetTab { get; }

        public Dictionary<string, string> Specs { get; set; }
        public List<InventoryEntry> Locations { get; set; }

        public ProductViewModel()
        {
            ActiveTab = Tabs[0];

            UPC = "123567890123";
            Picture = "https://90a1c75758623581b3f8-5c119c3de181c9857fcb2784776b17ef.ssl.cf2.rackcdn.com/632020_243147_01_front_thumbnail.jpg";
            Specs = new Dictionary<string, string>()
            {
                {"aaa", "123" },
                {"bbb", "123" },
                {"acccdcc", "123" }
            };

            SetTab = new Command<string>((tab) => ActiveTab = tab);
        }

        void SetItem()
        {
            if (Item == null)
            {
                return;
            }

            UPC = Item.Specs.ContainsKey("UPC") ? Item.Specs["UPC"] : "";
            Picture = Item.PictureUrls.FirstOrDefault();
            Specs = Item.Specs;
            OnPropertyChanged(nameof(Specs));
            Locations = Item.InventoryEntries;
            OnPropertyChanged(nameof(Locations));

        }


    }
}
