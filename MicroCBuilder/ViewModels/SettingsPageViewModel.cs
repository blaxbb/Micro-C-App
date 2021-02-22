using Lucene.Net.Util;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Primitives;
using static MicroCLib.Models.BuildComponent;
using static MicroCLib.Models.BuildComponent.ComponentType;

namespace MicroCBuilder.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        Dictionary<string, string> AllStores => Stores.AllStores;
        public List<string> StoreNames => AllStores.Keys.ToList();
        public string SelectedStore { get => selectedStore; set { SetProperty(ref selectedStore, value); Settings.Store(value); ForceUpdateCommand?.Execute(null); } }
        public double TaxRate { get => taxRate; set { SetProperty(ref taxRate, value); Settings.TaxRate(value); } }
        public TimeSpan LastUpdated { get => lastUpdated; set => SetProperty(ref lastUpdated, value); }

        public ObservableCollection<ComponentType> Categories { get; set; }
        public ObservableCollection<ComponentType> HiddenCategories { get; set; }
        public ObservableCollection<string> PresetNames { get; set; }
        public int NewCategoryIndex { get => newCategoryIndex; set { SetProperty(ref newCategoryIndex, value); HandleNewCategory(); } }
        public int SelectedPresetIndex { get => selectedPresetIndex; set { SetProperty(ref selectedPresetIndex, value); HandleNewPreset(); } }

        public ICommand ForceUpdateCommand { get; }
        public ICommand ForceDeepUpdateCommand { get; }
        public ICommand RemoveCategory { get; }

        private string selectedStore;
        private double taxRate;
        private TimeSpan lastUpdated;
        private int newCategoryIndex;
        private int selectedPresetIndex;

        public delegate void ForceUpdateEvent();
        public static event ForceUpdateEvent ForceUpdate;

        public Dictionary<string, List<ComponentType>> Presets { get; set; }

        public SettingsPageViewModel()
        {
            SelectedStore = Settings.Store();
            TaxRate = Settings.TaxRate();
            var catsss = Settings.Value<string>(Settings.CATEGORIES_KEY);
            var currentCategories = Settings.Categories();
            var allCategories = Enum.GetValues(typeof(ComponentType)).Cast<ComponentType>().ToList();

            Categories = new ObservableCollection<ComponentType>(currentCategories);
            Categories.CollectionChanged += Categories_CollectionChanged;
            HiddenCategories = new ObservableCollection<ComponentType>(allCategories.Where(c => !currentCategories.Contains(c)));
            NewCategoryIndex = -1;

            Presets = new Dictionary<string, List<ComponentType>>()
            {
                { "Preset categories", null },
                { "BYO",  PresetBYO().ToList() },
                { "Systems", PresetSystems().ToList() },
                { "CE", PresetCE().ToList() },
                { "GS", PresetGS().ToList() }
            };
            PresetNames = new ObservableCollection<string>(Presets.Keys);


            var updateTime = Settings.LastUpdated();
            LastUpdated = DateTime.Now - updateTime;

            ForceUpdateCommand = new Command((_) =>
            {
                ForceUpdate?.Invoke();
            });

            RemoveCategory = new Command<ComponentType>((cat) =>
            {
                Categories.Remove(cat);
                HiddenCategories.Add(cat);
            });
        }



        private void Categories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    Settings.Categories(Categories.ToList());
                    break;
            }
        }

        private void HandleNewCategory()
        {
            if (newCategoryIndex >= 0 && HiddenCategories.Count > newCategoryIndex)
            {
                var cat = HiddenCategories[newCategoryIndex];
                NewCategoryIndex = -1;
                var dispatcher = Window.Current.Dispatcher;
                dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    Task.Delay(1);
                    Categories.Add(cat);
                    HiddenCategories.Remove(cat);
                });

            }
        }

        private void HandleNewPreset()
        {
            if (SelectedPresetIndex > 0 && SelectedPresetIndex < PresetNames.Count)
            {
                var preset = PresetNames[SelectedPresetIndex];
                if(preset == null)
                {
                    return;
                }

                SelectedPresetIndex = 0;
                var allCategories = Enum.GetValues(typeof(ComponentType)).Cast<ComponentType>();
                var collection = Presets[preset];
                Categories.Clear();
                HiddenCategories.Clear();
                foreach (var cat in allCategories)
                {
                    if (collection.Contains(cat))
                    {
                        Categories.Add(cat);
                    }
                    else
                    {
                        HiddenCategories.Add(cat);
                    }
                }
            }
        }

        private static IEnumerable<ComponentType> PresetBYO()
        {
            yield return BuildService;
            yield return ComponentType.OperatingSystem;
            yield return CPU;
            yield return Motherboard;
            yield return RAM;
            yield return Case;
            yield return PowerSupply;
            yield return GPU;
            yield return SSD;
            yield return HDD;
            yield return CPUCooler;
            yield return WaterCoolingKit;
            yield return CaseFan;
        }
        private static IEnumerable<ComponentType> PresetSystems()
        {
            yield return Desktop;
            yield return Laptop;
            yield return Monitor;
            yield return Keyboard;
            yield return Mouse;
            yield return Printer;
        }

        private static IEnumerable<ComponentType> PresetGS()
        {
            yield return WirelessRouter;
            yield return WiredRouter;
            yield return WiredNetworkAdapter;
            yield return NetworkingPowerline;
            yield return POENetworkAdapter;
            yield return NetworkSwitch;
            yield return WirelessAdapter;
            yield return WirelessAccessPoint;
            yield return WirelessBoosters;
            yield return NetworkingBridge;
            yield return NetworkingCable;
            yield return NetworkingAccessory;
            yield return NetworkAttachedStorage;
            yield return BluetoothAdapter;
            yield return Keyboard;
            yield return Mouse;
            yield return Headphones;
            yield return Speakers;
            yield return ExternalDrives;
            yield return UninteruptablePowerSupply;
            yield return GameAccessories;
            yield return GameControllers;
            yield return Xbox;
            yield return Playstation;
            yield return Nintendo;
            yield return InkAndToner;
        }

        private static IEnumerable<ComponentType> PresetCE()
        {
            yield return Television;
            yield return HomeTheaterAudio;
            yield return HomeTheaterWireless;
            yield return StreamingMedia;
            yield return Printer;
            yield return InkAndToner;
            yield return SecurityCamera;
            yield return SecurityCameraKit;
            yield return HomeAutomation;
            yield return Projectors;
            yield return DigitalCamera;
            yield return FlashMemory;
        }
    }
}
