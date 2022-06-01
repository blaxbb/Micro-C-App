using micro_c_app.Models;
using micro_c_app.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MicroCLib.Models.BuildComponent;
using static MicroCLib.Models.BuildComponent.ComponentType;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SettingsPage : ContentPage
    {
        public const string PREF_SELECTED_STORE = "selected_store";
        public const string PREF_SALES_ID = "sales_id";
        public const string PREF_TAX_RATE = "tax_rate";
        public const string PREF_CSV_QUOTE = "csv_with_quote";
        public const string PREF_THEME = "theme";
        public const string PREF_VIBRATE = "vibarte";
        public const string PREF_VERSION_PROMPT = "version_prompt";
        public const string PREF_LOCATOR_COOKIE = "locator_cookie";

        public const string PREF_ACKNOWLEDGED_ANALYTICS = "acknowledged_analytics";
        public const string PREF_ANALYTICS_ENABLED = "analytics_enabled";

        public const string PREF_QUICKSEARCH_CATEGORIES = "quicksearch_categories";

        public const string PREF_HELPMESSAGE_INDEX = "helpmessage_index";

        public const string PREF_ENHANCED_SEARCH = "enhanced_search";

        public const string PREF_INVENTORY_CATEGORIES = "inventory_categories";

        public const string LOCATOR_BASE_URL = "https://locator.bbarrettnas.duckdns.org/";

        public const string SETTINGS_UPDATED_MESSAGE = "updated";

        public const string INVENTORY_LAST_NOTIFICATION = "inventory_notification_time";

        public const int CURRENT_VERSION_PROMPT = 1;
        public SettingsPage()
        {
            InitializeComponent();
            SetupQuicksearch();

            addCategoryPicker.SelectedIndex = -1;
            addCategoryPicker.SelectedIndexChanged += AddCategoryPicker_SelectedIndexChanged;
            if(BindingContext is SettingsPageViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        private void AddCategoryPicker_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            if(addCategoryPicker.SelectedIndex != -1)
            {
                addCategoryPicker.SelectedIndex = -1;
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(SettingsPageViewModel.QuicksearchCategories))
            {
                SetupQuicksearch();
            }
        }

        private void SetupQuicksearch()
        {
            if (BindingContext is SettingsPageViewModel vm)
            {
                quicksearchCategoriesStack.Children.Clear();
                
                foreach (var cat in vm.QuicksearchCategories)
                {
                    var grid = new Grid();
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(5, GridUnitType.Star) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });

                    var label = new Label()
                    {
                        Text = cat.Name,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    var removeButton = new Button()
                    {
                        Text = "X",
                        BackgroundColor = Color.Red
                    };
                    var border = new Frame()
                    {
                        Content = grid,
                        BorderColor = Color.DarkGray,
                        Padding = 10,
                        Margin = 0
                    };
                    border.Content = grid;
                    removeButton.Clicked += (sender, args) =>
                    {
                        vm.QuicksearchCategories.Remove(cat);
                        SetupQuicksearch();
                    };

                    grid.Children.Add(label);
                    grid.Children.Add(removeButton);
                    Grid.SetColumn(label, 0);
                    Grid.SetColumn(removeButton, 1);

                    quicksearchCategoriesStack.Children.Add(border);
                }
                quicksearchCategoriesStack.ResolveLayoutChanges();
            }
        }

        public static string StoreID() => Preferences.Get(PREF_SELECTED_STORE, "141");
        public static string SalesID() => Preferences.Get(PREF_SALES_ID, "");
        public static float TaxRate() => Preferences.Get(PREF_TAX_RATE, 7.5f);
        public static float TaxRateFactor() => (TaxRate() * .01f) + 1;
        public static bool IncludeCSVWithQuote() => Preferences.Get(PREF_CSV_QUOTE, true);
        public static OSAppTheme Theme() => (OSAppTheme)Preferences.Get(PREF_THEME, 0);
        public static bool Vibrate() => Preferences.Get(PREF_VIBRATE, true);
        public static string LocatorCookie() => Preferences.Get(PREF_LOCATOR_COOKIE, "");
        public static int VersionPrompt() => Preferences.Get(PREF_VERSION_PROMPT, 0);
        public static bool AcknowledgedAnalytics() => Preferences.Get(PREF_ACKNOWLEDGED_ANALYTICS, false);
        public static bool AnalyticsEnabled() => Preferences.Get(PREF_ANALYTICS_ENABLED, false);
        public static List<ComponentTypeInfo> QuicksearchCategories()
        {
            var json = Preferences.Get(PREF_QUICKSEARCH_CATEGORIES, null);
            if(json == null)
            {
                return PresetBYO().ToList();
            }

            return Newtonsoft.Json.JsonConvert.DeserializeObject<List<ComponentTypeInfo>>(json);
        }
        public static int HelpMessageIndex() => Preferences.Get(PREF_HELPMESSAGE_INDEX, 0);
        public static bool UseEnhancedSearch() => Preferences.Get(PREF_ENHANCED_SEARCH, false);
        public static List<ComponentType> InventoryFavorites()
        {
            var json = Preferences.Get(PREF_INVENTORY_CATEGORIES, null);
            if(json == null)
            {
                return new List<ComponentType>();
            }

            return JsonConvert.DeserializeObject<List<ComponentType>>(json) ?? new List<ComponentType>();
        }

        public static DateTime LastInventoryNotification() => Preferences.Get(INVENTORY_LAST_NOTIFICATION, DateTime.MinValue);


        public static void StoreID(string id) {Preferences.Set(PREF_SELECTED_STORE, id); SendSettingsUpdated();}
        public static void SalesID(string id) {Preferences.Set(PREF_SALES_ID, id); SendSettingsUpdated();}
        public static void TaxRate(float rate) {Preferences.Set(PREF_TAX_RATE, rate); SendSettingsUpdated();}
        public static void IncludeCSVWithQuote(bool include) {Preferences.Set(PREF_CSV_QUOTE, include); SendSettingsUpdated();}

        public static void Theme(OSAppTheme theme) {Preferences.Set(PREF_THEME, (int)theme); SendSettingsUpdated();}
        public static void Vibrate(bool vibrate) { Preferences.Set(PREF_VIBRATE, vibrate); SendSettingsUpdated();}
        public static void LocatorCookie(string cookie)  {Preferences.Set(PREF_LOCATOR_COOKIE, cookie); SendSettingsUpdated();}
        public static void VersionPrompt(int value) {Preferences.Set(PREF_VERSION_PROMPT, value); SendSettingsUpdated();}
        public static void AcknowledgedAnalytics(bool value)  {Preferences.Set(PREF_ACKNOWLEDGED_ANALYTICS, value); SendSettingsUpdated();}
        public static void AnalyticsEnabled(bool value) { Preferences.Set(PREF_ANALYTICS_ENABLED, value); SendSettingsUpdated();}
        public static void QuicksearchCategories(List<ComponentTypeInfo> categories)
        {
            var json = Newtonsoft.Json.JsonConvert.SerializeObject(categories);
            Preferences.Set(PREF_QUICKSEARCH_CATEGORIES, json);
            SendSettingsUpdated();
        }
        public static void HelpMessageIndex(int index) { Preferences.Set(PREF_HELPMESSAGE_INDEX, index); SendSettingsUpdated(); }
        public static void IncrementHelpMessageIndex()
        {
            var index = HelpMessageIndex();
            HelpMessageIndex(index + 1);
        }
        public static void UseEnhancedSearch(bool value) { Preferences.Set(PREF_ENHANCED_SEARCH, value); SendSettingsUpdated(); }
        public static void InventoryFavorites(List<ComponentType> favorites)
        {
            var json = JsonConvert.SerializeObject(favorites);
            Preferences.Set(PREF_INVENTORY_CATEGORIES, json);
            SendSettingsUpdated();
        }

        public static void LastInventoryNotification(DateTime time)
        {
            Preferences.Set(INVENTORY_LAST_NOTIFICATION, time);
            SendSettingsUpdated();
        }


        private static void SendSettingsUpdated()
        {
            MessagingCenter.Send<SettingsPage>(new SettingsPage(), SETTINGS_UPDATED_MESSAGE);
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