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
using micro_c_app.Models;
using micro_c_app.ViewModels;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StoreMapControl : ContentView, INotifyPropertyChanged
    {
        public static readonly BindableProperty PanPercentageProperty = BindableProperty.Create(nameof(PanPercentage), typeof(Point), typeof(StoreMapControl));
        public Point PanPercentage { get => (Point)GetValue(PanPercentageProperty); set => SetValue(PanPercentageProperty, value); }

        public StoreMapControl()
        {
            BindingContext = this;
            InitializeComponent();

            UpdateMapImage();
            PropertyChanged += StoreMapControl_PropertyChanged;
        }

        private void StoreMapControl_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(PanPercentage))
            {
                //Debug.WriteLine(PanPercentage);
            }
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
        }

        public void UpdateMapImage()
        {
            image.Source = ImageSource.FromStream(() => LocationEntry.GetMapImageStream(SettingsPage.StoreID(), SettingsPage.LocatorCookie()));
        }

        //
        //https://stackoverflow.com/a/16391873
        //
        public bool IsPointInPolygon(Point p, Point[] polygon)
        {
            double minX = polygon[0].X;
            double maxX = polygon[0].X;
            double minY = polygon[0].Y;
            double maxY = polygon[0].Y;
            for (int i = 1; i < polygon.Length; i++)
            {
                Point q = polygon[i];
                minX = Math.Min(q.X, minX);
                maxX = Math.Max(q.X, maxX);
                minY = Math.Min(q.Y, minY);
                maxY = Math.Max(q.Y, maxY);
            }

            if (p.X < minX || p.X > maxX || p.Y < minY || p.Y > maxY)
            {
                return false;
            }

            // https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
            bool inside = false;
            for (int i = 0, j = polygon.Length - 1; i < polygon.Length; j = i++)
            {
                if ((polygon[i].Y > p.Y) != (polygon[j].Y > p.Y) &&
                     p.X < (polygon[j].X - polygon[i].X) * (p.Y - polygon[i].Y) / (polygon[j].Y - polygon[i].Y) + polygon[i].X)
                {
                    inside = !inside;
                }
            }

            return inside;
        }
    }
}