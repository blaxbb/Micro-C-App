using micro_c_app.Models;
using micro_c_app.Views;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ItemDetailsViewViewModel : BaseViewModel
    {
        private Item? item;
        public Item? Item { get => item;
            set {
                SetProperty(ref item, value);
                PictureIndex = 0;
                OnPropertyChanged(nameof(ActivePicture));
            }
        }
        public string? ActivePicture
        {
            get
            {
                if(Item?.PictureUrls == null)
                {
                    return "";
                }
                if(PictureIndex >= item.PictureUrls.Count)
                {
                    return "";
                }
                return Item?.PictureUrls?[PictureIndex];
            }
        }
        int PictureIndex = 0;

        private bool fastView;
        public bool FastView { get => fastView; set { SetProperty(ref fastView, value); } }

        public ICommand BackPicture { get; }
        public ICommand ForwardPicture { get; }
        public ICommand GoToWebpage { get; }
        public ICommand AddReminder { get; }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            { "Go to Webpage", GoToWebpage },
            { "Add Reminder",  AddReminder }
        };

        public ItemDetailsViewViewModel()
        {
            Title = "Details";

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
                if (Item != null)
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        var vm = new ReminderEditPageViewModel
                        {
                            Reminder = new Reminder(Item),
                            NewItem = true
                        };
                        await Shell.Current.Navigation.PushAsync(new ReminderEditPage() { BindingContext = vm });
                    });
                }
            });
        }
    }
}
