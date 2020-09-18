using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace MicroCBuilder.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(1920, 1080);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            InitCache();
            MicroCBuilder.ViewModels.SettingsPageViewModel.ForceUpdate += async () => UpdateCache();


            Navigation.SelectedItem = Navigation.MenuItems.FirstOrDefault();
            Navigate(Navigation.SelectedItem as NavigationViewItemBase);
            //ContentFrame.Navigate(typeof(TestPage));
        }

        public void UpdateCache()
        {
            var progress = new Progress<int>((int p) =>
            {
                CacheProgress.Value = p;
            });
            var dispatcher = Window.Current.Dispatcher;

            CacheProgressContainer.Visibility = Visibility.Visible;

            Task.Run(async () =>
            {
                await BuildComponentCache.Current.PopulateCache(progress);
            }).ContinueWith(async (_) =>
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    CacheProgressContainer.Visibility = Visibility.Collapsed;
                });
            });
        }

        public void InitCache()
        {
            var progress = new Progress<int>((int p) =>
            {
                CacheProgress.Value = p;
            });


            var dispatcher = Window.Current.Dispatcher;
            var cache = new BuildComponentCache();
            if (cache.Cache.Count == 0)
            {
                Task.Run(async () =>
                {
                    var cached = await cache.LoadCache();
                    if (!cached)
                    {
                        await cache.PopulateCache(progress);
                    }
                }).ContinueWith(async (_) =>
                {
                    await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                    {
                        CacheProgressContainer.Visibility = Visibility.Collapsed;
                    });
                });
            }
            else
            {
                CacheProgressContainer.Visibility = Visibility.Collapsed;
            }
        }

        private void NavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                Navigation.PaneTitle = "Settings";
                ContentFrame.Navigate(typeof(SettingsPage), null, new EntranceNavigationTransitionInfo());
                return;
            }

            Navigate(args.InvokedItemContainer);
        }

        void Navigate(NavigationViewItemBase item)
        {
            Type? pageType;
            switch (item?.Tag?.ToString())
            {
                case "BuildPage":
                    pageType = typeof(BuildPage);
                    break;
                default:
                    pageType = null;
                    break;
            }

            if (pageType != null)
            {
                ContentFrame.Navigate(pageType);
            }
        }

        private void NavigationView_BackRequested(NavigationView sender, NavigationViewBackRequestedEventArgs args)
        {
            ContentFrame.GoBack();
        }
    }
}
