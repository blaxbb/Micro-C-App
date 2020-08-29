using micro_c_app.Models;
using micro_c_app.Views;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ItemDetailsPageViewModel : BaseViewModel
    {
        public Item Item { get; set; }
        public string ActivePicture => Item?.PictureUrls?[PictureIndex];
        int PictureIndex = 0;
        public ICommand BackPicture { get; }
        public ICommand ForwardPicture { get; }
        public ICommand GoToWebpage { get; }
        public ICommand AddReminder { get; }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            { "Go to Webpage", GoToWebpage },
            { "Add Reminder",  AddReminder }
        };

        public ItemDetailsPageViewModel()
        {
            Title = "Details";
            //Item = new Item()
            //{
            //    Name = "Item",
            //    OriginalPrice = 1000.00f,
            //    Price = 900.50f,
            //    Location = "Location 000",
            //    SKU = "123456",
            //    Stock = "6 in stock",
            //    Plans = new List<Plan>() {
            //        new Plan()
            //        {
            //            Name = "PLAN 1",
            //            Price = 100f
            //        },
            //    }
            //};

            BackPicture = new Command(() =>
            {
                if (PictureIndex > 0)
                {
                    PictureIndex--;
                    OnPropertyChanged(nameof(ActivePicture));
                }
                else if (Item != null && Item?.PictureUrls?.Count > 1)
                {
                    PictureIndex = Item.PictureUrls.Count - 1;
                    OnPropertyChanged(nameof(ActivePicture));
                }
            });

            ForwardPicture = new Command(() =>
            {
                if (PictureIndex < Item?.PictureUrls?.Count - 1)
                {
                    PictureIndex++;
                    OnPropertyChanged(nameof(ActivePicture));
                }
                else if (Item != null && Item?.PictureUrls?.Count > 1)
                {
                    PictureIndex = 0;
                    OnPropertyChanged(nameof(ActivePicture));
                }
            });

            GoToWebpage = new Command(async () =>
            {
                await Xamarin.Essentials.Browser.OpenAsync($"https://microcenter.com{Item.URL}", Xamarin.Essentials.BrowserLaunchMode.SystemPreferred);
            });

            AddReminder = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var vm = new ReminderEditPageViewModel();
                    vm.Reminder = new Reminder(Item);
                    vm.NewItem = true;
                    await Shell.Current.Navigation.PushAsync(new ReminderEditPage() { BindingContext = vm });
                });
            });
        }
    }
}