using DataFlareClient;
using micro_c_app.Models;
using micro_c_app.Views;
using micro_c_app.Views.CollectionFile;
using micro_c_lib.Models;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;

using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class QuotePageViewModel : BaseViewModel
    {
        public INavigation Navigation;
        private bool notBusy;
        private Item? selectedItem;

        public Item? SelectedItem { get => selectedItem; set { SetProperty(ref selectedItem, value); } }
        public ObservableCollection<Item> Items { get; set; }
        public ICommand OnProductFound { get; }
        public ICommand OnProductError { get; }
        public ICommand IncreaseQuantity { get; }
        public ICommand DecreaseQuantity { get; }
        public ICommand RemoveItem { get; }
        public ICommand DetailItem { get; }

        public ICommand SendQuote { get; }
        public ICommand ExportQuote { get; }
        public ICommand ImportQuote { get; }
        public ICommand Reset { get; }
        public ICommand Save { get; }
        public ICommand Load { get; }
        public ICommand ImportWeb { get; }
        public ICommand ExportWeb { get; }
        public ICommand BatchScan { get; }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            {"Send", SendQuote },
            {"Reset", Reset },
            {"Save", Save },
            {"Load", Load },
            {"Import", ImportWeb },
            {"Export", ExportWeb },
            {"Batch", BatchScan }
        };

        public bool NotBusy { get => notBusy; set { SetProperty(ref notBusy, value); } }

        public float Subtotal => Items.Sum(i => i.Price * i.Quantity);
        public string TaxedTotal => $"({SettingsPage.TaxRate()})% ${Subtotal * SettingsPage.TaxRateFactor():#0.00}";

        public QuotePageViewModel()
        {
            Title = "Quote";
            NotBusy = true;
            if (RestoreState.Instance.QuoteItems != null)
            {
                Items = new ObservableCollection<Item>(RestoreState.Instance.QuoteItems);
            }
            else
            {
                Items = new ObservableCollection<Item>();
            }

            //for (int i = 0; i < 10; i++)
            //{
            //    var item = new Item()
            //    {
            //        Name = "ITEM",
            //        SKU = "123456",
            //        OriginalPrice = 100,
            //        Price = 90,
            //        Stock = "5 in stock",
            //    };
            //    Items.Add(item);
            //}

            Items.CollectionChanged += (sender, args) =>
            {
                UpdateProperties();
            };

            OnProductFound = new Command<Item>((Item item) =>
            {
                Items.Add(item);
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", message, "Ok");
                }
            });

            IncreaseQuantity = new Command<Item>((Item item) =>
            {
                item.Quantity++;
                UpdateProperties();
            });

            DecreaseQuantity = new Command<Item>((Item item) =>
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    UpdateProperties();
                }
            });
            RemoveItem = new Command<Item>(async (Item item) =>
            {
                /*
                 * 
                 * I am not sure if this a xaml bug or not, but the layout for the elements that
                 * toggle display visibility on selected break if the list of items is not cleared
                 * and reset when removing...
                 * 
                 * 
                 * Ideally we just call Items.Remove(item) and are done with it, worth investigating longer
                 * 
                 * Also see RemindersPageViewModel
                 */
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    if (item != null)
                    {
                        var reset = await Shell.Current.DisplayAlert("Remove", $"Are you sure you want to remove {item.Name}", "Yes", "No");
                        if (reset)
                        {
                            Items.Remove(item);

                            var tmp = Items.ToList();
                            Items.Clear();
                            foreach (var i in tmp)
                            {
                                Items.Add(i);
                            }
                        }
                    }
                });
            });

            SendQuote = new Command(async () => await DoSendQuote(Items));

            ExportQuote = new Command(async () => await DoExportQuote(Items));

            ImportQuote = new Command(async () => await ImportQuoteAction());

            MessagingCenter.Subscribe<SettingsPageViewModel>(this, SettingsPageViewModel.SETTINGS_UPDATED_MESSAGE, (_) => { UpdateProperties(); });

            Reset = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var reset = await Shell.Current.DisplayAlert("Reset", "Are you sure you want to reset the quote?", "Yes", "No");
                    if (reset)
                    {
                        Items.Clear();
                        UpdateProperties();
                    }
                });
            });

            DetailItem = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var detailsPage = new ItemDetailsPageViewModel() { Item = SelectedItem };
                    await Shell.Current.Navigation.PushAsync(new ItemDetailsPage() { BindingContext = detailsPage, Item = SelectedItem });
                });
            });

            Save = new Command(async () =>
            {
                var vm = new CollectionSavePageViewModel("quote", Items.ToList());

                await Shell.Current.Navigation.PushModalAsync(new CollectionSavePage() { BindingContext = vm });
            });

            Load = new Command(async () =>
            {
                var vm = new CollectionLoadPageViewModel<Item>("quote");
                MessagingCenter.Subscribe<CollectionLoadPageViewModel<Item>>(this, "load", DoLoad, vm);
                await Shell.Current.Navigation.PushModalAsync(new CollectionLoadPage() { BindingContext = vm });
            });

            ImportWeb = new Command(async () =>
            {
                var shortCode = await Shell.Current.DisplayPromptAsync("Import", "Enter the code from Micro-C-Builder to import a quote", keyboard:Keyboard.Numeric);
                if (string.IsNullOrWhiteSpace(shortCode))
                {
                    return;
                }

                var flare = await Flare.GetShortCode("https://dataflare.bbarrettnas.duckdns.org/api/Flare", shortCode);
                if(flare == null || string.IsNullOrWhiteSpace(flare.Data))
                {
                    return;
                }

                Items.Clear();

                var components = JsonConvert.DeserializeObject<List<BuildComponent>>(flare.Data);
                foreach(var comp in components)
                {
                    if (comp.Item != null)
                    {
                        Items.Add(comp.Item);
                    }
                }
            });

            ExportWeb = new Command(async () =>
            {
                var components = Items.Select(i => new BuildComponent() { Item = i }).ToList();
                var flare = new Flare(JsonConvert.SerializeObject(components));
                flare.Tag = $"micro-c-{SettingsPage.StoreID()}";
                var success = await flare.Post("https://dataflare.bbarrettnas.duckdns.org/api/Flare");
                if (success)
                {
                    await Shell.Current.DisplayAlert("Import using code", $"{flare.ShortCode}", "Ok");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to export to DataFlare server.", "Ok");
                }
            });

            BatchScan = new Command(() => DoBatchScan());
        }

        private void DoLoad(CollectionLoadPageViewModel<Item> obj)
        {
            Items.Clear();
            foreach (var i in obj.Result)
            {
                Items.Add(i);
            }

            UpdateProperties();
        }

        public static async Task DoSendQuote(IEnumerable<Item> Items)
        {
            try
            {
                var message = new EmailMessage()
                {
                    Subject = $"MicroCenter Quote - {DateTime.Today.ToShortDateString()}",
                    Body = ExportTxtTable(Items),
                };

                if (SettingsPage.IncludeCSVWithQuote())
                {
                    var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "quote.csv");
                    File.WriteAllText(file, ExportCsv(Items));
                    var attach = new EmailAttachment(file);
                    message.Attachments = new List<EmailAttachment>()
                    {
                        attach
                    };
                }
                await Email.ComposeAsync(message);
            }
            catch (Exception e)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("Error", e.ToString(), "Ok");
                });
            }
        }

        public static string ExportCsv(IEnumerable<Item> items)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"SKU,Name,Qty,Unit,Price");
            foreach (var item in items)
            {
                b.AppendLine($"{item.SKU},{item.Name},{item.Quantity},${item.Price:#0.00},${item.Price * item.Quantity:#0.00}");
            }

            var Subtotal = items.Sum(i => i.Price * i.Quantity);
            var TaxedTotal = Subtotal * SettingsPage.TaxRateFactor();

            b.AppendLine();
            var salesId = Preferences.Get("sales_id", "SALESID");
            b.AppendLine($"Subtotal,${Subtotal:#0.00},,Sales ID,{salesId}");
            b.AppendLine($"Total ({SettingsPage.TaxRate()}),${TaxedTotal:#0.00},,Contact,{salesId}@microcenter.com");

            return b.ToString();
        }

        public static string ExportTxtTable(IEnumerable<Item> items)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"SKU      {string.Format("{0,-50}", "Name")}\tQty  Unit      Price");
            b.AppendLine();
            foreach (var item in items)
            {
                b.AppendLine($"{item.SKU}\t{string.Format("{0,-40}", item.Name.Substring(0, Math.Min(item.Name.Length, 30)))}\t{item.Quantity}    ${item.Price:#0.00}    ${item.Price * item.Quantity:#0.00}");
            }

            var Subtotal = items.Sum(i => i.Price * i.Quantity);
            var TaxedTotal = Subtotal * SettingsPage.TaxRateFactor();

            b.AppendLine();
            b.AppendLine(string.Format("{0,78}", $"Sub ${Subtotal:#0.00}"));
            b.AppendLine(string.Format("{0,78}", $"Total ${TaxedTotal:#0.00}"));

            b.AppendLine();

            var salesId = Preferences.Get("sales_id", "SALESID");
            b.AppendLine($"Quote created by {salesId} for additional help contact me at {salesId}@microcenter.com");

            return b.ToString();
        }

        public static async Task DoExportQuote(IEnumerable<Item> Items)
        {
            var page = new ExportQRPage();
            if (page.BindingContext is ExportQRPageViewModel vm)
            {
                foreach (var i in Items)
                {
                    vm.Items.Add(i);
                }
            }

            await Shell.Current.Navigation.PushAsync(page);
        }

        private async Task ImportQuoteAction()
        {
            NotBusy = false;
            await DoImportQuote(
                new Command<Item>(async (Item i) =>
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("Title", i.Name, "Ok");
                    });
                }),
                new Command<Item>(async (Item i) =>
                {
                    await Device.InvokeOnMainThreadAsync(async () =>
                    {
                        await Shell.Current.DisplayAlert("ERROR", i.Name, "Ok");
                    });
                })
            );
            NotBusy = true;
        }

        public static async Task DoImportQuote(Command<Item> productFound, Command error)
        {
            var view = new SearchView();
            view.ProductFound = productFound;
            view.Error = error;
            await view.OnSubmit("951970");
        }


        private void DoBatchScan()
        {
            SearchView.DoScan(Navigation, async (result, progress) => {
                System.Diagnostics.Debug.WriteLine(result);

                try
                {
                    int queryAttempts = 0;
                    //go back here on error, so that we can retry the request a few times
                    const int NUM_RETRY_ATTEMPTS = 5;

                    var storeId = SettingsPage.StoreID();

                startQuery:
                    queryAttempts++;

                    try
                    {
                        var results = await Search.LoadQuery(result, storeId, null, Search.OrderByMode.match, 1, progress: progress);
                        if (results.Items.Count == 1)
                        {
                            var stub = results.Items.First();
                            var item = await Item.FromUrl(stub.URL, storeId, progress: progress);
                            if (item != null)
                            {
                                Items.Add(item);
                                return;
                            }
                        }
                    }
                    catch(Exception e)
                    {
                        if(queryAttempts > NUM_RETRY_ATTEMPTS)
                        {
                            await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
                            return;
                        }
                    }
                    if (queryAttempts > NUM_RETRY_ATTEMPTS)
                    {
                        await Shell.Current.DisplayAlert("Error", $"Failed to find item \"{result}\"", "Ok");
                    }
                    else
                    {
                        progress?.Report(new ProgressInfo($"Retrying query...{queryAttempts}/{NUM_RETRY_ATTEMPTS}", 0));
                        await Task.Delay(1000);
                        goto startQuery;
                    }
                }
                catch(Exception e)
                {
                    await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
                }
            }, batchMode: true);
        }

        public void UpdateProperties()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TaxedTotal));
            RestoreState.Instance.QuoteItems = Items.ToList();
            RestoreState.Save();
        }
    }
}