using FuzzySharp.SimilarityRatio;
using FuzzySharp.SimilarityRatio.Scorer.StrategySensitive;
using MicroCBuilder.ViewModels;
using MicroCBuilder.Views;
using MicroCLib.Models;
using Microsoft.Toolkit.Uwp.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
            var addCommand = vm.Add;
            var componentTypes = Enum.GetValues(typeof(BuildComponent.ComponentType)).Cast<BuildComponent.ComponentType>().Where(t => t != BuildComponent.ComponentType.Miscellaneous && t != BuildComponent.ComponentType.Plan);
            //foreach (var type in componentTypes)
            //{
            //    ((MenuFlyout)AddButton.Flyout).Items.Add(new MenuFlyoutItem() { Text = type.ToString(), Command = addCommand, CommandParameter = type });
            //}
            //((MenuFlyout)AddButton.Flyout).Items.Add(new MenuFlyoutSeparator());
            //((MenuFlyout)AddButton.Flyout).Items.Add(new MenuFlyoutItem() { Text = "Search", Command = vm.AddSearchItem });
            //((MenuFlyout)AddButton.Flyout).Items.Add(new MenuFlyoutItem() { Text = "Custom", Command = vm.AddCustomItem });

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
            var tb = new TextBox() { PlaceholderText = "Sales ID" };
            var dialog = new ContentDialog()
            {
                Title = "Print options",
                Content = tb,
                PrimaryButtonText = "Print",
                SecondaryButtonText = "Cancel"
            };
            tb.KeyDown += (sender, args) => { if (args.Key == Windows.System.VirtualKey.Enter) dialog.Hide(); };
            var result = await dialog.ShowAsync();
            var name = tb.Text;
            if (result != ContentDialogResult.Secondary)
            {
                await DoPrint(name);
            }
        }
        private async Task DoPrint(string salesID = "")
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
                var comps = vm.Components.Where(c => c.Item != null).Skip(i).Take(ITEMS_PER_PAGE).ToList();
                for (int j = 0; j < comps.Count; j++)
                {
                    var comp = comps[j];

                    //get element from usercontrol
                    var pv = new PrintView();
                    var item = pv.printGrid;
                    pv.Content = null;
                    item.DataContext = comp;

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
    }
}
