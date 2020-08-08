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

        public float Subtotal => Items.Sum(i => i.Price);
        public float TaxedTotal => Subtotal * 1.075f;
        public QuotePageViewModel()
        {
            Title = "Quote";
            Items = new ObservableCollection<Item>();

            Items.CollectionChanged += (sender, args) => {
                OnPropertyChanged(nameof(Subtotal));
                OnPropertyChanged(nameof(TaxedTotal));
            };

            OnProductFound = new Command<Item>((Item item) =>
            {
                Items.Add(item);
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                await Shell.Current?.DisplayAlert("Error", message, "Ok");
            });
        }
    }
}