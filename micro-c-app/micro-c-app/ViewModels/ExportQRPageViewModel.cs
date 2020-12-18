using micro_c_app.Models;
using micro_c_app.Views;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ExportQRPageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; }
        public ObservableCollection<string> ItemsJson { get; }

        public string CurrentJSON => (CurrentIndex < ItemsJson.Count ? ItemsJson?[CurrentIndex] : null) ?? "{}";
        public int CurrentIndex { get => currentIndex; set { SetProperty(ref currentIndex, value); OnPropertyChanged(nameof(CurrentJSON)); OnPropertyChanged(nameof(CurrentIndexLabel)); } }
        public int CurrentIndexLabel => CurrentIndex + 1;
        public int TotalCodes { get => totalCodes; set { SetProperty(ref totalCodes, value); } }

        public ICommand QRCodeBack { get; }
        public ICommand QRCodeForward { get; }

        public const int MAX_ITEMS_PER_SCAN = 4;
        private int currentIndex = 0;
        private int totalCodes;

        public ExportQRPageViewModel()
        {
            Title = "Export";
            Items = new ObservableCollection<Item>();
            ItemsJson = new ObservableCollection<string>();
            Items.CollectionChanged += ItemsChanged;

            QRCodeBack = new Command(() =>
            {
                if(CurrentIndex > 0)
                {
                    CurrentIndex--;
                }
            });
            QRCodeForward = new Command(() =>
            {
                if (CurrentIndex < ItemsJson.Count - 1)
                {
                    CurrentIndex++;
                }
            });
        }

        private void ItemsChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(Items.Count == 0)
            {
                return;
            }

            ItemsJson.Clear();
            for (int i = 0; i < Items.Count; i += MAX_ITEMS_PER_SCAN)
            {
                var slice = Items.Skip(i).Take(MAX_ITEMS_PER_SCAN);
                ItemsJson.Add(JsonConvert.SerializeObject(slice.Select(item => new ExportItem(item))));
            }
            TotalCodes = (int)Math.Ceiling(((float)Items.Count / MAX_ITEMS_PER_SCAN));
            OnPropertyChanged(nameof(CurrentJSON));
        }
    }
}