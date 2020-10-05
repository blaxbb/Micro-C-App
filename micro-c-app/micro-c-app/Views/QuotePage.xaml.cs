
using micro_c_app.ViewModels;
using MicroCLib.Models;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class QuotePage : ContentPage
    {
        public QuotePage()
        {
            InitializeComponent();
            this.SetupActionButton();
            KeyboardHelper.KeyboardChanged += KeyboardHelper_KeyboardChanged;
        }

        private void KeyboardHelper_KeyboardChanged(object sender, KeyboardHelperEventArgs e)
        {
            grid.RowDefinitions[2].Height = e.Visible ? e.Height : 0;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if(Device.RuntimePlatform == "UWP")
            {
                //xamarin bug, shell does not pass correct width and height to contained pages on UWP
                width = App.Current.MainPage.Width;
                height = App.Current.MainPage.Height;
            }

            if (width > height)
            {
                Grid.SetRow(listView, 0);
                Grid.SetRow(SecondaryStack, 0);

                Grid.SetColumn(listView, 0);
                Grid.SetColumn(SecondaryStack, 1);
                SearchView.Orientation = "Vertical";
                grid.ColumnDefinitions[1].Width = new GridLength(1, GridUnitType.Star);
                grid.RowDefinitions[2].Height = 0;
            }
            else
            {
                Grid.SetRow(listView, 0);
                Grid.SetRow(SecondaryStack, 1);

                Grid.SetColumn(listView, 0);
                Grid.SetColumn(SecondaryStack, 0);
                SearchView.Orientation = "Horizontal";

                grid.ColumnDefinitions[1].Width = new GridLength(0);
            }
        }

        private void ItemPriceChanged(object sender, TextChangedEventArgs e)
        {
            if(BindingContext is QuotePageViewModel vm)
            {
                Task.Delay(1000).ContinueWith((_) =>
                {
                    vm.UpdateProperties();
                });
            }
        }

        Item? previousSelection;
        private void listView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            var newItem = e.Item;

            if (previousSelection == newItem)
            {
                listView.SelectedItem = null;
                previousSelection = null;
            }
            else
            {
                previousSelection = newItem as Item;
            }
        }
    }
}