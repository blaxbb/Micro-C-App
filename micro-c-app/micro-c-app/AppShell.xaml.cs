using micro_c_app.Views;
using System;
using System.Linq;
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

            if(SettingsPage.VersionPrompt() < SettingsPage.CURRENT_VERSION_PROMPT)
            {
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    var goToSettings = await DisplayAlert("Check settings", "You should set your store, sales ID, and tax rate before using the app.", "Ok", "Cancel");
                    SettingsPage.VersionPrompt(SettingsPage.CURRENT_VERSION_PROMPT);
                    if (goToSettings)
                    {
                        await Navigation.PushAsync(new SettingsPage());
                    }
                });
            }
        }

        private void DisplaySettings(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                FlyoutIsPresented = false;
                await Navigation.PushAsync(new SettingsPage());
            });
        }

        protected override bool OnBackButtonPressed()
        {
            var current = Navigation.NavigationStack.Last();
            var currentPage = (Shell.Current?.CurrentItem?.CurrentItem as IShellSectionController)?.PresentedPage;
            if (currentPage.BindingContext is ViewModels.ReferencePageViewModel vm)
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
