using micro_c_app.Models;
using micro_c_app.Views;
using MicroCLib.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using static MicroCLib.Models.BuildComponent;
using static MicroCLib.Models.BuildComponent.ComponentType;

namespace micro_c_app.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private Item item;
        private Stack<Item> itemQueue;
        private List<ComponentTypeInfo> categories;
        private string hintText;
        private bool hintVisible;
        private bool fastSearch;

        public Stack<Item> ItemQueue { get => itemQueue; set => SetProperty(ref itemQueue, value); }
        public ICommand PopItem { get; }
        public ICommand OnProductFound { get; }
        public ICommand OnProductFastFound { get; }
        public ICommand OnProductError { get; }
        public ICommand SearchCategory { get; }
        public INavigation Navigation { get; internal set; }
        public Item Item { get => item; set => SetProperty(ref item, value); }
        public bool FastSearch { get => fastSearch; set => SetProperty(ref fastSearch, value); }

        public ICommand GoToWebpage { get; }
        public ICommand AddReminder { get; }
        public ICommand DismissHint { get; }
        public string HintText { get => hintText; set => SetProperty(ref hintText, value); }
        public bool HintVisible { get => hintVisible; set => SetProperty(ref hintVisible, value); }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            { "Go to Webpage", GoToWebpage },
            { "Add Reminder",  AddReminder }
        };

        public List<ComponentTypeInfo> Categories { get => categories; set => SetProperty(ref categories, value); }
        private int HelpMessageIndex;

        public SearchViewModel()
        {
            Title = "Search";
            ItemQueue = new Stack<Item>();

            HintText = HelpMessages.GetNextMessage();
            if(HintText != null)
            {
                HintVisible = true;
            }

            Categories = SettingsPage.QuicksearchCategories();

            OnProductFound = new Command<Item>((Item item) =>
            {
                if (Item != null && !FastSearch)
                {
                    ItemQueue.Push(Item);
                }
                FastSearch = false;
                Item = item;
            });

            OnProductFastFound = new Command<Item>((Item item) =>
            {
                FastSearch = true;
                if (Item != null)
                {
                    ItemQueue.Push(Item);
                }
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

            PopItem = new Command(() =>
            {
                DoPopItem();
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
            DismissHint = new Command(() =>
            {
                HintVisible = false;
                SettingsPage.IncrementHelpMessageIndex();
            });

            MessagingCenter.Subscribe<SettingsPageViewModel>(this, SettingsPageViewModel.SETTINGS_UPDATED_MESSAGE, (_) => { UpdateProperties(); });
        }

        private void UpdateProperties()
        {
            Categories = SettingsPage.QuicksearchCategories();
        }

        public bool DoPopItem()
        {
            if (ItemQueue.Count > 0)
            {
                Item = ItemQueue.Pop();
                return true;
            }
            Item = null;
            return false;
        }
    }
}