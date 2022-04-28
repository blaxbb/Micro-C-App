using micro_c_app.Models;
using micro_c_app.Views;
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
        public List<string> Tabs { get; set; } = new List<string>() { "Specs", "Location", "Plans", "Clearance" };
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
        int PictureIndex = 0;
        private string store;

        public ICommand PictureSwipeForward { get; }
        public ICommand PictureSwipeBack { get; }

        public ICommand SetTab { get; }

        public Dictionary<string, string> Specs { get; set; }
        public List<InventoryEntry> Locations { get; set; }

        public string Store { get => store; set => SetProperty(ref store, value); }

        public ProductViewModel()
        {
            ActiveTab = Tabs[0];
            Store = SettingsPage.StoreID();
            if(Store != "141")
            {
                Tabs.Remove("Location");
            }

            UPC = "123567890123";
            Picture = "https://90a1c75758623581b3f8-5c119c3de181c9857fcb2784776b17ef.ssl.cf2.rackcdn.com/632020_243147_01_front_thumbnail.jpg";
            Specs = new Dictionary<string, string>()
            {
                {"aaa", "123" },
                {"bbb", "123" },
                {"acccdcc", "123" }
            };

            PictureSwipeBack = new Command(() => BackPicture());
            PictureSwipeForward = new Command(() => ForwardPicture());

            SetTab = new Command<string>((tab) => ActiveTab = tab);
        }

        void BackPicture()
        {
            if (Item?.PictureUrls == null || Item.PictureUrls.Count == 0)
            {
                return;
            }

            if (PictureIndex == 0)
            {
                PictureIndex = Item.PictureUrls.Count;
            }

            PictureIndex--;
            if (PictureIndex < 0)
            {
                PictureIndex = 0;
            }

            Picture = Item.PictureUrls[PictureIndex];
        }

        void ForwardPicture()
        {
            if (Item?.PictureUrls == null || Item.PictureUrls.Count == 0)
            {
                return;
            }

            PictureIndex++;

            if (PictureIndex == Item.PictureUrls.Count)
            {
                PictureIndex = 0;
            }

            Picture = Item.PictureUrls[PictureIndex];
        }

        void SetItem()
        {
            Console.WriteLine("SET ITEM");
            if (Item == null)
            {
                return;
            }

            UPC = Item.Specs.ContainsKey("UPC") ? Item.Specs["UPC"] : "";

            PictureIndex = 0;
            Picture = Item.PictureUrls.FirstOrDefault();

            Specs = Item.Specs;
            OnPropertyChanged(nameof(Specs));
            Locations = Item.InventoryEntries;
            OnPropertyChanged(nameof(Locations));

        }


    }
}
