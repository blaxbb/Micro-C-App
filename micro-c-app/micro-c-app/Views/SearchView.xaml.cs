using micro_c_app.Models;
using micro_c_app.ViewModels;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
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
        CancellationToken cancellationToken;

        private bool busy;

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

        private void OnScanClicked(object sender, EventArgs e)
        {
            var filter = CategoryFilter;
            Console.WriteLine(filter);
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
                var scanPage = new ZXingScannerPage(options)
                {
                    DefaultOverlayShowFlashButton = true
                };
                // Navigate to our scanner page
                scanPage.OnScanResult += (result) =>
                {
                    Debug.WriteLine($"SCANNED {result}");
                    if (SettingsPage.Vibrate())
                    {
                        Vibration.Vibrate();
                    }
                    if (!BatchScan)
                    {
                        // Stop scanning
                        scanPage.IsScanning = false;
                        // Pop the page and show the result
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            await Navigation.PopModalAsync();
                            SKUField.Text = FilterBarcodeResult(result);
                            await OnSubmit(SKUField.Text);
                        });
                    }
                    else
                    {
                        Device.BeginInvokeOnMainThread(async () =>
                        {
                            SKUField.Text = FilterBarcodeResult(result);
                            await OnSubmit(SKUField.Text);
                        });
                    }
                };

                var navPage = new NavigationPage(scanPage);
                navPage.ToolbarItems.Add(new ToolbarItem()
                {
                    Text = "Cancel",
                    Command = new Command(async () => { await Navigation.PopModalAsync(); })
                });
                await Navigation.PushModalAsync(navPage);
            });
        }

        private string FilterBarcodeResult(Result result)
        {
            switch (result.BarcodeFormat)
            {
                case BarcodeFormat.CODE_128:
                    if(result.Text.Length >= 6)
                    {
                        return result.Text.Substring(0, 6);
                    }
                    return "";
                case BarcodeFormat.UPC_A:
                default:
                    return result.Text;
            }
        }

        public async Task OnSubmit(string searchValue)
        {
            if (string.IsNullOrWhiteSpace(searchValue) && string.IsNullOrWhiteSpace(CategoryFilter))
            {
                return;
            }
            Busy = true;
            var storeId = SettingsPage.StoreID();
            int queryAttempts = 0;

            //go back here on error, so that we can retry the request a few times
            const int NUM_RETRY_ATTEMPTS = 5;
        startQuery:
            queryAttempts++;

            try
            {
                UpdateCancellationToken();
                var results = await LoadQuery(searchValue, storeId, CategoryFilter, OrderBy, 1, cancellationToken);
                if (results != null)
                {

                    switch (results.Items.Count)
                    {
                        case 0:
                            DoError($"Failed to find product with query {searchValue}");
                            break;
                        case 1:
                            var stub = results.Items.First();

                            UpdateCancellationToken();
                            var item = await Item.FromUrl(stub.URL, storeId, cancellationToken);
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
                                UpdateCancellationToken();
                                Task.Run(async () =>
                                {
                                    if (args.CurrentSelection.FirstOrDefault() is Item shortItem)
                                    {
                                        Busy = true;
                                        var item = await Item.FromUrl(shortItem.URL, storeId, cancellationToken);
                                        Busy = false;
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
                //triggered by user input, do nothing
            }
            catch(Exception e)
            {
                if (queryAttempts > NUM_RETRY_ATTEMPTS)
                {
                    DoError(e.Message);
                }
                else
                {
                    await Task.Delay(100);
                    client = new HttpClient();
                    goto startQuery;
                }
            }

            Busy = false;
        }

        private void UpdateCancellationToken()
        {
            tokenSource = new CancellationTokenSource();
            cancellationToken = tokenSource.Token;
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
    }
}
