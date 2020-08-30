using micro_c_app.Themes;
using micro_c_app.Views;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;

namespace micro_c_app
{
    public partial class App : Application
    {

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
        }

        private void App_RequestedThemeChanged(object sender, AppThemeChangedEventArgs e)
        {
            SwitchTheme(e.RequestedTheme);
        }

        private static void SwitchTheme(OSAppTheme theme)
        {
            var existing = Current.Resources.MergedDictionaries.FirstOrDefault(r => r.Source?.ToString() == "Themes/DarkTheme.xaml" || r.Source?.ToString() == "Themes/LightTheme.xaml");
            Current.Resources.MergedDictionaries.Remove(existing);
            if(theme == OSAppTheme.Unspecified)
            {
                theme = Current.RequestedTheme;
            }

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
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
