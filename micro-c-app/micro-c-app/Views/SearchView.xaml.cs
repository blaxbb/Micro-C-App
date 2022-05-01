using GoogleVisionBarCodeScanner;
using micro_c_app.Models;
using micro_c_app.ViewModels;
using micro_c_lib.Models.Inventory;
using MicroCLib.Models;
using MicroCLib.Models.Reference;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
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
using static MicroCLib.Models.Search;

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
        public static readonly BindableProperty ProductFastFoundProperty = BindableProperty.Create(nameof(ProductFastFound), typeof(ICommand), typeof(SearchView), null);
        public static readonly BindableProperty ProductLocationFoundProperty = BindableProperty.Create(nameof(ProductLocationFound), typeof(ICommand), typeof(SearchView), null);

        public ICommand ProductFound { get { return (ICommand)GetValue(ProductFoundProperty); } set { SetValue(ProductFoundProperty, value); } }
        public ICommand ProductFastFound { get { return (ICommand)GetValue(ProductFastFoundProperty); } set { SetValue(ProductFastFoundProperty, value); } }
        public ICommand ProductLocationFound { get { return (ICommand)GetValue(ProductLocationFoundProperty); } set { SetValue(ProductLocationFoundProperty, value); } }

        public static readonly BindableProperty ErrorProperty = BindableProperty.Create(nameof(Error), typeof(ICommand), typeof(SearchView), null);

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

        public static readonly BindableProperty LastItemProperty = BindableProperty.Create("LastItem", typeof(BuildComponent), typeof(SearchView), null, propertyChanged: LastItemPropertyChanged);

        public ICommand SearchCommand { get; }

        private static void LastItemPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if(bindable is SearchView view)
            {
                MessagingCenter.Send<SearchView>(view, "LastItemUpdated");
            }
        }

        public BuildComponent LastItem { get => (BuildComponent)GetValue(LastItemProperty); set => SetValue(LastItemProperty, value); }

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

            SearchCommand = new Command<string>(async (categoryFilter) =>
            {
                var oldFilter = CategoryFilter;
                CategoryFilter = categoryFilter;
                await OnSubmit("");
                CategoryFilter = oldFilter;
            });

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

        public static async Task DoScan(INavigation navigation, Func<string, IProgress<ProgressInfo>?, Task<BuildComponent>> resultTask, Func<string, Task> locationResultTask, string categoryFilter = "", bool batchMode = false)
        {
            bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            if (!allowed)
            {
                return;
            }

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

                    AnalyticsService.Track("Scan Result", result.Value);
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

                            if (locationResultTask != null && IsLocationTag(result))
                            {
                                await locationResultTask.Invoke(result.Value);
                            }
                            else
                            {
                                await resultTask.Invoke(FilterBarcodeResult(result), null);
                            }
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

                        scanPage.LastItem = await resultTask.Invoke(FilterBarcodeResult(result), progress);
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

        public static async Task DoSerialScan(INavigation navigation, BuildComponent component, Action<BuildComponent> callback)
        {
            bool allowed = await GoogleVisionBarCodeScanner.Methods.AskForRequiredPermission();
            if (!allowed)
            {
                return;
            }

            AnalyticsService.Track("DoSerialScan");
            Device.BeginInvokeOnMainThread(async () =>
            {
                var scanPage = new ScannerPage()
                {
                    LastItem = component,
                    CurrentScanMode = ScannerPage.ScanMode.Serial,
                    OnlySerialMode = true
                };

                scanPage.Disappearing += (sender, args) =>
                {
                    callback?.Invoke(component);
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
                    return null;
                },
                async (result) =>
                {
                    using var client = new HttpClient();
                    var entries = await InventoryView.GetLocationEntries(client, result);
                    if(entries == null)
                    {
                        await Shell.Current.DisplayAlert("Error", $"Location {result} did not retrieve any information from server!", "Ok");
                    }
                    else if(entries.Count == 0)
                    {
                        await Shell.Current.DisplayAlert("Error", $"Location {result} did not retrieve any information from server!", "Ok");
                    }
                    else
                    {
                        var skuItems = entries.Where(e => !e.Sku.StartsWith("CL"));
                        SearchResults searchResults;
                        if (skuItems.Count() > 0)
                        {
                            searchResults = await Search.LoadMultipleFast(skuItems.Select(e => e.Sku).ToList());
                        }
                        else
                        {
                            searchResults = new SearchResults();
                        }

                        var page = new SearchResultsPage()
                        {
                            BindingContext = new SearchResultsPageViewModel()
                        };

                        foreach(var clearanceEntry in entries.Where(e => e.Sku.StartsWith("CL")))
                        {
                            searchResults.Items.Add(new Item() { Name = clearanceEntry.Sku });
                        }

                        if(page.BindingContext is SearchResultsPageViewModel vm)
                        {
                            vm.ParseResults(searchResults);
                        }

                        page.AutoPop = AutoPopSearchPage;
                        page.ItemTapped += (sender, args) =>
                        {
                            if (args.CurrentSelection.FirstOrDefault() is Item shortItem && shortItem.SKU.StartsWith("CL"))
                            {
                                return;
                            }

                            Task.Run(async () =>
                            {
                                if (args.CurrentSelection.FirstOrDefault() is Item shortItem)
                                {
                                    Busy = true;
                                    var sw2 = Stopwatch.StartNew();
                                    var item = await Item.FromUrl(shortItem.URL, SettingsPage.StoreID(), token: cancellationToken);
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
                    }

                }, categoryFilter: CategoryFilter, batchMode: BatchScan);
            }
        }

        public static string FilterBarcodeResult(BarcodeResult result)
        {
            if (Regex.IsMatch(result.Value, "\\d{12}"))
            {
                return result.Value;
            }

            if(result.Value.Length < 6)
            {
                return "";
            }

            var match = Regex.Match(result.Value, "\\d{6}");
            if (match.Success)
            {
                return match.Groups[0].Value;
            }

            return "";
        }

        public static bool IsLocationTag(BarcodeResult result)
        {
            return InventoryView.IsLocationIdentifier(result.Value);
        }

        public async Task OnSubmit(string searchValue)
        {
            Busy = true;
            var cachedItem = App.SearchCache?.Get(searchValue);
            if (cachedItem != null)
            {
                AnalyticsService.Track("CacheHit", searchValue);
                await DoProductFound(cachedItem);
                Busy = false;
                return;
            }

            //SKU or UPC
            if (ProductFastFound != null && !string.IsNullOrWhiteSpace(searchValue) && Regex.IsMatch(searchValue, "^\\d{6}$|^\\d{12}$"))
            {
                var fast = await OnSubmitFastQuery(searchValue);
                if (fast != null)
                {
                    DoProductFastFound(fast);
                }

                if (ProductFound != null)
                {
                    await OnSubmitTextQuery(searchValue);
                }
                return;
            }
            else
            {
                await OnSubmitTextQuery(searchValue);
            }

            Busy = false;
        }

        public async Task<Item> OnSubmitFastQuery(string searchValue)
        {
            AnalyticsService.Track("FastSearchSubmit", searchValue);
            tokenSource = new CancellationTokenSource();
            try
            {
                return await LoadFast(searchValue, cancellationToken);
            }
            catch(Exception ex)
            {
                return null;
            }
        }

        public async Task OnSubmitTextQuery(string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue) && string.IsNullOrWhiteSpace(CategoryFilter))
            {
                return;
            }

            AnalyticsService.Track("SearchSubmit", searchValue);

            bool enhancedSearch = SettingsPage.UseEnhancedSearch();

            Busy = true;
            tokenSource = new CancellationTokenSource();
            var progress = new Progress<ProgressInfo>(ReportProgress);

            var storeId = SettingsPage.StoreID();
            int queryAttempts = 0;

            //go back here on error, so that we can retry the request a few times
            const int NUM_RETRY_ATTEMPTS = 2;
        startQuery:
            queryAttempts++;

            try
            {
                var sw = Stopwatch.StartNew();
                SearchResults? results;
                if (enhancedSearch)
                {
                    results = await LoadEnhanced(searchValue, storeId, CategoryFilter, token: cancellationToken);
                }
                else
                {
                    results = await LoadQuery(searchValue, storeId, CategoryFilter, OrderBy, 1, token: cancellationToken, progress: progress);
                }
                var clearance = results.Items.Where(i => i.ClearanceItems.Count > 0).ToList();
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
                            Item item;
                            if(enhancedSearch && stub.Specs.Count > 0)
                            {
                                item = stub;
                            }
                            else
                            {
                                sw.Restart();
                                item = await Item.FromUrl(stub.URL, storeId, token: cancellationToken, progress: progress);
                                sw.Stop();
                                AnalyticsService.Track("Item From Url Elapsed", ElapsedToAnalytics(sw.ElapsedMilliseconds));
                            }

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
                                    EnhancedSearch = enhancedSearch
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

        async Task FindLocation(Item item)
        {
            try
            {
                string query;
                if(item.ClearanceItems.Count > 0)
                {
                    query = $"{item.SKU},{string.Join(",", item.ClearanceItems.Select(c => c.Id))}";
                }
                else
                {
                    query = $"{item.SKU}";
                }
                var response = await client.GetAsync($"{InventoryView.LOCATION_TRACKER_BASEURL}/api/Entries/Sku/{query}");
                if (response.IsSuccessStatusCode)
                {
                    var text = await response.Content.ReadAsStringAsync();
                    var entry = JsonConvert.DeserializeObject<List<InventoryEntry>>(text);
                    if (entry != null)
                    {
                        ProductLocationFound?.Execute(entry);
                    }
                }
            }
            catch(Exception e)
            {
                AnalyticsService.TrackError(e, item?.SKU ?? "null");
            }
        }

        private async Task DoProductFound(Item item)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ProductFound?.Execute(item);
            });

            if (SettingsPage.StoreID() == "141")
            {
                await FindLocation(item);
            }
        }

        private async void DoProductFastFound(Item item)
        {
            await Device.InvokeOnMainThreadAsync(() =>
            {
                ProductFastFound?.Execute(item);
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

        private void ClearSkuFieldClicked(object sender, EventArgs e)
        {
            SKUField.Text = "";
        }

        private void ClearSearchFieldClicked(object sender, EventArgs e)
        {
            SearchField.Text = "";
        }
    }
}
