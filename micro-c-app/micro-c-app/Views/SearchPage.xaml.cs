using micro_c_app.ViewModels;
using System.Net.Http;
using Xamarin.Forms;

namespace micro_c_app.Views
{
    public partial class SearchPage : ContentPage
    {
        HttpClient client;
        public SearchPage()
        {
            client = new HttpClient();
            InitializeComponent();

            if (BindingContext is SearchViewModel vm)
            {
                vm.Navigation = Navigation;
            }
            KeyboardHelper.KeyboardChanged += KeyboardHelper_KeyboardChanged;
        }

        private void KeyboardHelper_KeyboardChanged(object sender, KeyboardHelperEventArgs e)
        {
            spacer.HeightRequest = e.Visible ? e.Height : 0;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if(width > height)
            {
                Grid.SetRow(detailView, 0);
                Grid.SetRow(searchView, 0);

                Grid.SetColumn(detailView, 0);
                Grid.SetColumn(searchView, 1);
                searchView.Orientation = "Vertical";
                grid.RowDefinitions[2].Height = 0;
            }
            else
            {
                Grid.SetRow(detailView, 0);
                Grid.SetRow(searchView, 1);

                Grid.SetColumn(detailView, 0);
                Grid.SetColumn(searchView, 0);
                searchView.Orientation = "Horizontal";
                grid.RowDefinitions[2].Height = new GridLength(1, GridUnitType.Auto);
            }
        }
    }
}