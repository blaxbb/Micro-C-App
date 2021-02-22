using MicroCLib.Models;
using MicroCBuilder.Views;
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
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using DataFlareClient;

using static MicroCLib.Models.BuildComponent.ComponentType;
using Newtonsoft.Json;
using Microsoft.Toolkit.Uwp.Helpers;
using Windows.UI.Xaml.Media;

namespace MicroCBuilder.ViewModels
{
    public class BuildPageViewModel : BaseViewModel
    {
        private BuildComponent selectedItem;

        public string Test { get; set; } = "AFASFSF";
        public ObservableCollection<BuildComponent> Components { get; }

        private string query;
        private Flare flare;
        private MCOLBuildContext buildContext;

        public ICommand Save { get; }
        public ICommand SaveSigns { get; }
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
        public ICommand ExportToWeb { get; }
        public ICommand ImportFromWeb { get; }

        public BuildComponent SelectedComponent { get => selectedItem; set => SetProperty(ref selectedItem, value); }

        public string Query { get => query; set => SetProperty(ref query, value); }

        public float SubTotal => Components.Where(c => c?.Item != null).Sum(c => c.Item.Price * c.Item.Quantity);

        public Flare Flare { get => flare; set => SetProperty(ref flare, value); }
        public MCOLBuildContext BuildContext { get => buildContext; set => SetProperty(ref buildContext, value); }

        public BuildPageViewModel()
        {
            BuildContext = new MCOLBuildContext();
            Components = new ObservableCollection<BuildComponent>();

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
            SaveSigns = new Command(DoSaveSigns);

            Load = new Command(DoLoad);

            Reset = new Command(DoReset);

            Remove = new Command<BuildComponent>(async (comp) => await DoRemove(comp));
            Add = new Command<BuildComponent.ComponentType>(AddItem);
            ItemSelected = new Command<Item>(async (Item item) => { await DoItemSelected(item); });

            RemoveFlyoutCommand = new Command<BuildComponent>(async (comp) => await DoRemove(comp));
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
                var url = BuildContext.BuildURL;
                if(string.IsNullOrWhiteSpace(url))
                {
                    return;
                }

                await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            });

            ItemValuesUpdated = new Command((_) =>
            {
                UpdateHintsAndErrors();

                Task.Run(async () =>
                {
                    await BuildContext.RemoveComponent(SelectedComponent);
                    await BuildContext.AddComponent(SelectedComponent);
                });
                OnPropertyChanged(nameof(SubTotal));
            });

            AddSearchItem = new Command(DoAddSearchItem);
            AddCustomItem = new Command(DoAddCustomItem);
            ExportToWeb = new Command(DoExportToWeb);
            ImportFromWeb = new Command(DoImportFromWeb);
        }

        private void UpdateHintsAndErrors()
        {
            foreach (var comp in Components)
            {
                comp.ErrorText = "";
                comp.HintText = "";
            }

            foreach (var dep in BuildComponentDependency.Dependencies)
            {
                var items = Components.Where(comp => comp.Item != null).Select(comp => comp.Item).ToList();
                var errors = dep.HasErrors(items);
                foreach (var result in errors)
                {
                    var matchingComp = Components.FirstOrDefault(comp => comp.Item == result.Primary);
                    matchingComp.ErrorText += result.Text.Replace("\n", ", ") + "\n\n";
                }

                foreach (var comp in Components)
                {
                    if (comp.Item != null)
                    {
                        continue;
                    }

                    var hint = dep.HintText(items, comp.Type);
                    if (!string.IsNullOrWhiteSpace(hint))
                    {
                        comp.HintText += hint.Replace("\n", ", ") + "\n\n";
                    }
                }
            }
        }

        private async Task DoItemSelected(Item item)
        {
            if (SelectedComponent != null)
            {
                if (SelectedComponent.Item != null)
                {
                    _ = BuildContext.RemoveComponent(SelectedComponent);
                }
                SelectedComponent.Item = item;
                _ = BuildContext.AddComponent(SelectedComponent);
                UpdateHintsAndErrors();
            }
            OnPropertyChanged(nameof(SubTotal));
        }

        private async void DoExportToWeb(object obj)
        {
            var flare = new Flare(JsonConvert.SerializeObject(Components.Where(c => c.Item != null).ToList()));
            flare.Tag = $"micro-c-{Settings.StoreID()}";
            var success = await flare.Post($"https://dataflare.bbarrett.me/api/Flare");

            if (!success)
            {
                flare = new Flare("") { ShortCode = 0000 };
            }

            Flare = flare;
            await Task.Delay(20 * 1000);
            Flare = null;
        }

        private async void DoImportFromWeb(object obj)
        {

            var tb = new TextBox() { PlaceholderText = "Code" };
            var dialog = new ContentDialog()
            {
                Title = "Import From Phone",
                Content = tb,
                PrimaryButtonText = "Import",
                SecondaryButtonText = "Cancel"
            };
            tb.KeyDown += (sender, args) => { if (args.Key == Windows.System.VirtualKey.Enter) dialog.Hide(); };
            var result = await dialog.ShowAsync();
            var shortCode = tb.Text;
            if (result == ContentDialogResult.Secondary)
            {
                return;
            }

            var flare = await Flare.GetShortCode("https://dataflare.bbarrett.me/api/Flare", shortCode);

            if (flare != null && flare.ShortCode.ToString() == shortCode)
            {
                var json = flare.Data;
                var imported = JsonConvert.DeserializeObject<List<BuildComponent>>(json);
                if (imported.Count > 0)
                {
                    DoReset(null);
                    foreach (var comp in imported)
                    {
                        if (comp.Item == null)
                        {
                            continue;
                        }

                        if (string.IsNullOrWhiteSpace(comp.CategoryFilter))
                        {
                            comp.Type = BuildComponentCache.Current.FindType(comp.Item.SKU);
                        }

                        var existing = Components.FirstOrDefault(c => c.Type == comp.Type);
                        if (existing == null)
                        {
                            Components.Add(comp);
                        }
                        else
                        {
                            if (existing.Item == null)
                            {
                                existing.Item = comp.Item;
                            }
                            else
                            {
                                Components.Insert(Components.IndexOf(existing) + 1, comp);
                            }
                        }
                    }
                }
            }

            UpdateHintsAndErrors();
            OnPropertyChanged(nameof(SubTotal));
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

            UpdateHintsAndErrors();
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

            MicroCLib.Models.SearchResults? results = null;
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
                    var comp = new BuildComponent() { Type = BuildComponent.ComponentType.Miscellaneous, Item = item };
                    AddDuplicate(comp);

                }
            }

            UpdateHintsAndErrors();
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

        private async Task DoRemove(BuildComponent comp)
        {
            if (comp == null)
            {
                return;
            }

            await BuildContext.RemoveComponent(comp);

            comp.Item = null;
            if (comp.Type == BuildComponent.ComponentType.Miscellaneous || comp.Type == BuildComponent.ComponentType.Plan || Components.Count(c => c.Type == comp.Type) > 1)
            {
                Components.Remove(comp);
            }

            OnPropertyChanged(nameof(SubTotal));
            UpdateHintsAndErrors();
        }

        private void AddItem(BuildComponent.ComponentType type)
        {
            SelectedComponent = InsertAtEndByType(type);
            UpdateHintsAndErrors();
        }

        private void AddDuplicate(BuildComponent orig)
        {
            var comp = InsertAtEndByType(orig.Type);
            comp.Item = orig.Item?.CloneAndResetQuantity();
            SelectedComponent = comp;
            _ = BuildContext.AddComponent(comp);
            OnPropertyChanged(nameof(SubTotal));
            UpdateHintsAndErrors();
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
            comp.PropertyChanged += (sender, args) =>
            {
                OnPropertyChanged(nameof(Components));
            };
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
            UpdateHintsAndErrors();
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

            UpdateHintsAndErrors();
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

            UpdateHintsAndErrors();
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

        private async void DoSaveSigns(object obj)
        {
            var text = string.Join(Environment.NewLine, Components.Where(c => c.Item != null).Select(c => c.Item.SKU));
            var titleTextBox = new TextBox()
            {
                PlaceholderText = "Batch Title",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5)
            };

            var signs = new string[]
            {
                "AD_PEG",
                "BULK"
            };

            var signComboBox = new ComboBox()
            {
                ItemsSource = signs,
                SelectedIndex = 0,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = new Thickness(5)
            };

            var stack = new StackPanel()
            {
                Orientation = Orientation.Vertical
            };
            stack.Children.Add(titleTextBox);
            stack.Children.Add(signComboBox);

            var dialog = new ContentDialog()
            {
                Title = "Save Signs",
                Content = stack,
                PrimaryButtonText = "Save",
                SecondaryButtonText = "Cancel"
            };

        showDialog:
            var result = await dialog.ShowAsync();

            if (result != ContentDialogResult.Primary)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(titleTextBox.Text))
            {
                Windows.UI.Popups.MessageDialog msg = new Windows.UI.Popups.MessageDialog("Batch title cannot be empty!", "Error");
                await msg.ShowAsync();
                goto showDialog;
            }

            var savePicker = new Windows.Storage.Pickers.FileSavePicker
            {
                SuggestedStartLocation = Windows.Storage.Pickers.PickerLocationId.DocumentsLibrary,
                SuggestedFileName = $"{titleTextBox.Text}-{signComboBox.SelectedItem}-{Settings.StoreID()}.txt",
            };

            // Dropdown of file types the user can save the file as
            savePicker.FileTypeChoices.Add("Text File", new List<string>() { ".txt" });
            // Default file name if the user does not type one in or select a file to replace

            var file = await savePicker.PickSaveFileAsync();
            if (file != null)
            {
                // Prevent updates to the remote version of the file until
                // we finish making changes and call CompleteUpdatesAsync.
                Windows.Storage.CachedFileManager.DeferUpdates(file);
                // write to file
                await Windows.Storage.FileIO.WriteTextAsync(file, text);
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
