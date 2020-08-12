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
    public partial class QuotePage : ContentPage
    {
        public QuotePage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (width > height)
            {
                FlipStack.Orientation = StackOrientation.Horizontal;
                SecondaryStack.VerticalOptions = LayoutOptions.FillAndExpand;
                SearchView.Orientation = "Vertical";
            }
            else
            {
                FlipStack.Orientation = StackOrientation.Vertical;
                SecondaryStack.VerticalOptions = LayoutOptions.End;
                SearchView.Orientation = "Horizontal";
            }
        }
    }
}