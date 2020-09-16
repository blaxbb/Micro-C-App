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


        private async void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            MainGrid.Children.Remove(PrintContent);

            _printHelper = new PrintHelper(Container);

            //for (int i = 0; i < Lists.Count; i = i + 4)
            //{
            //    var grid = new Grid();
            //    // Main content with layout from data template
            //    var listView = new ListView();
            //    listView.ItemTemplate = CustomPrintTemplate;
            //    listView.ItemsSource = Lists.Skip(i).Take(4);
            //    grid.Children.Add(listView);
            //    _printHelper.AddFrameworkElementToPrint(grid);
            //}
            _printHelper.AddFrameworkElementToPrint(PrintContent);

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
            if (!MainGrid.Children.Contains(PrintContent))
            {
                MainGrid.Children.Add(PrintContent);
            }
        }
    }
}
