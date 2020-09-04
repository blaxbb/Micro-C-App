
using micro_c_app.ViewModels;
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
            if (width > height)
            {
                FlipStack.Orientation = StackOrientation.Horizontal;
                SecondaryStack.VerticalOptions = LayoutOptions.FillAndExpand;
                SearchView.Orientation = "Vertical";
            }
            else
            {
                FlipStack.Orientation = StackOrientation.Vertical;
                SecondaryStack.VerticalOptions = LayoutOptions.End;
                SearchView.Orientation = "Horizontal";
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

        Models.Item previousSelection;
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
                previousSelection = newItem as Models.Item;
            }
        }
    }
}