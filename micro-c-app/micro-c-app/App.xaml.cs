using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using micro_c_app.Views;

namespace micro_c_app
{
    public partial class App : Application
    {

        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
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
