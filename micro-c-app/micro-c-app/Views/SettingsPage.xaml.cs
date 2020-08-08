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
    public partial class SettingsPage : ContentPage
    {
        public const string PREF_SELECTED_STORE = "selected_store";
        public const string PREF_SALES_ID = "sales_id";
        public const string PREF_TAX_RATE = "tax_rate";
        public SettingsPage()
        {
            InitializeComponent();
        }
    }
}