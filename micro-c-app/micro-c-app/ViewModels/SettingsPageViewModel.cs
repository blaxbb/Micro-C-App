using micro_c_app.Models;
using micro_c_app.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        public List<string> StoreNames { get; set; }
        public string SelectedStoreName { get; set; }

        public static Dictionary<string, string> Stores { get; private set; }
        public string StoreID { get; set; }
        public string SalesID { get; set; }
        public float TaxRate { get; set; }
        public bool IncludeCSVWithQuote { get; set; }

        public List<OSAppTheme> ThemeOptions { get; set; }
        public OSAppTheme Theme { get; set; }

        public bool Vibrate { get; set; }
        public bool AnalyticsEnabled { get; set; }
        public ICommand Save { get; }
        public ICommand Cancel { get; }

        public bool EnhancedSearch { get; set; }

        public List<ComponentTypeInfo> QuicksearchCategories { get => quicksearchCategories; set => SetProperty(ref quicksearchCategories, value); }
        public List<ComponentTypeInfo> AllCategories { get; set; }
        public List<string> AllCategoryNames { get => allCategoryNames; set => SetProperty(ref allCategoryNames, value); }
        public List<string> Presets { get => presets; set => SetProperty(ref presets, value); }
        public string SelectedNewItem { get => selectedNewItem; set => SetProperty(ref selectedNewItem, value); }
        public int SelectedNewItemIndex
        {
            get => selectedNewItemIndex;
            set
            {
                SetProperty(ref selectedNewItemIndex, value);
                if (value != -1)
                {
                    if (value >= 0 && value < AllCategories.Count)
                    {
                        var cat = AllCategories[value];
                        QuicksearchCategories.Add(cat);
                        OnPropertyChanged(nameof(QuicksearchCategories));
                    }
                }
            }
        }

        public string SelectedPreset
        {
            get => selectedPreset;
            set
            {
                selectedPreset = value;
                switch (value)
                {
                    case "BYO":
                        QuicksearchCategories = SettingsPage.PresetBYO().ToList();
                        break;
                    case "Systems":
                        QuicksearchCategories = SettingsPage.PresetSystems().ToList();
                        break;
                    case "GS":
                        QuicksearchCategories = SettingsPage.PresetGS().ToList();
                        break;
                    case "CE":
                        QuicksearchCategories = SettingsPage.PresetCE().ToList();
                        break;
                }
            }
        }

        public const string SETTINGS_UPDATED_MESSAGE = "updated";
        private List<ComponentTypeInfo> quicksearchCategories;
        private string selectedPreset;
        private List<string> presets;
        private List<string> allCategoryNames;
        private int selectedNewItemIndex = -2;
        private string selectedNewItem;

        public SettingsPageViewModel()
        {
            Save = new Command(async (o) => await DoSave(o));
            Cancel = new Command(async () => await ExitSettings());


            Title = "Settings";
            StoreID = SettingsPage.StoreID();
            SalesID = SettingsPage.SalesID();
            TaxRate = SettingsPage.TaxRate();
            IncludeCSVWithQuote = SettingsPage.IncludeCSVWithQuote();

            ThemeOptions = System.Enum.GetValues(typeof(OSAppTheme)).Cast<OSAppTheme>().ToList();
            Theme = SettingsPage.Theme();

            Stores = MicroCLib.Models.Stores.AllStores;

            StoreNames = Stores.Keys.ToList();
            SelectedStoreName = Stores.FirstOrDefault(kvp => kvp.Value == StoreID).Key;

            Vibrate = SettingsPage.Vibrate();
            AnalyticsEnabled = SettingsPage.AnalyticsEnabled();

            QuicksearchCategories = SettingsPage.QuicksearchCategories();
            Presets = new List<string>()
            {
                "BYO",
                "Systems",
                "GS",
                "CE"
            };

            var all = SettingsPage.PresetBYO().ToList();
            all.AddRange(SettingsPage.PresetSystems());
            all.AddRange(SettingsPage.PresetCE());
            all.AddRange(SettingsPage.PresetGS());

            AllCategories = all.GroupBy(c => c.Type).Select(group => group.First()).ToList();
            AllCategoryNames = AllCategories.Select(c => c.Name).ToList();

            EnhancedSearch = SettingsPage.UseEnhancedSearch();

            //SelectedNewItem = null;
        }

        private async Task DoSave(object obj)
        {
            SettingsPage.SalesID(SalesID);
            SettingsPage.TaxRate(TaxRate);

            var storeId = Stores[SelectedStoreName];
            SettingsPage.StoreID(storeId);
            SettingsPage.IncludeCSVWithQuote(IncludeCSVWithQuote);

            if (Theme != Application.Current.UserAppTheme)
            {
                Application.Current.UserAppTheme = Theme;
            }

            SettingsPage.Theme(Theme);
            SettingsPage.Vibrate(Vibrate);
            SettingsPage.AnalyticsEnabled(AnalyticsEnabled);

            SettingsPage.QuicksearchCategories(QuicksearchCategories);

            SettingsPage.UseEnhancedSearch(EnhancedSearch);

            MessagingCenter.Send(this, SETTINGS_UPDATED_MESSAGE);
            await ExitSettings();
        }

        private async Task ExitSettings()
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.Navigation.PopAsync();
            });
        }
    }
}