using micro_c_app.ViewModels;
using System.Net.Http;
using Xamarin.Forms;

namespace micro_c_app.Views
{
    [QueryProperty(nameof(SearchRoute), "search")]
    public partial class SearchPage : ContentPage
    {
        HttpClient client;
        public string SearchRoute {
            set
            {
                Device.InvokeOnMainThreadAsync(() =>
                {
                    searchView?.OnSubmit(value);
                });
            }
        }
        public SearchPage()
        {
            client = new HttpClient();
            InitializeComponent();

            if (BindingContext is SearchViewModel vm)
            {
                vm.Navigation = Navigation;
            }
            KeyboardHelper.KeyboardChanged += KeyboardHelper_KeyboardChanged;
            this.SetupActionButton();
        }

        private void KeyboardHelper_KeyboardChanged(object sender, KeyboardHelperEventArgs e)
        {
            spacer.HeightRequest = e.Visible ? e.Height : 0;
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height)
            {
                //
                //page grid contents
                //

                Grid.SetRow(hintGrid, 0);
                Grid.SetColumn(hintGrid, 0);

                Grid.SetRow(emptyView, 1);
                Grid.SetColumn(emptyView, 0);

                Grid.SetRow(productView, 1);
                Grid.SetColumn(productView, 0);

                Grid.SetRow(searchGrid, 0);
                Grid.SetColumn(searchGrid, 1);
                Grid.SetRowSpan(searchGrid, 2);

                Grid.SetRow(spacer, 2);
                Grid.SetColumn(spacer, 0);
                Grid.SetColumnSpan(spacer, 2);

                searchView.Orientation = "Vertical";

                searchGrid.HeightRequest = -1;

                grid.RowDefinitions[3].Height = 0;
                grid.ColumnDefinitions[1].Width = GridLength.Star;
                //
                // search grid contents
                //

                searchGrid.ColumnDefinitions[0].Width = GridLength.Star;
                searchGrid.ColumnDefinitions[1].Width = GridLength.Auto;

                searchGrid.RowDefinitions[0].Height = GridLength.Auto;
                searchGrid.RowDefinitions[1].Height = GridLength.Star;

                Grid.SetRow(homeButton, 0);
                Grid.SetColumn(homeButton, 0);
                homeButton.HorizontalOptions = LayoutOptions.End;

                Grid.SetRow(backButton, 0);
                Grid.SetColumn(backButton, 1);

                Grid.SetColumn(searchView, 0);
                Grid.SetRow(searchView, 1);
                Grid.SetColumnSpan(searchView, 2);

            }
            else
            {
                //
                //page grid contents
                //

                Grid.SetRow(hintGrid, 0);
                Grid.SetColumn(hintGrid, 0);

                Grid.SetRow(emptyView, 1);
                Grid.SetColumn(emptyView, 0);

                Grid.SetRow(productView, 1);
                Grid.SetColumn(productView, 0);

                Grid.SetRow(searchGrid, 2);
                Grid.SetColumn(searchGrid, 0);
                Grid.SetRowSpan(searchGrid, 1);

                Grid.SetRow(spacer, 3);
                Grid.SetColumn(spacer, 0);
                Grid.SetColumnSpan(spacer, 1);

                searchView.Orientation = "Horizontal";

                searchGrid.HeightRequest = 125;

                grid.RowDefinitions[3].Height = GridLength.Auto;
                grid.ColumnDefinitions[1].Width = 0;


                //
                // search grid contents
                //

                searchGrid.ColumnDefinitions[0].Width = GridLength.Auto;
                searchGrid.ColumnDefinitions[1].Width = GridLength.Star;

                searchGrid.RowDefinitions[0].Height = GridLength.Star;
                searchGrid.RowDefinitions[1].Height = GridLength.Star;

                Grid.SetRow(homeButton, 0);
                Grid.SetColumn(homeButton, 0);
                homeButton.HorizontalOptions = LayoutOptions.Center;

                Grid.SetRow(backButton, 1);
                Grid.SetColumn(backButton, 0);

                Grid.SetColumn(searchView, 1);
                Grid.SetRow(searchView, 0);
                Grid.SetRowSpan(searchView, 2);
            }

            InvalidateMeasure();
        }

        protected override bool OnBackButtonPressed()
        {
            if(BindingContext is SearchViewModel vm)
            {
                if (vm.Item != null)
                {
                    if (!vm.DoPopItem())
                    {
                    }
                    return true;
                }
            }
            return false;
        }
    }
}