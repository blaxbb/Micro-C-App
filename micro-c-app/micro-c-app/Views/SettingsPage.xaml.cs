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
        public const string PREF_VERSION_PROMPT = "version_prompt";

        public const int CURRENT_VERSION_PROMPT = 1;
        public SettingsPage()
        {
            InitializeComponent();
        }

        public static string StoreID() => Preferences.Get(PREF_SELECTED_STORE, "141");
        public static string SalesID() => Preferences.Get(PREF_SALES_ID, "SALESID");
        public static float TaxRate() => Preferences.Get(PREF_TAX_RATE, 7.5f);
        public static float TaxRateFactor() => (TaxRate() * .01f) + 1;
        public static bool IncludeCSVWithQuote() => Preferences.Get(PREF_CSV_QUOTE, true);
        public static OSAppTheme Theme() => (OSAppTheme)Preferences.Get(PREF_THEME, 0);
        public static int VersionPrompt() => Preferences.Get(PREF_VERSION_PROMPT, 0);



        public static void StoreID(string id) => Preferences.Set(PREF_SELECTED_STORE, id);
        public static void SalesID(string id) => Preferences.Set(PREF_SALES_ID, id);
        public static void TaxRate(float rate) => Preferences.Set(PREF_TAX_RATE, rate);
        public static void IncludeCSVWithQuote(bool include) => Preferences.Set(PREF_CSV_QUOTE, include);

        public static void Theme(OSAppTheme theme) => Preferences.Set(PREF_THEME, (int)theme);
        public static void VersionPrompt(int value) => Preferences.Set(PREF_VERSION_PROMPT, value);
    }
}