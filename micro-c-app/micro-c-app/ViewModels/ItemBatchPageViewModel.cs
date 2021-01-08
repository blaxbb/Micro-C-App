using micro_c_app.Models;
using micro_c_app.ViewModels;
using MicroCLib.Models;
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

        public Item? SelectedItem { get => selectedItem; set { SetProperty(ref selectedItem, value); } }
        public ObservableCollection<Item> Items { get => items; set => SetProperty(ref items, value); }
        public ICommand OnProductFound { get; }
        public ICommand OnProductError { get; }
        public ICommand IncreaseQuantity { get; }
        public ICommand DecreaseQuantity { get; }
        public ICommand RemoveItem { get; }
        public ICommand DetailItem { get; }
        public ICommand Reset { get; }

        protected override Dictionary<string, ICommand> Actions => new Dictionary<string, ICommand>()
        {
            {"Reset", Reset },
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
                if(item == null)
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
                    IncreaseQuantity.Execute(existing);
                }
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
        }
        public void UpdateProperties()
        {
            OnPropertyChanged(nameof(Items));
            RestoreState.Instance.BatchItems = Items.ToList();
            RestoreState.Save();
        }


    }
}
