using MicroCLib.Models;
using System;
using System.Collections.Generic;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace MicroCBuilder.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>

    public sealed partial class BarcodePrintView : UserControl, INotifyPropertyChanged
    {
        public BarcodePrintView()
        {
            this.DataContext = this;
            this.InitializeComponent();
            printGrid.DataContextChanged += PrintGrid_DataContextChanged;
        }

        public void SetImages(string sku, string serial = "cereal")
        {
            var writer = new ZXing.Mobile.BarcodeWriter
            {
                Format = ZXing.BarcodeFormat.CODE_128,
                Options = new ZXing.Common.EncodingOptions
                {
                    Height = 200,
                    Width = 200 * serial.Length,
                },
                Renderer = new ZXing.Mobile.WriteableBitmapRenderer() { Foreground = Windows.UI.Colors.Black }
            };
            if (!string.IsNullOrWhiteSpace(serial))
            {
                SerialImage.Source = writer.Write(serial);
                SerialText.Text = serial;
            }

            writer.Options.Width = 1200;
            if (!string.IsNullOrWhiteSpace(sku))
            {
                SkuImage.Source = writer.Write(sku);
                SkuText.Text = sku;
            }
        }

        private void PrintGrid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            Console.WriteLine("Ctx");
            if(printGrid.DataContext is BuildComponent comp)
            {
                Console.WriteLine("Comp");
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
