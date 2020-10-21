using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
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
    public sealed partial class MainPage : Page, INotifyPropertyChanged
    {
        private string progressTitleText;
        private string progressElapsedText;
        private double progressElapsedValue;
        private Visibility progressVisibility;

        public static MainPage Instance { get; private set; }
        public string ProgressTitleText { get => progressTitleText; set => SetProperty(ref progressTitleText, value); }
        public string ProgressElapsedText { get => progressElapsedText; set => SetProperty(ref progressElapsedText, value); }
        public double ProgressElapsedValue { get => progressElapsedValue; set => SetProperty(ref progressElapsedValue, value); }
        public Visibility ProgressVisibility { get => progressVisibility; set => SetProperty(ref progressVisibility, value); }

        public MainPage()
        {
            Instance = this;
            this.InitializeComponent();
            this.DataContext = this;
            ProgressVisibility = Visibility.Collapsed;
            InitCache();
            MicroCBuilder.ViewModels.SettingsPageViewModel.ForceUpdate += async () => await UpdateCache();
            MicroCBuilder.ViewModels.SettingsPageViewModel.ForceDeepUpdate += async () => await DeepUpdateCache();


            Navigation.SelectedItem = Navigation.MenuItems.FirstOrDefault();
            Navigate(Navigation.SelectedItem as NavigationViewItemBase);
            //ContentFrame.Navigate(typeof(TestPage));
        }

        public async Task DisplayProgress(Func<IProgress<int>, Task> action, string title, int itemCount)
        {
            //ProgressTitle.Text = title;
            ProgressTitleText = title;
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


                ProgressElapsedValue = percent;
                ProgressElapsedText = $"{i} / {itemCount} {average.Average:hh\\:mm\\:ss} remaining";
                sw.Restart();
            });

            ProgressVisibility = Visibility.Visible;
            await action(progress)
                .ContinueWith(async (_) =>
            {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ProgressVisibility = Visibility.Collapsed;
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
            var categoryStrings = Settings.Categories().Select(c => BuildComponent.CategoryFilterForType(c)).ToList();
            await DisplayProgress(async (progress) =>
            {
                await BuildComponentCache.Current.DeepPopulateCache(progress);
            }, "Updating cache", BuildComponentCache.Current?.Cache.Where(kvp => categoryStrings.Contains(kvp.Key)).Sum(kvp => kvp.Value.Count) ?? 100);
        }

        public async void InitCache()
        {
            var progress = new Progress<int>((int p) =>
            {
                ProgressElapsedValue = p;
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
                ProgressVisibility = Visibility.Collapsed;
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
        protected bool SetProperty<T>(ref T backingStore, T value, [CallerMemberName] string propertyName = "", Action? onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
