using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using micro_c_app;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Xamarin.Forms.Shapes;
using micro_c_app.ViewModels;
using micro_c_app.Models;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreMapResultsControl : ContentView, INotifyPropertyChanged
    {
        public StoreMapResultsControl()
        {
            BindingContext = this;
            InitializeComponent();
            UpdateMapImage();
        }
        public void UpdateMapImage()
        {
            image.Source = ImageSource.FromStream(() => LocationEntry.GetMapImageStream(SettingsPage.StoreID(), SettingsPage.LocatorCookie()));
        }

        public void AddMarker(Point percent, double size, SolidColorBrush color, double thickness)
        {
            var delta = new Point((imageFrame.Width - image.Width) / 2, (imageFrame.Height - image.Height) / 2);

            var pos = new Point((percent.X * image.Width) + delta.X - (size / 2), (percent.Y * image.Height) + delta.Y - (size / 2));

            var shape = new Xamarin.Forms.Shapes.Ellipse() { WidthRequest = size, HeightRequest = size, Stroke = color, StrokeThickness = thickness, HorizontalOptions = LayoutOptions.Start, VerticalOptions = LayoutOptions.Start };
            shape.TranslateTo(pos.X, pos.Y);
            grid.Children.Add(shape);
            Grid.SetRow(shape, 0);
        }

        public void ClearMarkers()
        {
            foreach(var child in grid.Children.ToList())
            {
                if(child is Ellipse)
                {
                    grid.Children.Remove(child);
                }
            }
        }
    }
}