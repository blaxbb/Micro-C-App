using DataFlareClient;
using micro_c_app.Models;
using micro_c_app.ViewModels;
using micro_c_app.Views;
using micro_c_app.Views.CollectionFile;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class ItemBatchPageViewModel : BaseViewModel
    {
        private bool notBusy;
        private Item? selectedItem;
        private ObservableCollection<Item> items;
        private Item lastItem;

        public Item? SelectedItem { get => selectedItem; set { SetProperty(ref selectedItem, value); } }
        public ObservableCollection<Item> Items { get => items; set => SetProperty(ref items, value); }
        public ICommand OnProductFound { get; }
        public ICommand OnProductError { get; }
        public ICommand IncreaseQuantity { get; }
        public ICommand DecreaseQuantity { get; }
        public ICommand RemoveItem { get; }
        public ICommand DetailItem { get; }
        public ICommand Reset { get; }
        public ICommand Save { get; }
        public ICommand Load { get; }
        public ICommand ImportWeb { get; }
        public ICommand ExportWeb { get; }

        public Item LastItem { get => lastItem; set => SetProperty(ref lastItem, value); }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            {"Reset", Reset },
            {"Save", Save },
            {"Load", Load },
            {"Import", ImportWeb },
            {"Export", ExportWeb },
        };

        public ItemBatchPageViewModel()
        {
            if (RestoreState.Instance?.BatchItems != null)
            {
                Items = new ObservableCollection<Item>(RestoreState.Instance.BatchItems);
            }
            else
            {
                Items = new ObservableCollection<Item>();
            }

            Items.CollectionChanged += (sender, args) =>
            {
                UpdateProperties();
            };

            OnProductFound = new Command<Item>((Item item) =>
            {
                if (item == null)
                {
                    return;
                }

                var existing = Items.FirstOrDefault(i => i.SKU == item.SKU);
                if (existing == null)
                {
                    Items.Add(item);
                }
                else
                {
                    IncreaseQuantity?.Execute(existing);
                }

                UpdateProperties();
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

            Reset = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var reset = await Shell.Current.DisplayAlert("Reset", "Are you sure you want to reset this batch scan?", "Yes", "No");
                    if (reset)
                    {
                        Items.Clear();
                        UpdateProperties();
                    }
                });
            });

            Save = new Command(async () =>
            {
                var vm = new CollectionSavePageViewModel("batch", Items.ToList());

                await Shell.Current.Navigation.PushModalAsync(new CollectionSavePage() { BindingContext = vm });
            });

            Load = new Command(async () =>
            {
                var vm = new CollectionLoadPageViewModel<Item>("batch");
                MessagingCenter.Subscribe<CollectionLoadPageViewModel<Item>>(this, "load", DoLoad, vm);
                await Shell.Current.Navigation.PushModalAsync(new CollectionLoadPage() { BindingContext = vm });
            });

            ImportWeb = new Command(async () =>
            {
                var shortCode = await Shell.Current.DisplayPromptAsync("Import", "Enter the share code to import a item list", keyboard: Keyboard.Numeric);
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
                        Items.Add(comp.Item);
                    }
                }
            });

            ExportWeb = new Command(async () =>
            {
                var components = Items.Select(i => new BuildComponent() { Item = i }).ToList();
                var flare = new Flare(JsonConvert.SerializeObject(components));
                flare.Tag = $"micro-c-{SettingsPage.StoreID()}";
                var success = await flare.Post("https://dataflare.bbarrett.me/api/Flare");
                if (success)
                {
                    await Shell.Current.DisplayAlert("Import using code", $"{flare.ShortCode}", "Ok");
                }
                else
                {
                    await Shell.Current.DisplayAlert("Error", "Failed to export to DataFlare server.", "Ok");
                }
            });

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

        public void UpdateProperties()
        {
            OnPropertyChanged(nameof(Items));
            RestoreState.Instance.BatchItems = Items.ToList();
            RestoreState.Save();
        }


    }
}
