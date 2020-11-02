using micro_c_lib.Models;
using MicroCBuilder.Views;
using MicroCLib.Models;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DataFlareClient;

using static MicroCLib.Models.BuildComponent.ComponentType;

namespace MicroCBuilder.ViewModels
{
    public class BuildPageViewModel : BaseViewModel
    {
        private BuildComponent selectedItem;

        public string Test { get; set; } = "AFASFSF";
        public ObservableCollection<BuildComponent> Components { get; }
        public BuildComponent.ComponentType ttt = BuildComponent.ComponentType.GPU;
        private string query;
        private Flare flare;

        public ICommand Save { get; }
        public ICommand Load { get; }
        public ICommand Reset { get; }
        public ICommand Add { get; }
        public ICommand Remove { get; }
        public ICommand ItemSelected { get; }
        public ICommand ExportToMCOL { get; }
        public ICommand ItemValuesUpdated { get; }
        public ICommand RemoveFlyoutCommand { get; }
        public ICommand AddEmptyFlyoutCommand { get; }
        public ICommand AddDuplicateFlyoutCommand { get; }
        public ICommand InfoFlyoutCommand { get; }
        public ICommand AddSearchItem { get; }
        public ICommand AddCustomItem { get; }
        public ICommand ExportToPhone { get; }

        public BuildComponent SelectedComponent { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string Query { get => query; set => SetProperty(ref query, value); }

        public float SubTotal => Components.Where(c => c?.Item != null).Sum(c => c.Item.Price * c.Item.Quantity);

        public Flare Flare { get => flare; set => SetProperty(ref flare, value); }

        public BuildPageViewModel()
        {
            Components = new ObservableCollection<BuildComponent>();

            //DefaultComponentTypes().ToList().ForEach(c => {
            //    var comp = new BuildComponent() { Type = c };
            //    comp.PropertyChanged += (sender, args) =>
            //    {
            //        OnPropertyChanged(nameof(Components));
            //    };
            //    Components.Add(comp);
            //});

            Settings.Categories().ForEach(c =>
            {
                var comp = new BuildComponent() { Type = c };
                comp.PropertyChanged += (sender, args) =>
                {
                    OnPropertyChanged(nameof(Components));
                };
                Components.Add(comp);
            });


            Save = new Command(DoSave);

            Load = new Command(DoLoad);

            Reset = new Command(DoReset);

            Remove = new Command<BuildComponent>(DoRemove);
            Add = new Command<BuildComponent.ComponentType>(AddItem);
            ItemSelected = new Command<Item>((Item item) => { if (SelectedComponent != null) SelectedComponent.Item = item; OnPropertyChanged(nameof(SubTotal)); });

            RemoveFlyoutCommand = new Command<BuildComponent>(DoRemove);
            InfoFlyoutCommand = null;
            AddEmptyFlyoutCommand = new Command<BuildComponent.ComponentType>(AddItem);
            AddDuplicateFlyoutCommand = new Command<BuildComponent>(AddDuplicate);
            InfoFlyoutCommand = new Command<BuildComponent>(async (comp) =>
            {
                if (comp.Item != null && !string.IsNullOrWhiteSpace(comp.Item.URL))
                {
                    var success = await Windows.System.Launcher.LaunchUriAsync(new Uri($"https://microcenter.com{comp.Item.URL}"));
                }
            });

            ExportToMCOL = new Command(async (_) =>
            {
                var total = Components.Count(comp => comp != null && comp.Item != null);
                await MainPage.Instance.DisplayProgress(async (progress) =>
                {
                    var result = await BuildComponent.ExportToMCOL(Components.ToList(), progress);
                    Debug.WriteLine(result);
                    var success = await Windows.System.Launcher.LaunchUriAsync(new Uri(result));
                    if (!success)
                    {
                        //todo: show dialog with URL
                    }
                }, "Exporting to MCOL", total);
            });

            ItemValuesUpdated = new Command((_) =>
            {
                OnPropertyChanged(nameof(SubTotal));
            });

            AddSearchItem = new Command(DoAddSearchItem);
            AddCustomItem = new Command(DoAddCustomItem);
            ExportToPhone = new Command(DoExportToPhone);
        }

        private async void DoExportToPhone(object obj)
        {
            var flare = new Flare(JsonSerializer.Serialize(Components.Where(c => c.Item != null).ToList()));
            flare.Tag = $"micro-c-{Settings.StoreID()}";
            var success = await flare.Post($"https://dataflare.bbarrettnas.duckdns.org/api/Flare");

            if (!success)
            {
                flare = new Flare("") { ShortCode = 0000 };
            }

            Flare = flare;
            await Task.Delay(20 * 1000);
            Flare = null;
        }

        private async void DoAddCustomItem(object obj)
        {
            var name = new TextBox() { PlaceholderText = "Name" };
            var price = new NumberBox() { PlaceholderText = "Price" };
            var panel = new StackPanel() { Orientation = Orientation.Vertical };
            panel.Children.Add(name);
            panel.Children.Add(price);
            var dialog = new ContentDialog
            {
                Title = "Add - Search",
                Content = panel,
                PrimaryButtonText = "Submit",
                SecondaryButtonText = "Cancel"
            };
            name.KeyDown += (sender, args) => { if (args.Key == Windows.System.VirtualKey.Enter) dialog.Hide(); };
            price.KeyDown += (sender, args) => { if (args.Key == Windows.System.VirtualKey.Enter) dialog.Hide(); };

            var dialogResult = await dialog.ShowAsync();
            if (dialogResult != ContentDialogResult.Secondary)
            {
                AddDuplicate(new BuildComponent()
                {
                    Type = BuildComponent.ComponentType.Miscellaneous,
                    Item = new Item()
                    {
                        Name = name.Text,
                        Price = (float)(double.IsNaN(price.Value) ? 0d : price.Value)
                    }
                });
            }
        }

        private async void DoAddSearchItem(object obj)
        {
            var dispatcher = Window.Current.Dispatcher;

            var tb = new TextBox() { PlaceholderText = "Search query" };
            var dialog = new ContentDialog
            {
                Title = "Add - Search",
                Content = tb,
                PrimaryButtonText = "Submit",
                SecondaryButtonText = "Cancel"
            };

            tb.KeyDown += (sender, args) => { if (args.Key == Windows.System.VirtualKey.Enter) dialog.Hide(); };

            var dialogResult = await dialog.ShowAsync();
            var query = tb.Text;

            micro_c_lib.Models.SearchResults? results = null;
            if (dialogResult != ContentDialogResult.Secondary && !string.IsNullOrWhiteSpace(query))
            {
                await MainPage.Instance.DisplayProgress(async (progress) =>
                {
                    results = await Search.LoadAll(query, Settings.StoreID(), null, Search.OrderByMode.match);
                }, $"Searching for {query}", 1);
            }

            if (results != null)
            {
                Debug.WriteLine($"{results.Items.Count} RESULTS");
                Item? item = null;
                if (results.Items.Count == 1)
                {
                    item = results.Items.First();
                }
                else if (results.Items.Count > 0)
                {
                    item = await DisplaySearchResults(results.Items);
                }
                else
                {
                    var msg = new ContentDialog()
                    {
                        Title = "No results found.",
                        PrimaryButtonText = "Ok"
                    };
                    await msg.ShowAsync();
                }

                if (item != null)
                {
                    AddDuplicate(new BuildComponent() { Type = BuildComponent.ComponentType.Miscellaneous, Item = item });
                }
            }
        }

        private static async Task<Item?> DisplaySearchResults(List<Item> items)
        {
            var listView = new ListView()
            {
                ItemsSource = items
            };

            var dialog = new ContentDialog
            {
                Title = "Search Results",
                PrimaryButtonText = "Submit",
                SecondaryButtonText = "Cancel",
                Content = listView
            };

            listView.PreviewKeyDown += (sender, args) =>
            {
                if (args.Key == Windows.System.VirtualKey.Enter)
                {
                    dialog.Hide();
                    args.Handled = true;
                }
            };

            var result = await dialog.ShowAsync();
            if (result != ContentDialogResult.Secondary)
            {
                return listView.SelectedItem as Item;
            }
            return null;
        }

        private static IEnumerable<BuildComponent.ComponentType> DefaultComponentTypes()
        {
            yield return BuildService;
            yield return BuildComponent.ComponentType.OperatingSystem;
            yield return CPU;
            yield return Motherboard;
            yield return RAM;
            yield return Case;
            yield return PowerSupply;
            yield return GPU;
            yield return SSD;
            yield return HDD;
            yield return CPUCooler;
            yield return WaterCoolingKit;
            yield return CaseFan;
        }

        private void DoRemove(BuildComponent comp)
        {
            if (comp == null)
            {
                return;
            }

            comp.Item = null;
            if (comp.Type == BuildComponent.ComponentType.Miscellaneous || comp.Type == BuildComponent.ComponentType.Plan || Components.Count(c => c.Type == comp.Type) > 1)
            {
                Components.Remove(comp);
            }
            OnPropertyChanged(nameof(SubTotal));
        }

        private void AddItem(BuildComponent.ComponentType type)
        {
            SelectedComponent = InsertAtEndByType(type);
        }

        private void AddDuplicate(BuildComponent orig)
        {
            var comp = InsertAtEndByType(orig.Type);
            comp.Item = orig.Item?.CloneAndResetQuantity();
            SelectedComponent = comp;
            OnPropertyChanged(nameof(SubTotal));
        }

        private BuildComponent InsertAtEndByType(BuildComponent.ComponentType type)
        {
            int index = Components.Count;
            for (int i = Components.Count - 1; i >= 0; i--)
            {
                var existing = Components[i];
                if (existing.Type == type)
                {
                    index = i + 1;
                    break;
                }
            }

            var comp = new BuildComponent() { Type = type };
            Components.Insert(index, comp);
            OnPropertyChanged(nameof(SubTotal));
            return comp;
        }

        private void DoReset(object obj)
        {
            foreach (var c in Components)
            {
                c.Item = null;
            }
            OnPropertyChanged(nameof(SubTotal));
        }

        private async void DoLoad(object obj)
        {
            //
            //Get a collection of BuildComponents from a .build file
            //
            FileOpenPicker openPicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.DocumentsLibrary
            };

            openPicker.FileTypeFilter.Add(".build");
            StorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                DoReset(obj);
                var text = await Windows.Storage.FileIO.ReadTextAsync(file);
                var components = System.Text.Json.JsonSerializer.Deserialize<List<BuildComponent>>(text);

                //
                //If there is an existing empty component, use that one, otherwise create a new one
                //
                InsertComponents(components);
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }

        private void InsertComponents(List<BuildComponent> fromFile)
        {
            foreach (var loadedComp in fromFile)
            {
                bool found = false;
                foreach (var oldComp in Components)
                {
                    if (oldComp.Type == loadedComp.Type && oldComp.Item == null)
                    {
                        oldComp.Item = loadedComp.Item;
                        found = true;
                        break;
                    }
                }
                if (!found)
                {
                    var comp = InsertAtEndByType(loadedComp.Type);
                    comp.Item = loadedComp.Item;
                }
            }

            OnPropertyChanged(nameof(SubTotal));
        }

        private async void DoSave(object obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(Components.Where(c => c.Item != null), new System.Text.Json.JsonSerializerOptions() { WriteIndented = true });

            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                SuggestedFileName = "New Build"
            };

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("MCBuild", new List<string>() { ".build" });
            // Default file name if the user does not type one in or select a file to replace

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                await Windows.Storage.FileIO.WriteTextAsync(file, json);
                // Let Windows know that we're finished changing the file so
                // the other app can update the remote version of the file.
                // Completing updates may require Windows to ask for user input.
                Windows.Storage.Provider.FileUpdateStatus status =
                    await Windows.Storage.CachedFileManager.CompleteUpdatesAsync(file);
                if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
                {
                    Debug.WriteLine("File " + file.Name + " was saved.");
                }
                else
                {
                    Debug.WriteLine("File " + file.Name + " couldn't be saved.");
                }
            }
            else
            {
                Debug.WriteLine("Operation cancelled.");
            }
        }
    }
}
