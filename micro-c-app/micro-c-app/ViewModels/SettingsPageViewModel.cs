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

            Stores = micro_c_lib.Models.Stores.AllStores;

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