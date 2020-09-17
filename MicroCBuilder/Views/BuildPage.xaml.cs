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
        BuildPageViewModel vm => DataContext as BuildPageViewModel;
        private PrintHelper _printHelper;

        public BuildPage()
        {
            this.InitializeComponent();
            var addCommand = vm.Add;
            var componentTypes = Enum.GetValues(typeof(BuildComponent.ComponentType)).Cast<BuildComponent.ComponentType>();
            foreach (var type in componentTypes)
            {
                ((MenuFlyout)AddButton.Flyout).Items.Add(new MenuFlyoutItem() { Text = type.ToString(), Command = addCommand, CommandParameter = type });
            }
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

            SearchView.Focus(FocusState.Keyboard);
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

        private async void PrintButton_Click(object sender, RoutedEventArgs e)
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
                Grid page = new Grid();
                page.Padding = new Thickness(40, 10, 40, 10);

                //
                //force grid to full width
                //
                page.Children.Add(new Canvas() { Width = 1000 });
                TextBlock header = new TextBlock();
                header.TextAlignment = TextAlignment.Center;
                header.Text = "";

                var footer = new BuildSummaryControl();
                footer.SubTotal = vm.SubTotal;

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
                    var border = new Border();
                    border.BorderThickness = new Thickness(1, 0, 1, 1);
                    border.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black);
                    border.Child = item;

                    //add it to the grid
                    contents.Children.Add(border);
                    contents.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });
                    Grid.SetRow(border, j);
                }

                var border2 = new Border() { Child = contents, BorderBrush = new SolidColorBrush(Windows.UI.Colors.Black), BorderThickness = new Thickness(0, 1, 0, 0) };
                page.Children.Add(border2);
                Grid.SetRow(border2, 1);

                //add full grid as new page
                _printHelper.AddFrameworkElementToPrint(page);
            }

            _printHelper.OnPrintCanceled += PrintHelper_OnPrintCanceled;
            _printHelper.OnPrintFailed += PrintHelper_OnPrintFailed;
            _printHelper.OnPrintSucceeded += PrintHelper_OnPrintSucceeded;

            var printHelperOptions = new PrintHelperOptions(true);
            printHelperOptions.Orientation = Windows.Graphics.Printing.PrintOrientation.Portrait;

            await _printHelper.ShowPrintUIAsync("Windows Community Toolkit Sample App", printHelperOptions);
        }

        private async void PrintHelper_OnPrintSucceeded()
        {
            ReleasePrintHelper();
            var dialog = new MessageDialog("Printing done.");
            await dialog.ShowAsync();
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
    }
}
