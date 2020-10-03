using micro_c_app.Models;
using micro_c_app.Views;
using MicroCLib.Models;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private Item item;

        public ICommand OnProductFound { get; }
        public ICommand OnProductError { get; }
        public INavigation Navigation { get; internal set; }
        public Item Item { get => item; set => SetProperty(ref item, value); }

        public SearchViewModel()
        {
            Title = "Search";
            Item = new Item()
            {
                Name = "AJFKLJSKLF"
            };

            OnProductFound = new Command<Item>(async (Item item) =>
            {
                Item = item;
                return;
                var detailsPage = new ItemDetailsPageViewModel() { Item = item };
                await Navigation?.PushAsync(new ItemDetailsPage() { BindingContext = detailsPage });
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", message, "Ok");
                }
            });
        }
    }
}