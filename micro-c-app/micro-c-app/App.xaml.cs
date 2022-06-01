using micro_c_app.Themes;
using micro_c_app.Views;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xamarin.Forms;

namespace micro_c_app
{
    public partial class App : Application
    {
        public static SearchCache SearchCache = new SearchCache(TimeSpan.FromHours(1));
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();

            if (Device.RuntimePlatform == Device.Android)
            {
                //IOS.Clear();
            }
            else if (Device.RuntimePlatform == Device.iOS)
            {
                //Android.Clear();
            }

            RequestedThemeChanged += App_RequestedThemeChanged;
            SwitchTheme(SettingsPage.Theme());

            MessagingCenter.Subscribe<SettingsPage>(this, SettingsPage.SETTINGS_UPDATED_MESSAGE, (_) => { SettingsUpdated(); });
        }

        private void SettingsUpdated()
        {
            AnalyticsService.Track($"Store {SettingsPage.StoreID()}");
        }

        private void App_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            SwitchTheme(e.RequestedTheme);
        }

        private static void SwitchTheme(OSAppTheme theme)
        {
            var existing = Current.Resources.MergedDictionaries.FirstOrDefault(r => r.Source?.ToString() == "Themes/DarkTheme.xaml" || r.Source?.ToString() == "Themes/LightTheme.xaml");
            Current.Resources.MergedDictionaries.Remove(existing);
            if (theme == OSAppTheme.Unspecified)
            {
                theme = Current.RequestedTheme;
            }
            Current.UserAppTheme = theme;
            switch (theme)
            {
                case OSAppTheme.Light:
                    Current.Resources.MergedDictionaries.Add(new LightTheme());
                    break;
                case OSAppTheme.Dark:
                    Current.Resources.MergedDictionaries.Add(new DarkTheme());
                    break;
            }
        }

        protected override void OnStart()
        {
            //These values aren't *really* secrets
            Microsoft.AppCenter.AppCenter.Start("android=e236360c-f1d0-4e7c-a4b2-a0edd6f40241;" +
                                                  "uwp={Your UWP App secret here};" +
                                                  "ios=2cf94c3f-2fe7-4c6f-8d44-17da1c3f957f",
                                                  typeof(Microsoft.AppCenter.Analytics.Analytics), typeof(Microsoft.AppCenter.Crashes.Crashes));

            AnalyticsService.Track("Store ID", SettingsPage.StoreID());

            InventoryLandingPage.CheckNotifications();
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
            InventoryLandingPage.CheckNotifications();
        }
    }
}
