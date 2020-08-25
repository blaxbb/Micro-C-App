using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Xamarin.Forms;
using Xamarin.Forms.Markup;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace microcapp_uwp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage
    {
        public MainPage()
        {
            this.InitializeComponent();
            
            SizeChanged += MainPage_SizeChanged;
        }

        protected override void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            var app = e.Parameter as Xamarin.Forms.Application;
            LoadApplication(app);
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility =
                Windows.UI.Core.AppViewBackButtonVisibility.Visible;
            Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;
            ApplicationView appView = ApplicationView.GetForCurrentView();
            string title = app.MainPage.Title;
            if (string.IsNullOrEmpty(title))
            {
                title = app.GetType().Namespace;
            }
            appView.Title = title;
            base.OnNavigatedTo(e);
        }

        private void MainPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var w = e.NewSize.Width;
            Shell.Current.Layout(new Rectangle(0,0, e.NewSize.Width, e.NewSize.Height));
        }

        private void MainPage_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            if (!e.Handled && Frame.CanGoBack)
            {
                e.Handled = true;
                Frame.GoBack();
            }
        }
    }
}
