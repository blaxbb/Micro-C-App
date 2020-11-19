using micro_c_app.Models;
using micro_c_app.Views;
using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class LocationPageViewModel : BaseViewModel
    {
        private Point clickPosition;
        private bool searchMode;
        private Point cursorPosition;
        private Point cursorPercent;
        private List<LocationEntry> markers;

        public ICommand Tapped { get; }
        public Point ClickPosition { get => clickPosition; set => SetProperty(ref clickPosition, value); }

        public Point CursorPosition { get => cursorPosition; set => SetProperty(ref cursorPosition, value); }

        //
        //set in LocaitonPage.xaml.cs
        public Point CursorPercent { get => cursorPercent; set => SetProperty(ref cursorPercent, value); }
        //
        //accessed in LocationPage.xaml.cs
        public List<LocationEntry> Markers { get => markers; set => SetProperty(ref markers, value); }

        public bool SearchMode { get => searchMode; set { SetProperty(ref searchMode, value); OnPropertyChanged(nameof(ScanMode)); } }
        public bool ScanMode => !SearchMode;

        public ICommand DoScanMode { get; }
        public ICommand DoSearchMode { get; }
        public ICommand ProductFound { get; }

        public LocationPageViewModel()
        {
            DoScanMode = new Command(() =>
            {
                SearchMode = false;
            });

            DoSearchMode = new Command(() =>
            {
                Markers = new List<LocationEntry>();
                SearchMode = true;
            });

            Tapped = new Command<Point>(p =>
            {
                ClickPosition = p;
                Debug.WriteLine($"{p.X} - {p.Y}");
            });
            ProductFound = new Command<Item>(async (item) => await DoProductFound(item));
        }

        private async Task DoProductFound(Item item)
        {
            if (SearchMode)
            {
                await DoProductSearch(item);
            }
            else
            {
                await DoProductScan(item);
            }
        }

        private async Task DoProductScan(Item item)
        {
            var entry = new LocationEntry(SettingsPage.StoreID(), item.SKU, CursorPercent.X, CursorPercent.Y);
            await entry.Post(SettingsPage.LocatorCookie());
        }

        private async Task DoProductSearch(Item item)
        {
            Markers = await LocationEntry.Search(item.SKU, SettingsPage.StoreID(), SettingsPage.LocatorCookie());

        }
    }
}
