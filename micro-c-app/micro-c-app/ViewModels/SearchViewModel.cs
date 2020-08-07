using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;

namespace micro_c_app.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        public List<string> StoreNames { get; set; }
        public string SelectedStoreName { get; set; }

        public static Dictionary<string, string> Stores { get; private set; }
        public const string PREF_SELECTED_STORE = "selected_store";
        public SearchViewModel()
        {
            Title = "Search";
            var storeId = Preferences.Get(PREF_SELECTED_STORE, "141");

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

            SelectedStoreName = Stores.FirstOrDefault(kvp => kvp.Value == storeId).Key;
        }
    }
}