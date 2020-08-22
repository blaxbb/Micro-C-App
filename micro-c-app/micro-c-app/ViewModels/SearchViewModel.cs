using micro_c_app.Models;
using micro_c_app.Views;
using System.Windows.Input;
using Xamarin.Forms;

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
                var detailsPage = new ItemDetailsPageViewModel() { Item = item };

                await Navigation.PushAsync(new ItemDetailsPage() { BindingContext = detailsPage });
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                await Shell.Current?.DisplayAlert("Error", message, "Ok");
            });



        }
    }
}