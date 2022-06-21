using micro_c_app.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class InventoryReviewViewModel : BaseViewModel
    {
        private Dictionary<string, List<string>>? scans;

        public Dictionary<string, List<string>>? Scans { get => scans; set => SetProperty(ref scans, value); }

        public ICommand Reset { get; }
        public ICommand Submit { get; }
        public ICommand Replace { get; }

        public const string LOCATION_TRACKER_BASEURL = "https://location.bbarrett.me";

        public event EventHandler<Dictionary<string, List<string>>> ScansUpdated;

        public InventoryReviewViewModel()
        {
            Reset = new Command(async () =>
            {
                var confirm = await Shell.Current.DisplayAlert("Confirm", "This will remove all items you have scanned!", "Confirm", "Cancel");
                if (confirm)
                {
                    Scans?.Clear();
                    RestoreState.Instance.InventoryScans = Scans;
                    RestoreState.Save();

                    ScansUpdated?.Invoke(this, Scans);
                    await Shell.Current.Navigation.PopAsync();
                }
            });

            Submit = new Command(async () =>
            {
                await DoSubmit("add");
                await Shell.Current.Navigation.PopAsync();
            });

            Replace = new Command(async () =>
            {
                var confirm = await Shell.Current.DisplayAlert("Confirm", "This is a complete audit and will remove all items previously submitted to the locations scanned.  This is irreversible!", "Confirm", "Cancel");
                if (confirm)
                {
                    await DoSubmit("replace");
                    await Shell.Current.Navigation.PopAsync();
                }
            });
        }

        public void RemoveEntry(string location, string entry)
        {
            if (Scans?.ContainsKey(location) ?? false)
            {
                Scans[location].Remove(entry);

                var dupe = Scans;
                Scans = null;
                Scans = dupe;

                RestoreState.Instance.InventoryScans = Scans;
                RestoreState.Save();

                ScansUpdated?.Invoke(this, Scans);
            }
        }

        public void RemoveLocation(string location)
        {
            if (Scans?.ContainsKey(location) ?? false)
            {
                Scans.Remove(location);

                var dupe = Scans;
                Scans = null;
                Scans = dupe;

                RestoreState.Instance.InventoryScans = Scans;
                RestoreState.Save();

                ScansUpdated?.Invoke(this, Scans);
            }
        }

        async Task<bool> DoSubmit(string method)
        {
            if (Scans == null || Scans.Count == 0 || !Scans.Values.Any(l => l.Count > 0))
            {
                return true;
            }

            bool error = false;
            foreach (var kvp in Scans)
            {
                var result = await DoSubmit(kvp.Key, kvp.Value, method);
                if (!result)
                {
                    error = true;
                }
            }

            if (!error)
            {
                Scans?.Clear();
                RestoreState.Instance.InventoryScans = Scans ?? new Dictionary<string, List<string>>();
                RestoreState.Save();

                ScansUpdated?.Invoke(this, Scans);
            }

            return !error;
        }

        public static async Task<bool> DoSubmit(string location, List<string> skus, string method = "add")
        {
            try
            {
                using var client = new HttpClient();

                var json = JsonConvert.SerializeObject(skus);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{LOCATION_TRACKER_BASEURL}/api/Entries/bulk/{method}/{location}", content);
                if (response.IsSuccessStatusCode)
                {
                    return true;
                }

            }
            catch (Exception e)
            {
                AnalyticsService.TrackError(e, location, string.Join(", ", skus), method);
            }

            return false;
        }
    }
}
