using micro_c_app.Models;
using micro_c_app.Views;
using MicroCLib.Models;
using System.Collections.Generic;
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

        public ICommand GoToWebpage { get; }
        public ICommand AddReminder { get; }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            { "Go to Webpage", GoToWebpage },
            { "Add Reminder",  AddReminder }
        };

        public SearchViewModel()
        {
            Title = "Search";

            OnProductFound = new Command<Item>(async (Item item) =>
            {
                Item = item;
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", message, "Ok");
                }
            });

            GoToWebpage = new Command(async () =>
            {
                if (Item != null)
                {
                    await Xamarin.Essentials.Browser.OpenAsync($"https://microcenter.com{Item.URL}", Xamarin.Essentials.BrowserLaunchMode.SystemPreferred);
                }
            });

            AddReminder = new Command(async () =>
            {
                if (Item != null)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var vm = new ReminderEditPageViewModel();
                        vm.Reminder = new Reminder(Item);
                        vm.NewItem = true;
                        await Shell.Current.Navigation.PushAsync(new ReminderEditPage() { BindingContext = vm });
                    });
                }
            });
        }
    }
}