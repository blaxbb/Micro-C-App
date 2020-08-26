using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views.CollectionFile
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CollectionSavePage : ContentPage
    {
        public CollectionSavePage()
        {
            InitializeComponent();
            if (string.IsNullOrWhiteSpace(filenameEntry.Text))
            {
                filenameEntry.Focus();
            }
        }
    }
}