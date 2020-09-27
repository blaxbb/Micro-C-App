using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        public static MainPage Instance { get; private set; }
        public MainPage()
        {
            Instance = this;
            this.InitializeComponent();
            ApplicationView.PreferredLaunchViewSize = new Size(1920, 1080);
            ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
            InitCache();
            MicroCBuilder.ViewModels.SettingsPageViewModel.ForceUpdate += async () => await UpdateCache();
            MicroCBuilder.ViewModels.SettingsPageViewModel.ForceDeepUpdate += async () => await DeepUpdateCache();



            Navigation.SelectedItem = Navigation.MenuItems.FirstOrDefault();
            Navigate(Navigation.SelectedItem as NavigationViewItemBase);
            //ContentFrame.Navigate(typeof(TestPage));
        }

        public async Task DisplayProgress(Func<IProgress<int>, Task> action, string title, int itemCount)
        {
            ProgressTitle.Text = title;
            var dispatcher = Window.Current.Dispatcher;

            var lastPercentCheck = 0f;
            TimeSpanRollingAverage average = new TimeSpanRollingAverage();
            Stopwatch sw = new Stopwatch();
            sw.Start();



            var progress = new Progress<int>((i) =>
            {
                var deltaTime = sw.Elapsed;
                var percent = 100 * i / (float)itemCount;
                var deltaPercent = percent - lastPercentCheck;
                var percentRemaining = 100 - percent;
                lastPercentCheck = percent;

                TimeSpan estimation;
                if (deltaPercent > 0)
                {
                    estimation = deltaTime * (percentRemaining / deltaPercent);
                }
                else
                {
                    estimation = TimeSpan.Zero;
                }
                average.Push(estimation);


                ProgressBar.Value = percent;
                ProgressElapsed.Text = $"{i} / {itemCount}  Estimated time remaining {average.Average:hh\\:mm\\:ss}";
                sw.Restart();
            });

            ProgressContainer.Visibility = Visibility.Visible;
            await action(progress)
                .ContinueWith(async (_) =>
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ProgressContainer.Visibility = Visibility.Collapsed;
                });
            });
        }

        public async Task UpdateCache()
        {
            await DisplayProgress(async (progress) =>
            {
                await BuildComponentCache.Current.PopulateCache(progress);
            }, "Updating cache", 100);
        }

        public async Task DeepUpdateCache()
        {
            await DisplayProgress(async (progress) =>
            {
                await BuildComponentCache.Current.DeepPopulateCache(progress);
            }, "Updating cache", BuildComponentCache.Current?.TotalItems ?? 100);
        }

        public async void InitCache()
        {
            var progress = new Progress<int>((int p) =>
            {
                ProgressBar.Value = p;
            });


            var dispatcher = Window.Current.Dispatcher;
            var cache = new BuildComponentCache();
            if (cache.Cache.Count == 0)
            {
                await DisplayProgress(async (progress) =>
                {
                    var ts = DateTime.Now - Settings.LastUpdated();
                    var cached = await cache.LoadCache();
                    if (!cached || ts.TotalHours >= 20)
                    {
                        await cache.PopulateCache(progress);
                    }
                }, "Updating cache", 100);
            }
            else
            {
                ProgressContainer.Visibility = Visibility.Collapsed;
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
            Type pageType = (item?.Tag?.ToString()) switch
            {
                "BuildPage" => typeof(BuildPageTabContainer),
                _ => null,
            };
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
