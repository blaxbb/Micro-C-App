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
        private Point cursorPosition;
        private Point cursorPercent;
        public Point CursorPercent { get => cursorPercent; set { cursorPercent = value; OnPropertyChanged(nameof(CursorPercent)); } }
        public Point CursorPosition { get => cursorPosition; set { cursorPosition = value; OnPropertyChanged(nameof(CursorPosition)); cursor.TranslateTo(CursorPosition.X, CursorPosition.Y, 50); } }
        Point[] Points;

        public double x { get; set; } = 0;
        public double y { get; set; } = 0;
        public double scale { get; set; } = 1;


        public StoreMapControl()
        {
            BindingContext = this;
            InitializeComponent();

            MessagingCenter.Subscribe<SettingsPageViewModel>(this, SettingsPageViewModel.SETTINGS_UPDATED_MESSAGE, SettingsUpdated);
            UpdateMapImage();
            

            cursor.IsVisible = false;

            Points = new Point[]
            {
                new Point() { X = 0.564746775390276, Y = 0.311131858323938 },
                new Point() { X = 0.852397341485005, Y = 0.314126503194288 },
                new Point() { X = 0.855062795614092, Y = 0.658334507703871 },
                new Point() { X = 0.536912442927517, Y = 0.66132915257422 },
                new Point() { X = 0.564746775390276, Y = 0.311131858323938 }
            };

            //Gestures.SetTapped(image, new Command<Point>(p =>
            //{
            //    OnClick(p);
            //}));
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            //var delta = new Point((imageFrame.Width - image.Width) / 2, (imageFrame.Height - image.Height) / 2);
            //CursorPosition = new Point((CursorPercent.X * image.Width) + delta.X, (CursorPercent.Y * image.Height) + delta.Y);
            Debug.WriteLine($"CursorPercent {CursorPercent} CusrorPosition {CursorPosition}");

        }

        private void OnClick(Point p)
        {
            //cursor.IsVisible = true;

            //var delta = new Point((imageFrame.Width - image.Width) / 2, (imageFrame.Height - image.Height) / 2);

            //CursorPosition = new Point(p.X + delta.X - (cursor.Width / 2), p.Y + delta.Y - (cursor.Height / 2));

            //var x = p.X / image.Width;
            //var y = p.Y / image.Height;
            //CursorPercent = new Point(x, y);

            ////Debug.WriteLine($"CursorPercent {CursorPercent} CusrorPosition {CursorPosition} ClickedPosition {p}");
            //if (IsPointInPolygon(CursorPercent, Points) && false)
            //{
            //    image.Scale = 2;
            //    //image.TranslationX = -((image.Scale - 1) / 1) * (Points[0].X * (image.Width + delta.X));
            //    //image.TranslationY = -((image.Scale - 1) / 1) * (Points[1].Y * (image.Height + delta.Y));

            //    image.TranslateTo(-p.X, -p.Y);
            //    Debug.WriteLine("INSIDE");
            //}
            //else
            //{
            //    image.Scale = 1;
            //    image.TranslateTo(0, 0);
            //    Debug.WriteLine("OUTSIDE");
            //}
            //Debug.WriteLine($"new Point(){CursorPercent}");
        }

        public void UpdateMapImage()
        {
            image.Source = ImageSource.FromStream(() => LocationEntry.GetMapImageStream(SettingsPage.StoreID(), SettingsPage.LocatorCookie()));
        }

        private void SettingsUpdated(SettingsPageViewModel obj)
        {
            UpdateMapImage();
        }

        bool DEV_ENABLED = false;

        private void DevButton(object sender, EventArgs e)
        {
            DEV_ENABLED = !DEV_ENABLED;
            Debug.WriteLine($"DEV {(DEV_ENABLED ? "ENABLED" : "DISABLED")}");
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