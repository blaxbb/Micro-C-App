using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ExportQRPage : ContentPage
    {
        public ExportQRPage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if(width > height)
            {
                FlipStack.Orientation = StackOrientation.Horizontal;
            }
            else
            {
                FlipStack.Orientation = StackOrientation.Vertical;
            }
        }
    }
}