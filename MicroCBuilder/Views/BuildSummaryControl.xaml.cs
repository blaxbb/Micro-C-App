using MicroCLib.Models;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace MicroCBuilder.Views
{
    public sealed partial class BuildSummaryControl : UserControl, INotifyPropertyChanged
    {
        public float SubTotal
        {
            get { return (float)GetValue(SubTotalProperty); }
            set { SetValue(SubTotalProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubTotal.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubTotalProperty =
            DependencyProperty.Register("SubTotal", typeof(float), typeof(BuildSummaryControl), new PropertyMetadata(0f, new PropertyChangedCallback(PropChanged)));
        public double TaxRate { get; set; }

        public double TaxAmt => SubTotal * TaxRate;
        public double Total => SubTotal * (1 + TaxRate);
        public double CCDiscount => Total * .05f;
        public double CCTotal => Total * .95f;
        public string MCOLUrl
        {
            get { return (string)GetValue(MCOLUrlProperty); }
            set { SetValue(MCOLUrlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MCOLUrl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MCOLUrlProperty =
            DependencyProperty.Register("MCOLUrl", typeof(string), typeof(BuildSummaryControl), new PropertyMetadata("", new PropertyChangedCallback(QrPropChanged)));
        private WriteableBitmap bitmap;

        public WriteableBitmap Bitmap { get => bitmap; set { bitmap = value; OnPropertyChanged(nameof(Bitmap)); } }


        private static void PropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BuildSummaryControl control)
            {
                control.UpdateProperties();
            }
        }

        private static async void QrPropChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is BuildSummaryControl control)
            {
                await control.CreateQrCode();
            }
        }

        public BuildSummaryControl()
        {
            SettingsUpdated("", 0);
            this.InitializeComponent();
            DataContext = this;
            Settings.SettingsUpdated += SettingsUpdated;
        }

        private void SettingsUpdated(string key, object value)
        {
            TaxRate = Settings.TaxRate() / 100;
            UpdateProperties();
        }

        private void UpdateProperties()
        {
            OnPropertyChanged(nameof(TaxRate));
            OnPropertyChanged(nameof(TaxAmt));
            OnPropertyChanged(nameof(Total));
            OnPropertyChanged(nameof(CCDiscount));
            OnPropertyChanged(nameof(CCTotal));
            OnPropertyChanged(nameof(SubTotal));
        }

        public async Task CreateQrCode()
        {
            if (string.IsNullOrWhiteSpace(MCOLUrl))
            {
                return;
            }

            var qrGen = new QRCodeGenerator();
            var qrData = qrGen.CreateQrCode(MCOLUrl, QRCodeGenerator.ECCLevel.H);
            var code = new BitmapByteQRCode(qrData);
            var graphic = code.GetGraphic(20);
            using (var stream = new InMemoryRandomAccessStream())
            {
                using (var writer = new DataWriter(stream.GetOutputStreamAt(0)))
                {
                    writer.WriteBytes(graphic);
                    await writer.StoreAsync();
                }
                Bitmap = new WriteableBitmap(1024, 1024);
                await Bitmap.SetSourceAsync(stream);
            }
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
