using micro_c_app.Models;
using micro_c_app.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

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

        public ICommand Save { get; }
        public SettingsPageViewModel()
        {
            Save = new Command(DoSave);

            Title = "Settings";
            StoreID = Preferences.Get(SettingsPage.PREF_SELECTED_STORE, "141");
            SalesID = Preferences.Get(SettingsPage.PREF_SALES_ID, "SALESID");
            TaxRate = Preferences.Get(SettingsPage.PREF_TAX_RATE, 7.5f);

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

        private void DoSave(object obj)
        {
            Preferences.Set(SettingsPage.PREF_SALES_ID, SalesID);
            Preferences.Set(SettingsPage.PREF_TAX_RATE, TaxRate);

            var storeId = Stores[SelectedStoreName];
            Preferences.Set(SettingsPage.PREF_SELECTED_STORE, storeId);
        }
    }
}