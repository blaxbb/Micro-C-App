using DataFlareClient;
using micro_c_app.Models;
using micro_c_app.Views;
using micro_c_app.Views.CollectionFile;
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
        public static QuotePageViewModel Instance;
        public INavigation Navigation;
        private bool notBusy;
        private BuildComponent? selectedItem;

        public BuildComponent? SelectedItem { get => selectedItem; set { SetProperty(ref selectedItem, value); } }
        public ObservableCollection<BuildComponent> Items { get => items; set => SetProperty(ref items, value); }
        public ICommand OnProductFound { get; }
        public ICommand OnProductFastFound { get; }
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
            {"Save", Save },
            {"Load", Load },
            {"Import", ImportWeb },
            {"Batch Scan", BatchScan },
            {"Reset", Reset }
        };

        public bool NotBusy { get => notBusy; set { SetProperty(ref notBusy, value); } }

        public float Subtotal => Items.Sum(i => i.Item != null ? i.Item.Price * i.Item.Quantity : 0);
        public float TaxRate => SettingsPage.TaxRate();
        public float TaxedTotal => Subtotal * SettingsPage.TaxRateFactor();

        private BuildComponent lastItem;
        private bool lastItemWasFast;
        private ObservableCollection<BuildComponent> items;

        public BuildComponent LastItem { get => lastItem; set => SetProperty(ref lastItem, value); }

        public int TotalUnits => Items.Sum(i => i.Item != null ? i.Item.Quantity : 0);

        public QuotePageViewModel()
        {
            Instance = this;
            Title = "Quote";
            NotBusy = true;
            if (RestoreState.Instance.MigratedQuoteItems != null)
            {
                Items = new ObservableCollection<BuildComponent>(RestoreState.Instance.MigratedQuoteItems);
            }
            else
            {
                Items = new ObservableCollection<BuildComponent>();
            }

            Items.CollectionChanged += (sender, args) =>
            {
                UpdateProperties();
            };

            OnProductFound = new Command<Item>((Item item) =>
            {
                if(lastItemWasFast)
                {
                    Items.Remove(LastItem);
                }
                lastItemWasFast = false;
                LastItem = AddNewItem(item);
            });

            OnProductFastFound = new Command<Item>((Item item) =>
            {
                LastItem = AddNewItem(item);
                lastItemWasFast = true;
            });

            OnProductError = new Command<string>(async (string message) =>
            {
                if (Shell.Current != null)
                {
                    await Shell.Current.DisplayAlert("Error", message, "Ok");
                }
            });

            IncreaseQuantity = new Command<BuildComponent>((BuildComponent comp) =>
            {
                if (comp?.Item != null)
                {
                    comp.Item.Quantity++;
                    UpdateProperties();
                }
            });

            DecreaseQuantity = new Command<BuildComponent>((BuildComponent comp) =>
            {
                if (comp?.Item != null)
                {
                    if (comp.Item.Quantity > 1)
                    {
                        comp.Item.Quantity--;
                        UpdateProperties();
                    }
                }
            });
            RemoveItem = new Command<BuildComponent>(async (BuildComponent comp) =>
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
                    if (comp != null && comp.Item != null)
                    {
                        var reset = await Shell.Current.DisplayAlert("Remove", $"Are you sure you want to remove {comp.Item.Name}", "Yes", "No");
                        if (reset)
                        {
                            Items.Remove(comp);

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
                    if (SelectedItem != null && SelectedItem.Item != null && !string.IsNullOrWhiteSpace(SelectedItem.Item.SKU) && SelectedItem.Item.SKU != "000000")
                    {
                        await Shell.Current.GoToAsync($"//SearchPage?search={SelectedItem.Item.SKU}");
                    }
                });
            });

            Load = new Command(async () =>
            {
                var page = await ImportPage.Create<BuildComponent>("quote");
                page.OnImportResults += (sender) =>
                {
                    if (sender.BindingContext is ImportPageViewModel<BuildComponent> vm && vm.Result != null)
                    {
                        Items = new ObservableCollection<BuildComponent>(vm.Result);
                    }
                };
            });

            ImportWeb = new Command(async () =>
            {
                var shortCode = await Shell.Current.DisplayPromptAsync("Import", "Enter the code from Micro-C-Builder to import a quote", keyboard: Keyboard.Numeric);
                if (string.IsNullOrWhiteSpace(shortCode))
                {
                    return;
                }

                var flare = await Flare.GetShortCode("https://dataflare.bbarrett.me/api/Flare", shortCode);
                if (flare == null || string.IsNullOrWhiteSpace(flare.Data))
                {
                    return;
                }

                Items.Clear();

                var components = JsonConvert.DeserializeObject<List<BuildComponent>>(flare.Data);
                foreach (var comp in components)
                {
                    if (comp.Item != null)
                    {
                        Items.Add(comp);
                    }
                }
            });

            Save = new Command(async () =>
            {
                await ExportPage.Create(Items.Where(i => i.Item != null).ToList(), "quote");
            });

            BatchScan = new Command(() => DoBatchScan());
        }

        private BuildComponent AddNewItem(Item item)
        {
            var comp = new BuildComponent()
            {
                Item = item,
                Type = item.ComponentType
            };

            Items.Add(comp);
            comp.PropertyChanged += (sender, args) => Instance.UpdateProperties();
            return comp;
        }

        public static void AddItem(Item item)
        {
            if(Instance == null)
            {
                if (RestoreState.Instance != null)
                {
                    RestoreState.Instance.QuoteItems.Add(item);
                    RestoreState.Save();
                }
            }
            else
            {
                var comp = new BuildComponent() { Item = item, Type = item.ComponentType };
                Instance.Items.Add(comp);
                comp.PropertyChanged += (sender, args) => { Instance.UpdateProperties(); };
                Instance.LastItem = comp;

            }
        }

        private void DoLoad(CollectionLoadPageViewModel<BuildComponent> obj)
        {
            Items.Clear();
            foreach (var i in obj.Result)
            {
                Items.Add(i);
            }

            UpdateProperties();
        }

        public static async Task DoSendQuote(IEnumerable<BuildComponent> Items)
        {
            try
            {
                var message = new EmailMessage()
                {
                    Subject = $"MicroCenter Quote - {DateTime.Today.ToShortDateString()}",
                    Body = ExportTxtTable(Items.Select(i => i.Item)),
                };

                if (SettingsPage.IncludeCSVWithQuote())
                {
                    var file = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "quote.csv");
                    File.WriteAllText(file, ExportCsv(Items.Select(i => i.Item)));
                    var attach = new EmailAttachment(file);
                    message.Attachments = new List<EmailAttachment>()
                    {
                        attach
                    };
                }
                //await Email.ComposeAsync(message);

                foreach (var line in message.Body.Split('\n'))
                {
                    Console.WriteLine(line);
                }

                await Share.RequestAsync(new ShareTextRequest()
                {
                    Title = message.Subject,
                    Text = message.Body,
                });
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
            b.AppendLine($"Subtotal,${Subtotal:#0.00}");
            b.AppendLine($"Total ({SettingsPage.TaxRate()}),${TaxedTotal:#0.00}");

            return b.ToString();
        }

        public static string ExportTxtTable(IEnumerable<Item> items)
        {
            StringBuilder b = new StringBuilder();
            b.AppendLine($"SKU         {string.Format("{0,-50}", "Name")}  Qty  Unit       Price");
            b.AppendLine();
            foreach (var item in items)
            {
                b.AppendLine($"{item.SKU}    {string.Format("{0,-40}", item.Name.Substring(0, Math.Min(item.Name.Length, 30)))}  {item.Quantity}    ${item.Price:#0.00}    ${item.Price * item.Quantity:#0.00}");
            }

            var Subtotal = items.Sum(i => i.Price * i.Quantity);
            var TaxedTotal = Subtotal * SettingsPage.TaxRateFactor();

            b.AppendLine();
            b.AppendLine(string.Format("{0,78}", $"Sub ${Subtotal:#0.00}"));
            b.AppendLine(string.Format("{0,78}", $"Total ${TaxedTotal:#0.00}"));

            b.AppendLine();

            var salesId = Preferences.Get("sales_id", "SALESID");
            if (!string.IsNullOrWhiteSpace(salesId))
            {
                //b.AppendLine($"Quote created by {salesId} for additional help contact me at {salesId}@microcenter.com");
            }

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
        }


        private void DoBatchScan()
        {
            SearchView.DoScan(Navigation, async (result, progress) =>
            {
                System.Diagnostics.Debug.WriteLine(result);

                try
                {
                    int queryAttempts = 0;
                    //go back here on error, so that we can retry the request a few times
                    const int NUM_RETRY_ATTEMPTS = 2;

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
                                LastItem = AddNewItem(item);
                                return LastItem;
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        if (queryAttempts > NUM_RETRY_ATTEMPTS)
                        {
                            await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
                            return null;
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
                catch (Exception e)
                {
                    await Shell.Current.DisplayAlert("Error", e.Message, "Ok");
                }

                return null;
            }, batchMode: true);
        }

        public void UpdateProperties()
        {
            OnPropertyChanged(nameof(Subtotal));
            OnPropertyChanged(nameof(TaxedTotal));
            OnPropertyChanged(nameof(TotalUnits));
            RestoreState.Instance.MigratedQuoteItems = Items.ToList();
            RestoreState.Save();
        }
    }
}