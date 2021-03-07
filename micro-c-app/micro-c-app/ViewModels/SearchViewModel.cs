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

        public Stack<Item> ItemQueue { get => itemQueue; set => SetProperty(ref itemQueue, value); }
        public ICommand PopItem { get; }
        public ICommand OnProductFound { get; }
        public ICommand OnProductError { get; }
        public ICommand SearchCategory { get; }
        public INavigation Navigation { get; internal set; }
        public Item Item { get => item; set => SetProperty(ref item, value); }

        public ICommand GoToWebpage { get; }
        public ICommand AddReminder { get; }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            { "Go to Webpage", GoToWebpage },
            { "Add Reminder",  AddReminder }
        };

        public List<ComponentTypeInfo> Categories { get => categories; set => SetProperty(ref categories, value); }

        public SearchViewModel()
        {
            Title = "Search";
            ItemQueue = new Stack<Item>();

            Categories = PresetBYO().ToList();

            OnProductFound = new Command<Item>((Item item) =>
            {
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

        #region PRESETS

        public static IEnumerable<ComponentTypeInfo> PresetBYO()
        {
            yield return new ComponentTypeInfo(BuildService, "\uf7d9");
            yield return new ComponentTypeInfo(ComponentType.OperatingSystem, "\uf17a");
            yield return new ComponentTypeInfo(CPU, "\uf2db");
            yield return new ComponentTypeInfo(Motherboard);
            yield return new ComponentTypeInfo(RAM, "\uf538");
            yield return new ComponentTypeInfo(Case, "\uf0c8");
            yield return new ComponentTypeInfo(PowerSupply, "\uf5df");
            yield return new ComponentTypeInfo(GPU, "\uf1b3");
            yield return new ComponentTypeInfo(SSD, "\uf0a0");
            yield return new ComponentTypeInfo(HDD, "\uf0a0");
            yield return new ComponentTypeInfo(CPUCooler, "\uf76b");
            yield return new ComponentTypeInfo(WaterCoolingKit, "\uf76b");
            yield return new ComponentTypeInfo(CaseFan, "\uf863");
        }
        public static IEnumerable<ComponentTypeInfo> PresetSystems()
        {
            yield return new ComponentTypeInfo(Desktop, "\uf0c8");
            yield return new ComponentTypeInfo(Laptop, "\uf109");
            yield return new ComponentTypeInfo(Monitor, "\uf108");
            yield return new ComponentTypeInfo(ComponentType.Keyboard, "\uf11c");
            yield return new ComponentTypeInfo(Mouse, "\uf8cc");
            yield return new ComponentTypeInfo(Printer, "\uf02f");
        }

        public static IEnumerable<ComponentTypeInfo> PresetGS()
        {
            yield return new ComponentTypeInfo(WirelessRouter, "\uf1eb");
            yield return new ComponentTypeInfo(WiredRouter, "\uf796");
            yield return new ComponentTypeInfo(WiredNetworkAdapter, "\uf796");
            yield return new ComponentTypeInfo(NetworkingPowerline, "\uf796");
            yield return new ComponentTypeInfo(POENetworkAdapter, "\uf796");
            yield return new ComponentTypeInfo(NetworkSwitch, "\uf6ff");
            yield return new ComponentTypeInfo(WirelessAdapter, "\uf1eb");
            yield return new ComponentTypeInfo(WirelessAccessPoint, "\uf1eb");
            yield return new ComponentTypeInfo(WirelessBoosters, "\uf1eb");
            yield return new ComponentTypeInfo(NetworkingBridge, "\uf1eb");
            yield return new ComponentTypeInfo(NetworkingCable, "\uf1eb");
            yield return new ComponentTypeInfo(NetworkingAccessory, "\uf1eb");
            yield return new ComponentTypeInfo(NetworkAttachedStorage, "\uf0a0");
            yield return new ComponentTypeInfo(BluetoothAdapter, "\uf294");
            yield return new ComponentTypeInfo(ComponentType.Keyboard, "\uf11c");
            yield return new ComponentTypeInfo(Mouse, "\uf8cc");
            yield return new ComponentTypeInfo(Headphones, "\uf025");
            yield return new ComponentTypeInfo(Speakers, "\uf028");
            yield return new ComponentTypeInfo(ExternalDrives, "\uf0a0");
            yield return new ComponentTypeInfo(UninteruptablePowerSupply, "\uf5df");
            yield return new ComponentTypeInfo(GameAccessories, "\uf11b");
            yield return new ComponentTypeInfo(GameControllers, "\uf11b");
            yield return new ComponentTypeInfo(Xbox, "\uf412");
            yield return new ComponentTypeInfo(Playstation, "\uf3df");
            yield return new ComponentTypeInfo(Nintendo, "\uf11b");
            yield return new ComponentTypeInfo(InkAndToner, "\uf02f");
        }

        public static IEnumerable<ComponentTypeInfo> PresetCE()
        {
            yield return new ComponentTypeInfo(Television, "\uf26c");
            yield return new ComponentTypeInfo(HomeTheaterAudio, "\uf008");
            yield return new ComponentTypeInfo(HomeTheaterWireless, "\uf1eb");
            yield return new ComponentTypeInfo(StreamingMedia, "\uf03d");
            yield return new ComponentTypeInfo(Printer, "\uf02f");
            yield return new ComponentTypeInfo(InkAndToner, "\uf02f");
            yield return new ComponentTypeInfo(SecurityCamera, "\uf030");
            yield return new ComponentTypeInfo(SecurityCameraKit, "\uf030");
            yield return new ComponentTypeInfo(HomeAutomation, "\uf0d0");
            yield return new ComponentTypeInfo(Projectors, "\uf03d");
            yield return new ComponentTypeInfo(DigitalCamera, "\uf030");
            yield return new ComponentTypeInfo(FlashMemory, "\uf7c2");
        }
        #endregion
    }
}