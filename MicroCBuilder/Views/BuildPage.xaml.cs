using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using MicroCBuilder.ViewModels;
using MicroCBuilder.Views;
using MicroCLib.Models;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using Windows.Devices.AllJoyn;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MicroCBuilder.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class BuildPage : Page
    {
        BuildPageViewModel? vm => DataContext as BuildPageViewModel;
        private PrintHelper _printHelper;

        public BuildPage()
        {
            this.InitializeComponent();
            var dispatcher = Window.Current.Dispatcher;

            Task.Run(async () => {
                await dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
                {
                    await MainPage.Instance.UpdateCache();
                });
            });
        }

        public void QueryUpdated (BuildComponentControl control, string query)
        {
            vm.SelectedComponent = control.Component;
            vm.Query = query;
        }

        public void QuerySubmitted(BuildComponentControl control, string query)
        {
            vm.SelectedComponent = control.Component;
            vm.Query = query;
            SearchView.Update();

            SearchView.dataGrid.Focus(FocusState.Keyboard);
            SearchView.dataGrid.SelectedIndex = 0;
        }

        private void SearchView_OnItemSelected(object sender, Item item)
        {
            int index = ComponentListView.SelectedIndex + 1;
            if(index == ComponentListView.Items.Count)
            {
                index = 0;
            }
            var container = ComponentListView.ContainerFromIndex(index) as Control;
            container?.Focus(FocusState.Keyboard);
            //ComponentListView.Focus(FocusState.Keyboard);
        }

        public async Task PrintClicked()
        {
            var grid = new Grid();
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
            grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

            var cb = new CheckBox() { Content = "Export to MCOL" };
            var tb = new TextBox() { PlaceholderText = "Sales ID" };

            grid.Children.Add(cb);
            grid.Children.Add(tb);

            Grid.SetRow(cb, 0);
            Grid.SetRow(tb, 1);

            var dialog = new ContentDialog()
            {
                Title = "Print options",
                Content = grid,
                PrimaryButtonText = "Print",
                SecondaryButtonText = "Cancel"
            };
            tb.KeyDown += (sender, args) => { if (args.Key == Windows.System.VirtualKey.Enter) dialog.Hide(); };
            var result = await dialog.ShowAsync();
            var name = tb.Text;
            var doExport = cb.IsChecked;
            if (result != ContentDialogResult.Secondary)
            {
                await DoPrint(name, doExport.HasValue ? doExport.Value : false);
            }
        }
        private async Task DoPrint(string salesID = "", bool exportToMCOL = false)
        { 

            var itemsCount = vm.Components.Count(c => c.Item != null);
            if (itemsCount == 0)
            {
                return;
            }

            _printHelper = new PrintHelper(Container);

            const int ITEMS_PER_PAGE = 12;
            
            for(int i = 0; i < itemsCount; i += ITEMS_PER_PAGE)
            {
                //create a new page
                Grid page = new Grid
                {
                    Padding = new Thickness(40, 10, 40, 10)
                };

                //
                //force grid to full width
                //
                page.Children.Add(new Canvas() { Width = 1000 });
                TextBlock header = new TextBlock
                {
                    TextWrapping = TextWrapping.WrapWholeWords,
                    TextAlignment = TextAlignment.Center
                };
                if (string.IsNullOrWhiteSpace(salesID))
                {
                    header.Text = $"Order created on {DateTime.Now:yyyy-MM-dd}.";
                }
                else
                {
                    header.Text = $"Order created on {DateTime.Now:yyyy-MM-dd} at the {Settings.Store()} MicroCenter Location.\nContact {salesID}@microcenter.com with additional questions.";
                }

                var buildContext = new MCOLBuildContext();
                if (exportToMCOL)
                {
                    await buildContext.AddComponents(vm.Components.ToList());
                }

                var footer = new BuildSummaryControl
                {
                    SubTotal = vm.SubTotal,
                    MCOLUrl = buildContext.TinyBuildURL
                };

                page.Children.Add(header);
                page.Children.Add(footer);

                Grid.SetRow(header, 0);
                Grid.SetRow(footer, 2);

                page.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                page.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                page.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                var contents = new Grid();
                var comps = vm.Components.Where(c => c.Item != null).Skip(i).Take(ITEMS_PER_PAGE).ToList();
                for (int j = 0; j < comps.Count; j++)
                {
                    var comp = comps[j];

                    //get element from usercontrol
                    var pv = new PrintView();
                    var item = pv.printGrid;
                    pv.Content = null;

                    var plan1 = PrintView.GetPlan(2, comp);
                    var plan2 = PrintView.GetPlan(3, comp);

                    if (plan1 != null && plan2 != null)
                    {
                        item.DataContext = new
                        {
                            Item = comp.Item,
                            PlanString = $"${plan1?.Item.Price}/${plan2.Item.Price}"
                        };
                    }
                    else
                    {
                        item.DataContext = new
                        {
                            Item = comp.Item
                        };
                    }

                    //stick it in a border
                    var border = new Border
                    {
                        BorderThickness = new Thickness(1, 0, 1, 1),
                        BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black),
                        Child = item
                    };

                    //add it to the grid
                    contents.Children.Add(border);
                    contents.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    Grid.SetRow(border, j);
                }

                var border2 = new Border()
                {
                    Child = contents,
                    BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black),
                    BorderThickness = new Thickness(0, 1, 0, 0)
                };

                page.Children.Add(border2);
                Grid.SetRow(border2, 1);

                //add full grid as new page
                _printHelper.AddFrameworkElementToPrint(page);
            }

            _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

            var printHelperOptions = new PrintHelperOptions(true)
            {
                Orientation = Windows.Graphics.Printing.PrintOrientation.Portrait
            };

            await _printHelper.ShowPrintUIAsync("Print Quote", printHelperOptions);
        }

        public async Task PrintBarcodesClicked()
        {
            var grid = new Grid();
            grid.VerticalAlignment = VerticalAlignment.Stretch;
            grid.HorizontalAlignment = HorizontalAlignment.Stretch;
            grid.Margin = new Thickness(10);

            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });


            List<TextBox> serialInputs = new List<TextBox>();
            int row = 0;
            foreach(var comp in vm.Components.Where(c => c.Item != null))
            {
                var label = new TextBlock() {
                    Text = comp.Item.Name,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(5)
                };

                grid.Children.Add(label);
                Grid.SetColumn(label, 0);
                Grid.SetRow(label, row);

                for (int i = 0; i < comp.Item.Quantity; i++)
                {
                    /*
                     * Create a text input for each quantity of each build component
                     */

                    var input = new TextBox() {
                        PlaceholderText = "Serial",
                        HorizontalAlignment = HorizontalAlignment.Stretch,
                        Text = comp.Serials.Count > i ? comp.Serials[i] : ""
                    };

                    input.KeyDown += (sender, args) =>
                    {
                        if(args.Key == Windows.System.VirtualKey.Enter)
                        {
                            FocusManager.TryMoveFocus(FocusNavigationDirection.Down, new FindNextElementOptions()
                            {
                                SearchRoot = grid
                            });
                        }
                    };
                    input.Tag = (comp, i);

                    serialInputs.Add(input);
                    grid.Children.Add(input);
                    Grid.SetColumn(input, 1);

                    grid.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    Grid.SetRow(input, row);
                    row++;
                }

            }

            var scrollView = new ScrollViewer();
            scrollView.Content = grid;
            scrollView.Width = 500;

            var dialog = new ContentDialog()
            {
                Title = "Print Barcodes",
                Content = scrollView,
                PrimaryButtonText = "Print",
                SecondaryButtonText = "Cancel",
                FullSizeDesired = true,
            };

            var result = await dialog.ShowAsync();
            if (result == ContentDialogResult.Primary)
            {
                foreach (var input in serialInputs)
                {
                    /*
                     * Pass the updated serial back into the BuildComponent so it can be saved for later
                     */
                    if (input.Tag is (BuildComponent comp, int i))
                    {
                        if (comp.Serials.Count > i)
                        {
                            comp.Serials[i] = input.Text;
                        }
                        else
                        {
                            comp.Serials.Add(input.Text);
                        }
                    }
                }

                List<(BuildComponent comp, int index)> entries = new List<(BuildComponent comp, int index)>();


                BuildComponent last = null;
                foreach(var entry in serialInputs.Select(s => s.Tag).Cast<(BuildComponent comp, int index)>())
                {
                    /*
                     * Only add a single print entry if there are no serial numbers, otherwise do a single line item per serial number.
                     */
                    if(entry.comp == last && string.IsNullOrEmpty(entry.comp.Serials[entry.index]))
                    {
                        continue;
                    }

                    if(string.IsNullOrEmpty(entry.comp.Serials[entry.index]))
                    {
                        last = entry.comp;
                    }

                    entries.Add(entry);
                }

                await DoPrintBarcodes(entries);
            }
        }

        public async Task DoPrintBarcodes(List<(BuildComponent comp, int index)> serials)
        {
            var itemsCount = vm.Components.Count(c => c.Item != null);
            if (itemsCount == 0)
            {
                return;
            }

            _printHelper = new PrintHelper(Container);

            const int BARCODE_ITEMS_PER_PAGE = 9;

            int serialIndex = 0;

            for (int i = 0; i < itemsCount; i += BARCODE_ITEMS_PER_PAGE)
            {
                //create a new page
                Grid page = new Grid
                {
                    Padding = new Thickness(40, 10, 40, 10)
                };

                //
                //force grid to full width
                //
                page.Children.Add(new Canvas() { Width = 1000 });
                TextBlock header = new TextBlock
                {
                    TextWrapping = TextWrapping.WrapWholeWords,
                    TextAlignment = TextAlignment.Center
                };
                header.Text = $"Order created on {DateTime.Now:yyyy-MM-dd}.";

                var footer = new BuildSummaryControl
                {
                    SubTotal = vm.SubTotal
                };

                page.Children.Add(header);
                page.Children.Add(footer);

                Grid.SetRow(header, 0);
                Grid.SetRow(footer, 2);

                page.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                page.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
                page.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

                var contents = new Grid();
                //var comps = vm.Components.Where(c => c.Item != null).Skip(i).Take(BARCODE_ITEMS_PER_PAGE).ToList();
                var pageItems = serials.Skip(i).Take(BARCODE_ITEMS_PER_PAGE).ToList();
                for (int j = 0; j < pageItems.Count; j++)
                {
                    var lineItem = pageItems[j];
                    var serial = lineItem.comp.Serials[lineItem.index];
                    serialIndex++;
                    //get element from usercontrol
                    var pv = new BarcodePrintView();
                    var item = pv.printGrid;
                    pv.Content = null;
                    item.DataContext = lineItem.comp;
                    pv.SetImages(lineItem.comp.Item.SKU, serial);

                    //stick it in a border
                    var border = new Border
                    {
                        BorderThickness = new Thickness(1, 0, 1, 1),
                        BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black),
                        Child = item
                    };

                    //add it to the grid
                    contents.Children.Add(border);
                    contents.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    Grid.SetRow(border, j);
                }

                var border2 = new Border()
                {
                    Child = contents,
                    BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black),
                    BorderThickness = new Thickness(0, 1, 0, 0)
                };

                page.Children.Add(border2);
                Grid.SetRow(border2, 1);

                //add full grid as new page
                _printHelper.AddFrameworkElementToPrint(page);
            }

            _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

            var printHelperOptions = new PrintHelperOptions(true)
            {
                Orientation = Windows.Graphics.Printing.PrintOrientation.Portrait
            };

            await _printHelper.ShowPrintUIAsync("Print Quote", printHelperOptions);
        }

        private void PrintHelper_OnPrintSucceeded()
        {
            ReleasePrintHelper();
        }

        private async void PrintHelper_OnPrintFailed()
        {
            ReleasePrintHelper();
            var dialog = new MessageDialog("Printing failed.");
            await dialog.ShowAsync();
        }

        private void PrintHelper_OnPrintCanceled()
        {
            ReleasePrintHelper();
        }

        private void ReleasePrintHelper()
        {
            _printHelper.Dispose();
        }

        public void Reset()
        {
            for(int i = 0; i < vm.Components.Count; i++)
            {
                var li = ComponentListView.ContainerFromIndex(i) as ListViewItem;
                if(li.ContentTemplateRoot is BuildComponentControl control)
                {
                    control.SetTextBox("");
                }
            }
        }

        private async void FlareTapped(object sender, TappedRoutedEventArgs e)
        {
            if (vm.Flare != null)
            {
                var url = $"https://microc.bbarrett.me/quotes/{vm.Flare.ShortCode}";
                await Windows.System.Launcher.LaunchUriAsync(new Uri(url));
            }
        }
    }
}
