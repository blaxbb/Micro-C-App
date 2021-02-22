using micro_c_app.ViewModels;
using MicroCLib.Models;
using MicroCLib.Models.Reference;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MicroCLib.Models.Search;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuildComponentPage : ContentPage
    {
        public BuildComponentPage()
        {
            InitializeComponent();
            this.SetupActionButton();
        }

        public void Setup()
        {
            if(BindingContext is BuildComponentViewModel vm && vm.Component != null)
            {
                if(vm.Component.AutoSearch() && vm.Component.Item == null)
                {
                    SearchView.OrderBy = OrderByMode.pricelow;
                    Task.Run(() => SearchView.OnSubmit(""));
                }
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            return;

            //if (Device.RuntimePlatform == "UWP")
            //{
            //    //xamarin bug, shell does not pass correct width and height to contained pages on UWP
            //    width = App.Current.MainPage.Width;
            //    height = App.Current.MainPage.Height;
            //}
            //if (width > height)
            //{
            //    //FlipStack.Orientation = StackOrientation.Horizontal;
            //    if(grid.RowDefinitions.Count > 1 && grid.ColumnDefinitions.Count > 0)
            //    {
            //        grid.RowDefinitions[1].Height = 0;
            //        grid.ColumnDefinitions[1].Width = GridLength.Star;
            //    }

            //    grid.RowSpacing = 0;
            //    grid.ColumnSpacing = 20;

            //    Grid.SetRow(ItemInfo, 0);
            //    Grid.SetColumn(ItemInfo, 1);
            //    //SearchView.Orientation = "Vertical";
            //}
            //else
            //{
            //    //FlipStack.Orientation = StackOrientation.Vertical;
            //    if (grid.RowDefinitions.Count > 1 && grid.ColumnDefinitions.Count > 0)
            //    {
            //        grid.RowDefinitions[1].Height = new GridLength(2.25, GridUnitType.Star);
            //        grid.ColumnDefinitions[1].Width = 0;
            //    }
            //    grid.RowSpacing = 20;
            //    grid.ColumnSpacing = 0;
            //    Grid.SetRow(ItemInfo, 1);
            //    Grid.SetColumn(ItemInfo, 0);
            //    //SearchView.Orientation = "Horizontal";
            //}
        }
    }
}