using System;
using System.Collections.Generic;
using micro_c_app.ViewModels;
using micro_c_app.Views;
using Xamarin.Forms;

namespace micro_c_app
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(SearchPage), typeof(SearchPage));
            Routing.RegisterRoute(nameof(BuildPage), typeof(BuildPage));
            Routing.RegisterRoute(nameof(QuotePage), typeof(QuotePage));
        }
    }
}
