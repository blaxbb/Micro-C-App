using micro_c_app.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class QuotePageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; set; }
        public ICommand OnProductFound { get; }
        public ICommand OnProductError { get; }
        public ICommand IncreaseQuantity { get; }
        public ICommand DecreaseQuantity { get; }
        public ICommand RemoveItem { get; }

        public float Subtotal => Items.Sum(i => i.Price * i.Quantity);
        public float TaxedTotal => Subtotal * 1.075f;
        public QuotePageViewModel()
        {
            Title = "Quote";
            Items = new ObservableCollection<Item>()
            {
                new Item()
                {
                    Name = "ITEM",
                    SKU = "123456",
                    OriginalPrice = 100,
                    Price = 90,
                    Stock = "5 in stock",
                }
            };

            Items.CollectionChanged += (sender, args) =>
            {
                UpdateProperties();
            };

            OnProductFound = new Command<Item>((Item item) =>
            {
                Items.Add(item);
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                await Shell.Current?.DisplayAlert("Error", message, "Ok");
            });

            IncreaseQuantity = new Command<Item>((Item item) =>
            {
                item.Quantity++;
                UpdateProperties();
            });

            DecreaseQuantity = new Command<Item>((Item item) =>
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    UpdateProperties();
                }
            });
            RemoveItem = new Command<Item>((Item item) =>
            {
                Items.Remove(item);
            });

        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TaxedTotal));
        }
    }
}