using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class BuildPageTabContainer : Page
    {
        public BuildPageTabContainer()
        {
            this.InitializeComponent();
        }

        private void PushTab(string title, Type pageType)
        {
            var tab = new TabViewItem()
            {
                Header = title,
            };

            var frame = new Frame();
            frame.HorizontalAlignment = HorizontalAlignment.Stretch;
            frame.VerticalAlignment = VerticalAlignment.Stretch;
            tab.Content = frame;
            frame.Navigate(pageType);

            Tabs.TabItems.Add(tab);
            Tabs.SelectedItem = tab;
        }

        private void Tabs_AddTabButtonClick(Microsoft.UI.Xaml.Controls.TabView sender, object args)
        {
            PushTab("Build", typeof(BuildPage));
        }

        private void Tabs_TabCloseRequested(Microsoft.UI.Xaml.Controls.TabView sender, Microsoft.UI.Xaml.Controls.TabViewTabCloseRequestedEventArgs args)
        {
            sender.TabItems.Remove(args.Tab);
        }

        private void LoadClicked(object sender, RoutedEventArgs e)
        {

        }

        private void SaveClicked(object sender, RoutedEventArgs e)
        {

        }

        private void PrintClicked(object sender, RoutedEventArgs e)
        {

        }

        private void ResetClicked(object sender, RoutedEventArgs e)
        {

        }

        private void ExportClicked(object sender, RoutedEventArgs e)
        {

        }

        private void MailClicked(object sender, RoutedEventArgs e)
        {

        }
        private void SettingsClick(object sender, RoutedEventArgs e)
        {
            PushTab("Settings", typeof(SettingsPage));
        }
    }
}
