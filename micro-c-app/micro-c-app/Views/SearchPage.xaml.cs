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
                Device.InvokeOnMainThreadAsync(async () =>
                {
                    searchView.OnSubmit(value);
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
            if(width > height)
            {
                Grid.SetRow(hintGrid, 0);
                Grid.SetRow(detailView, 1);
                Grid.SetRow(emptyView, 1);
                Grid.SetRow(searchView, 0);

                Grid.SetRowSpan(searchView, 2);

                Grid.SetColumn(hintGrid, 0);
                Grid.SetColumn(detailView, 0);
                Grid.SetColumn(emptyView, 0);
                Grid.SetColumn(searchView, 1);

                searchView.Orientation = "Vertical";
                grid.RowDefinitions[3].Height = 0;
            }
            else
            {
                Grid.SetRow(hintGrid, 0);
                Grid.SetRow(detailView, 1);
                Grid.SetRow(emptyView, 1);
                Grid.SetRow(searchView, 2);

                Grid.SetColumn(hintGrid, 0);
                Grid.SetColumn(detailView, 0);
                Grid.SetColumn(emptyView, 0);
                Grid.SetColumn(searchView, 0);
                searchView.Orientation = "Horizontal";
                grid.RowDefinitions[3].Height = new GridLength(1, GridUnitType.Auto);
            }
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