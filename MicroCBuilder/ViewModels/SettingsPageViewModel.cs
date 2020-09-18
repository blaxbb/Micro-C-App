using micro_c_lib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MicroCBuilder.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        Dictionary<string, string> AllStores => Stores.AllStores;
        public List<string> StoreNames => AllStores.Keys.ToList();
        public string SelectedStore { get => selectedStore; set { SetProperty(ref selectedStore, value); Settings.Store(value); ForceUpdateCommand?.Execute(null); } }
        public double TaxRate { get => taxRate; set { SetProperty(ref taxRate, value); Settings.TaxRate(value); } }
        public TimeSpan LastUpdated { get => lastUpdated; set => SetProperty(ref lastUpdated, value); }
        public ICommand ForceUpdateCommand { get; }

        private string selectedStore;
        private double taxRate;
        private TimeSpan lastUpdated;

        public delegate void ForceUpdateEvent();
        public static event ForceUpdateEvent ForceUpdate;

        public SettingsPageViewModel()
        {
            SelectedStore = Settings.Store();
            TaxRate = Settings.TaxRate();
            var updateTime = Settings.LastUpdated();
            LastUpdated = DateTime.Now - updateTime;

            ForceUpdateCommand = new Command((_) =>
            {
                ForceUpdate?.Invoke();
                Debug.WriteLine("UPDATE");
            });
        }
    }
}
