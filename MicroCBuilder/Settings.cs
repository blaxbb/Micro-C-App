using micro_c_lib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using static MicroCLib.Models.BuildComponent;

namespace MicroCBuilder
{
    public static class Settings
    {
        public const string STORE_KEY = "settings";
        public const string TAXRATE_KEY = "taxrate";
        public const string LASTUPDATE_KEY = "lastupdate";
        public const string CATEGORIES_KEY = "categories";
        static ApplicationDataContainer localSettings => Windows.Storage.ApplicationData.Current.LocalSettings;
        static bool Exists(string key) => localSettings.Values.ContainsKey(key);
        public static T Value<T>(string key) => (T)localSettings.Values[key];
        static void Set(string key, object value)
        {
            localSettings.Values[key] = value;
            SettingsUpdated?.Invoke(key, value);
        }

        public delegate void SettingsUpdatedEvent(string key, object value);
        public static event SettingsUpdatedEvent SettingsUpdated;

        public static string Store()  => Exists(STORE_KEY) ? Value<string>(STORE_KEY) : Stores.AllStores.Keys.FirstOrDefault();
        public static string StoreID() => Stores.AllStores[Store()];
        public static double TaxRate() => Exists(TAXRATE_KEY) ? Value<double>(TAXRATE_KEY) : 7.5d;
        public static DateTimeOffset LastUpdated() => Exists(LASTUPDATE_KEY) ? Value<DateTimeOffset>(LASTUPDATE_KEY) : DateTimeOffset.Now;
        public static List<ComponentType> Categories() => Exists(CATEGORIES_KEY) ? JsonConvert.DeserializeObject<List<ComponentType>>(Value<string>(CATEGORIES_KEY)) : new List<ComponentType>();


        public static void Store(string store) => Set(STORE_KEY, store);
        public static void TaxRate(double tax) => Set(TAXRATE_KEY, tax);
        public static void LastUpdated(DateTimeOffset time) => Set(LASTUPDATE_KEY, time);
        public static void Categories(List<ComponentType> components) => Set(CATEGORIES_KEY, JsonConvert.SerializeObject(components));
    }
}
