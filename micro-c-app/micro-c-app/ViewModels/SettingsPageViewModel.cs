using micro_c_app.Views;
using System.Collections.Generic;
using System.Linq;
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

        public ICommand Save { get; }
        public ICommand Cancel { get; }

        public const string SETTINGS_UPDATED_MESSAGE = "updated";
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

            Stores = new Dictionary<string, string>()
            {
                {"CA - Tustin", "101"},
                {"CO - Denver", "181"},
                {"GA - Duluth", "065"},
                {"GA - Marietta", "041"},
                {"IL - Chicago", "151"},
                {"IL - Westmont", "025"},
                {"KS - Overland Park", "191"},
                {"MA - Cambridge", "121"},
                {"MD - Rockville", "085"},
                {"MD - Parkville", "125"},
                {"MI - Madison Heights", "055"},
                {"MN - St. Louis Park", "045"},
                {"MO - Brentwood", "095"},
                {"NJ - North Jersey", "075"},
                {"NY - Westbury", "171"},
                {"NY - Brooklyn", "115"},
                {"NY - Flushing", "145"},
                {"NY - Yonkers", "105"},
                {"OH - Columbus", "141"},
                {"OH  - Mayfield Heights", "051"},
                {"OH - Sharonville", "071"},
                {"PA - St. Davids", "061"},
                {"TX - Houston", "155"},
                {"TX - Dallas", "131"},
                {"VA - Fairfax", "081"},
                {"Micro Center Web Store", "029"}
            };

            StoreNames = Stores.Keys.ToList();
            SelectedStoreName = Stores.FirstOrDefault(kvp => kvp.Value == StoreID).Key;
        }

        private async Task DoSave(object obj)
        {
            SettingsPage.SalesID(SalesID);
            SettingsPage.TaxRate(TaxRate);

            var storeId = Stores[SelectedStoreName];
            SettingsPage.StoreID(storeId);
            SettingsPage.IncludeCSVWithQuote(IncludeCSVWithQuote);

            if(Theme != SettingsPage.Theme())
            {
                Application.Current.UserAppTheme = Theme;
            }

            SettingsPage.Theme(Theme);

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