
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
                FlipStack.Direction = FlexDirection.RowReverse;
                //SecondaryStack.VerticalOptions = LayoutOptions.FillAndExpand;
                SearchView.Orientation = "Vertical";
                SecondaryStack.WidthRequest = 300;
                SecondaryStack.HeightRequest = -1;
            }
            else
            {
                if(Device.RuntimePlatform == Device.iOS)
                {
                    FlipStack.Direction = FlexDirection.Column;
                }
                else
                {
                    FlipStack.Direction = FlexDirection.ColumnReverse;
                }

                SearchView.Orientation = "Horizontal";
                SecondaryStack.WidthRequest = -1;
                SecondaryStack.HeightRequest = 400;
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