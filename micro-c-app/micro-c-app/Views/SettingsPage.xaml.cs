using System.Runtime.CompilerServices;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

        public const string LOCATOR_BASE_URL = "https://locator.bbarrettnas.duckdns.org/";

        public const string SETTINGS_UPDATED_MESSAGE = "updated";

        public const int CURRENT_VERSION_PROMPT = 1;
        public SettingsPage()
        {
            InitializeComponent();
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

        private static void SendSettingsUpdated()
        {
            MessagingCenter.Send<SettingsPage>(new SettingsPage(), SETTINGS_UPDATED_MESSAGE);
        }
    }
}