using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using static MicroCLib.Models.BuildComponent;

namespace micro_c_app.Views.InventoryAudit
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ZeroStockPage : ContentPage
    {
        public ZeroStockPage()
        {
            InitializeComponent();
        }
    }
}