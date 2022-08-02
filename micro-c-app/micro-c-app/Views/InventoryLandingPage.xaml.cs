using micro_c_app.ViewModels.InventoryAudit;
using micro_c_app.Views.InventoryAudit;
using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class InventoryLandingPage : ContentPage
    {
        public List<LandingItem> Items { get; set; }

        public ICommand Clicked { get; }
        public InventoryLandingPage()
        {
            BindingContext = this;
            Items = new List<LandingItem>()
            {
                new LandingItem("Zero Stock", "Products which have been scanned into inventory, but are listed as zero stock on MicroCenter.com. Consider checking for inventory discrepencies.", "\uf059"),
                new LandingItem("Unknown Location", "Items which have not been scanned into inventory, but are listed as in stock on MicroCenter.com.  Consider checking for product in other areas.", "\uf14e"),
                new LandingItem("Consolidation", "Items which have been scanned in multiple areas.  Consider consolidating them into fewer areas if stock warrants.", "\uf0e8"),
                new LandingItem("Freshness", "The previous time that each section of a given category has been scanned.  Consider re-auditing sections when they are too stale.", "\uf5d0"),
                new LandingItem("Compliance", "Items which conflict with compliance rules for a specific location.", "\uf3ed"),
                new LandingItem("Clearance", "Clearance Id's which exist, but have not been scanned in a section.", "\uf49e")
            };

            Clicked = new Command<string>(OnClicked);

            InitializeComponent();
        }

        private void OnClicked(string item)
        {
            ContentPage? page = null;
            switch (item)
            {
                case "Scan":
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
                        if (!allowed)
                        {
                            return;
                        }

                        await Navigation.PushAsync(new InventoryView());
                    });
                    break;
                case "Zero Stock":
                    page = new ZeroStockPage();
                    break;
                case "Unknown Location":
                    page = new UnknownLocationPage();
                    break;
                case "Consolidation":
                    page = new ConsolidationPage();
                    break;
                case "Freshness":
                    page = new FreshnessPage();
                    break;
                case "Compliance":
                    page = new CompliancePage();
                    break;
                case "Clearance":
                    page = new MissingClearance();
                    break;
            }
            if (page != null)
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    await Navigation.PushAsync(page);
                });
            }
        }

        public static void CheckNotifications()
        {
            var lastNotification = SettingsPage.LastInventoryNotification();
            if (DateTime.Now - lastNotification < TimeSpan.FromDays(4))
            {
                return;
            }

            SettingsPage.LastInventoryNotification(DateTime.Now);

            if(SettingsPage.StoreID() != "141")
            {
                //testing only in 141 for now...
                return;
            }

            Task.Run(async () =>
            {
                var notificationManager = DependencyService.Get<INotificationManager>();
                //foreach (var type in new BuildComponent.ComponentType[] {BuildComponent.ComponentType.Case, BuildComponent.ComponentType.Motherboard})
                foreach (var type in SettingsPage.InventoryFavorites())
                {
                    var unknowns = await BaseInventoryViewModel.Get<List<Item>>(type, UnknownLocationPageViewModel.Method);
                    var compliance = await BaseInventoryViewModel.Get<List<ComplianceReport>>(type, CompliancePageViewModel.Method);
                    var freshness = await BaseInventoryViewModel.Get<List<InventoryLocation>>(type, FreshnessPageViewModel.Method);
                    var clearance = await BaseInventoryViewModel.Get<List<MissingClearanceInfo>>(type, MissingClearanceViewModel.Method);

                    StringBuilder sb = new StringBuilder();

                    if (unknowns.Count > 0)
                    {
                        sb.AppendLine($"{unknowns.Count} items with unknown locations.");
                        //notificationManager.ScheduleNotification($"Unknown Location", $"{unknowns.Count} items with unknown locations.");
                    }

                    if(compliance.Count > 0)
                    {
                        //notificationManager.ScheduleNotification($"Compliance", $"{compliance.Count} locations with compliance issues.");
                        sb.AppendLine($"{compliance.Count} locations with compliance issues.");
                    }

                    var freshCutoff = DateTime.Now - TimeSpan.FromDays(7);
                    var staleCutoff = DateTime.Now - TimeSpan.FromDays(14);

                    var expiredCount = freshness?.Count(l => l.LastFullScan < staleCutoff) ?? 0;
                    var staleCount = freshness?.Count(l => l.LastFullScan < freshCutoff) ?? 0 - expiredCount;

                    if(expiredCount > 0 || staleCount > 0)
                    {
                        sb.AppendLine($"{expiredCount} locations are expired, and {staleCount} are stale.");
                    }

                    if(clearance.Count > 0)
                    {
                        sb.AppendLine($"{clearance.Sum(c => c?.MissingClearance.Count ?? 0)} clearance tags are missing.");
                    }

                    if (sb.Length > 0)
                    {
                        notificationManager.ScheduleNotification($"Micro C - {type}", sb.ToString(), ("inventory", type.ToString()));
                    }
                }
            });
        }
    }

    public class LandingItem
    {
        public string Name { get; }
        public string Description { get; }
        public string Icon { get; }

        public LandingItem(string name, string description, string icon)
        {
            Name = name;
            Description = description;
            Icon = icon;
        }
    }
}