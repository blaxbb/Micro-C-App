using GoogleVisionBarCodeScanner;
using micro_c_app.Models;
using micro_c_app.ViewModels;
using MicroCLib.Models;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing;
using ZXing.Mobile;
using ZXing.Net.Mobile.Forms;
using static micro_c_lib.Models.Search;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SearchView : ContentView
    {
        HttpClient client;
        CancellationTokenSource tokenSource = new CancellationTokenSource();
        CancellationToken cancellationToken => tokenSource.Token;

        private bool busy;
        private ProgressInfo progress;
        public static readonly BindableProperty ProductFoundProperty = BindableProperty.Create(nameof(ProductFound), typeof(ICommand), typeof(SearchView), null);

        public static readonly BindableProperty ErrorProperty = BindableProperty.Create(nameof(Error), typeof(ICommand), typeof(SearchView), null);
        public ICommand ProductFound { get { return (ICommand)GetValue(ProductFoundProperty); } set { SetValue(ProductFoundProperty, value); } }
        public ICommand Error { get { return (ICommand)GetValue(ErrorProperty); } set { SetValue(ErrorProperty, value); } }

        public static readonly BindableProperty CategoryFilterProperty = BindableProperty.Create(nameof(CategoryFilter), typeof(string), typeof(SearchView), "");
        public string CategoryFilter { get { return (string)GetValue(CategoryFilterProperty); } set { SetValue(CategoryFilterProperty, value); } }

        public static readonly BindableProperty AutoPopSearchPageProperty = BindableProperty.Create(nameof(AutoPopSearchPage), typeof(bool), typeof(SearchView), false);
        public bool AutoPopSearchPage { get { return (bool)GetValue(AutoPopSearchPageProperty); } set { SetValue(AutoPopSearchPageProperty, value); } }

        public static readonly BindableProperty OrientationProperty = BindableProperty.Create(nameof(Orientation), typeof(string), typeof(SearchView), null);
        public string Orientation { get { return (string)GetValue(OrientationProperty); } set { SetValue(OrientationProperty, value); } }

        public static readonly BindableProperty OrderByProperty = BindableProperty.Create(nameof(OrderBy), typeof(OrderByMode), typeof(SearchView), null);
        public OrderByMode OrderBy { get { return (OrderByMode)GetValue(OrderByProperty); } set { SetValue(OrderByProperty, value); } }

        public static readonly BindableProperty BatchScanProperty = BindableProperty.Create("BatchScan", typeof(bool), typeof(SearchView), false);
        public bool BatchScan { get { return (bool)GetValue(BatchScanProperty); } set { SetValue(BatchScanProperty, value); } }

        public ProgressInfo Progress { get => progress; set { progress = value; OnPropertyChanged(nameof(Progress)); } }

        public bool Busy
        {
            get
            {
                return busy;
            }
            set
            {
                busy = value;
                OnPropertyChanged(nameof(Busy));
            }
        }

        public SearchView()
        {
            InitializeComponent();
            Busy = false;
            if (!DesignMode.IsDesignModeEnabled)
            {
                client = new HttpClient();
            }
            SKUField.ReturnCommand = new Command(async () => await OnSubmit(SKUField.Text));
            SearchField.ReturnCommand = new Command(async () => await OnSubmit(SearchField.Text));
            SKUSubmitButton.Command = new Command(async () => await OnSubmit(SKUField.Text));
            SearchSubmitButton.Command = new Command(async () => await OnSubmit(SearchField.Text));
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (string.IsNullOrWhiteSpace(Orientation))
            {
                FlipStack.Orientation = width > height ? StackOrientation.Horizontal : StackOrientation.Vertical;
            }
            else
            {
                FlipStack.Orientation = Orientation == "Horizontal" ? StackOrientation.Horizontal : StackOrientation.Vertical;
            }

        }

        private void ReportProgress(ProgressInfo info)
        {
            Progress = info;
        }

        public static void DoScan(INavigation navigation, Func<string, IProgress<ProgressInfo>?, Task> resultTask, string categoryFilter = "", bool batchMode = false)
        {
            AnalyticsService.Track("DoScan");
            Device.BeginInvokeOnMainThread(async () =>
            {
                var options = new MobileBarcodeScanningOptions
                {
                    AutoRotate = false,
                    TryHarder = true,
                    PossibleFormats = new List<BarcodeFormat>() {
                        BarcodeFormat.CODE_128,
                        BarcodeFormat.UPC_A
                    },
                    UseNativeScanning = true,
                    DelayBetweenContinuousScans = 1000
                };
                var scanPage = new ScannerPage()
                {
                };

                // Navigate to our scanner page
                scanPage.OnScanResult += async (result) =>
                {
                    var currentPage = navigation.NavigationStack.LastOrDefault();
                    if (currentPage != scanPage)
                    {
                        return;
                    }

                    AnalyticsService.Track("Scan Result", result.DisplayValue);
                    Debug.WriteLine($"SCANNED {result}");
                    if (SettingsPage.Vibrate())
                    {
                        Vibration.Vibrate();
                    }
                    if (!batchMode)
                    {
                        // Stop scanning
                        //scanPage.IsScanning = false;
                        // Pop the page and show the result
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await navigation.PopAsync();

                            //SKUField.Text = FilterBarcodeResult(result);
                            //await OnSubmit(SKUField.Text);
                            await resultTask.Invoke(FilterBarcodeResult(result), null);
                        });
                    }
                    else
                    {
                        var progress = new Progress<ProgressInfo>((info) =>
                        {
                            scanPage.Progress = info;
                        });
                        //scanPage.IsScanning = false;
                        scanPage.IsBusy = true;
                        scanPage.IsRunningTask = true;
                        options.DelayBetweenContinuousScans = int.MaxValue;

                        await resultTask.Invoke(FilterBarcodeResult(result), progress);
                        options.DelayBetweenContinuousScans = 0;
                        //scanPage.IsScanning = true;
                        scanPage.IsBusy = false;
                        scanPage.IsRunningTask = false;
                    }
                };

                //var navPage = new NavigationPage(scanPage);
                //navPage.ToolbarItems.Add(new ToolbarItem()
                //{
                //    Text = "Cancel",
                //    Command = new Command(async () => { await navigation.PopModalAsync(); })
                //});
                await navigation.PushAsync(scanPage);
            });
        }

        private async void OnScanClicked(object sender, EventArgs e)
        {
            bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            if (allowed)
            {
                DoScan(Navigation, async (result, progress) =>
                {
                    SKUField.Text = result;
                    progress?.Report(new ProgressInfo($"Scanned {result}", .33d));
                    await OnSubmit(result);
                    progress?.Report(new ProgressInfo($"Submitted {result}", .66d));
                }, categoryFilter: CategoryFilter, batchMode: BatchScan);
            }
        }

        private static string FilterBarcodeResult(BarcodeResult result)
        {
            if (Regex.IsMatch(result.DisplayValue, "\\d{12}"))
            {
                return result.DisplayValue;
            }
            else if (result.DisplayValue.Length >= 6)
            {
                return result.DisplayValue.Substring(0, 6);
            }
            return "";

            //switch (result.BarcodeType)
            //{
            //    case BarcodeTypes:
            //        if(Regex.IsMatch(result.Text, "\\d{12}"))
            //        {
            //            return result.Text;
            //        }
            //        if (result.Text.Length >= 6)
            //        {
            //            return result.Text.Substring(0, 6);
            //        }
            //        return "";
            //    case BarcodeFormat.UPC_A:
            //    default:
            //        return result.Text;
            //}
        }
        public async Task OnSubmit(string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue) && string.IsNullOrWhiteSpace(CategoryFilter))
            {
                return;
            }

            AnalyticsService.Track("SearchSubmit", searchValue);

            var cachedItem = App.SearchCache?.Get(searchValue);
            if(cachedItem != null)
            {
                Debug.WriteLine("HIT NEW CACHE!!");
                AnalyticsService.Track("CacheHit", searchValue);
                DoProductFound(cachedItem);
                return;
            }

            Busy = true;
            tokenSource = new CancellationTokenSource();
            var progress = new Progress<ProgressInfo>(ReportProgress);

            var storeId = SettingsPage.StoreID();
            int queryAttempts = 0;

            //go back here on error, so that we can retry the request a few times
            const int NUM_RETRY_ATTEMPTS = 5;
        startQuery:
            queryAttempts++;

            try
            {
                var sw = Stopwatch.StartNew();
                var results = await LoadQuery(searchValue, storeId, CategoryFilter, OrderBy, 1, token: cancellationToken, progress: progress);
                sw.Stop();

                AnalyticsService.Track("Load Query Elapsed", ElapsedToAnalytics(sw.ElapsedMilliseconds));

                if (results != null)
                {

                    switch (results.Items.Count)
                    {
                        case 0:
                            DoError($"Failed to find product with query {searchValue}");
                            break;
                        case 1:
                            var stub = results.Items.First();
                            sw.Restart();
                            var item = await Item.FromUrl(stub.URL, storeId, token: cancellationToken, progress: progress);
                            sw.Stop();

                            AnalyticsService.Track("Item From Url Elapsed", ElapsedToAnalytics(sw.ElapsedMilliseconds));
                            App.SearchCache?.Add(item);
                            DoProductFound(item);
                            break;
                        default:
                            //count > 1
                            var page = new SearchResultsPage()
                            {
                                BindingContext = new SearchResultsPageViewModel()
                                {
                                    SearchQuery = searchValue,
                                    StoreID = storeId,
                                    CategoryFilter = CategoryFilter,
                                    OrderBy = OrderBy,
                                }
                            };


                            if (page.BindingContext is SearchResultsPageViewModel vm)
                            {
                                vm.ParseResults(results);
                            }

                            page.AutoPop = AutoPopSearchPage;
                            page.ItemTapped += (sender, args) =>
                            {
                                Task.Run(async () =>
                                {
                                    if (args.CurrentSelection.FirstOrDefault() is Item shortItem)
                                    {
                                        Busy = true;
                                        var sw2 = Stopwatch.StartNew();
                                        var item = await Item.FromUrl(shortItem.URL, storeId, token: cancellationToken, progress: progress);
                                        sw2.Stop();

                                        AnalyticsService.Track("Item From Url Elapsed", ElapsedToAnalytics(sw2.ElapsedMilliseconds));
                                        Busy = false;
                                        App.SearchCache?.Add(item);
                                        DoProductFound(item);
                                    }
                                }, cancellationToken).ContinueWith((task) => { Busy = false; });
                            };

                            await Device.InvokeOnMainThreadAsync(async () =>
                            {
                                await Shell.Current.Navigation.PushAsync(page);
                            });

                            break;
                    }
                }
            }
            catch (TaskCanceledException e)
            {
                AnalyticsService.Track("Search Submit Cancelled");
                //triggered by user input, do nothing
            }
            catch (OperationCanceledException e)
            {
                AnalyticsService.Track("Search Submit Cancelled");
                //triggered by user input, do nothing
            }
            catch (Exception e)
            {
                if (queryAttempts > NUM_RETRY_ATTEMPTS)
                {
                    DoError(e.Message);
                    AnalyticsService.TrackError(e, searchValue);
                }
                else
                {
                    AnalyticsService.Track("Search Submit Retrying");
                    if (progress is IProgress<ProgressInfo> p)
                    {
                        p.Report(new ProgressInfo($"Retrying query...{queryAttempts}/{NUM_RETRY_ATTEMPTS}", 0));
                    }
                    await Task.Delay(500);
                    client = new HttpClient();
                    goto startQuery;
                }
            }
            Busy = false;
        }

        private async void DoProductFound(Item item)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ProductFound?.Execute(item);
            });
        }

        private async void DoError(string message)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                Error?.Execute(message);
            });
        }

        private void CancelButton_Clicked(object sender, EventArgs e)
        {
            tokenSource.Cancel();
            Busy = false;
        }

        public static string ElapsedToAnalytics(long ms)
        {
            if(ms < 1 * 1000)
            {
                return "0-1 Sec";
            }

            if(ms < 2 * 1000)
            {
                return "1-2 Sec";
            }

            if(ms < 5 * 1000)
            {
                return "2-5 Sec";
            }

            if (ms < 10 * 1000)
            {
                return "5-10 Sec";
            }

            return "10+ Sec";
        }
    }
}
