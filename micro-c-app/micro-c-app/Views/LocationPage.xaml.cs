using micro_c_app.ViewModels;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Shapes;
using Xamarin.Forms.Xaml;
using Path = Xamarin.Forms.Shapes.Path;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationPage : ContentPage
    {
        public LocationPage()
        {
            InitializeComponent();
            if (BindingContext is LocationPageViewModel vm)
            {
                vm.SearchMode = true;
                vm.PropertyChanged += Vm_PropertyChanged;
            }
            map.PropertyChanged += Map_PropertyChanged;
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (BindingContext is LocationPageViewModel vm && e.PropertyName == nameof(vm.Markers))
            {
                mapResults.ClearMarkers();
                var minSize = 5d;
                var maxSize = 30d;
                var deltaSize = (maxSize - minSize) / vm.Markers.Count;
                var size = maxSize;
                var ordered = vm.Markers.OrderByDescending(m => m.Created).ToList();
                var colors = new SolidColorBrush[] { SolidColorBrush.Green, SolidColorBrush.Yellow, SolidColorBrush.Red };
                for (int i = 0; i < ordered.Count; i++)
                {
                    var marker = ordered[i];
                    var percent = (float)i / ordered.Count;
                    var color = colors[(int)(percent * colors.Length)];

                    mapResults.AddMarker(new Point(marker.X, marker.Y), size, color, 5);
                    size -= deltaSize;
                }
            }
        }

        private void Map_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (BindingContext is LocationPageViewModel vm)
            {
                if (e.PropertyName == nameof(StoreMapControl.CursorPercent))
                {
                    vm.CursorPercent = map.CursorPercent;
                }
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            
            if(width > height)
            {
                grid.ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition(){ Width = GridLength.Star},
                    new ColumnDefinition(){ Width = GridLength.Auto},
                };

                grid.RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition() {Height = GridLength.Auto},
                    new RowDefinition() {Height = GridLength.Star},
                };

                //Grid.SetColumn(mapContainer, 0);
                //Grid.SetRow(mapContainer, 0);
                //Grid.SetRowSpan(mapContainer, 2);

                Grid.SetColumn(mapContainer, 0);
                Grid.SetRow(mapContainer, 0);
                Grid.SetRowSpan(mapContainer, 2);

                Grid.SetColumn(mapResultsContainer, 0);
                Grid.SetRow(mapResultsContainer, 0);
                Grid.SetRowSpan(mapResultsContainer, 2);

                Grid.SetColumn(modeStack, 1);
                Grid.SetRow(modeStack, 0);

                Grid.SetColumn(searchView, 1);
                Grid.SetRow(searchView, 1);

                searchView.Orientation = "Vertical";
            }
            else
            {
                grid.RowDefinitions = new RowDefinitionCollection()
                {
                    new RowDefinition(){ Height = GridLength.Star},
                    new RowDefinition(){ Height = GridLength.Auto},
                    new RowDefinition(){ Height = GridLength.Auto},
                };
                grid.ColumnDefinitions = new ColumnDefinitionCollection()
                {
                    new ColumnDefinition() {Width = GridLength.Star},
                };

                Grid.SetColumn(mapContainer, 0);
                Grid.SetRow(mapContainer, 0);
                Grid.SetRowSpan(mapContainer, 1);

                Grid.SetColumn(mapResultsContainer, 0);
                Grid.SetRow(mapResultsContainer, 0);
                Grid.SetRowSpan(mapResultsContainer, 1);

                Grid.SetRow(modeStack, 1);
                Grid.SetColumn(modeStack, 0);

                Grid.SetColumn(searchView, 0);
                Grid.SetRow(searchView, 2);
                searchView.Orientation = "Horizontal";
            }
        }
    }
}