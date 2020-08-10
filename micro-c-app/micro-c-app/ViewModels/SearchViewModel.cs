using micro_c_app.Models;
using micro_c_app.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public ICommand OnProductFound { get; }
        public ICommand OnProductError { get; }
        public INavigation Navigation { get; internal set; }

        public SearchViewModel()
        {
            Title = "Search";

            OnProductFound = new Command<Item>(async (Item item) =>
            {
                var detailsPage = new ItemDetailsViewModel() { Item = item };

                await Navigation.PushAsync(new ItemDetails() { BindingContext = detailsPage });
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                await Shell.Current?.DisplayAlert("Error", message, "Ok");
            });



        }
    }
}