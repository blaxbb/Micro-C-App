﻿using micro_c_app.Models;
using micro_c_app.Views;
using Microsoft.AppCenter.Analytics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace micro_c_app
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        INotificationManager notificationManager;
        public AppShell()
        {
            InitializeComponent();

            notificationManager = DependencyService.Get<INotificationManager>();

            //Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
            //Routing.RegisterRoute(nameof(BuildPage), typeof(BuildPage));
            //Routing.RegisterRoute(nameof(QuotePage), typeof(QuotePage));

            Routing.RegisterRoute("inventory", typeof(InventoryLandingPage));

            RestoreState.Load();
            if(!SettingsPage.AcknowledgedAnalytics())
            {
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    var enableAnalytics = await DisplayAlert("Analytics", "This app uses anonymous analytics to track the most used features, as well as provide detailed crash reports to help improve the reliablity of the application.  Would you like to enable or disable them?", "Enable", "Disable");
                    SettingsPage.AcknowledgedAnalytics(true);
                    SettingsPage.AnalyticsEnabled(enableAnalytics);
                    if (SettingsPage.VersionPrompt() < SettingsPage.CURRENT_VERSION_PROMPT)
                    {
                        var goToSettings = await DisplayAlert("Check settings", "You should set your store and tax rate before using the app.", "Ok", "Cancel");
                        SettingsPage.VersionPrompt(SettingsPage.CURRENT_VERSION_PROMPT);
                        if (goToSettings)
                        {
                            await Navigation.PushAsync(new SettingsPage());
                        }
                    }
                });
            }
            else if(SettingsPage.VersionPrompt() < SettingsPage.CURRENT_VERSION_PROMPT)
            {
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    var goToSettings = await DisplayAlert("Check settings", "You should set your store and tax rate before using the app.", "Ok", "Cancel");
                    SettingsPage.VersionPrompt(SettingsPage.CURRENT_VERSION_PROMPT);
                    if (goToSettings)
                    {
                        await Navigation.PushAsync(new SettingsPage());
                    }
                });
            }

            Reminder.CheckReminders(notificationManager);
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);
            if (SettingsPage.AnalyticsEnabled())
            {
                AnalyticsService.Track("Navigate", args.Target.Location.ToString());
            }
        }

        private void RealtimeScan(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                bool allowed = await BarcodeScanner.Mobile.Methods.AskForRequiredPermission();
                if (!allowed)
                {
                    return;
                }

                FlyoutIsPresented = false;
                await Navigation.PushAsync(new RealtimeScan());
            });
        }

        private void InventoryScan(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                FlyoutIsPresented = false;
                await Navigation.PushAsync(new InventoryLandingPage());
            });
        }

        private void DisplaySettings(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                FlyoutIsPresented = false;
                await Navigation.PushAsync(new SettingsPage());
            });
        }

        private void DisplayReminders(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                FlyoutIsPresented = false;
                await Navigation.PushAsync(new RemindersPage());
            });
        }

        protected override bool OnBackButtonPressed()
        {
            var currentPage = (Shell.Current?.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage;
            if (currentPage?.BindingContext is ViewModels.ReferencePageViewModel vm)
            {
                return vm.BackButton();
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }
    }
}
