using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ItemBatchPage : ContentPage
    {
        public ItemBatchPage()
        {
            InitializeComponent();
            this.SetupActionButton();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (Device.RuntimePlatform == "UWP")
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