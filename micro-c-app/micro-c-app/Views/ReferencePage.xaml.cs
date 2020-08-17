using micro_c_app.ViewModels;
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
    public partial class ReferencePage : ContentPage
    {
        public ReferencePage()
        {
            InitializeComponent();
        }

        private void ListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if(BindingContext is ReferencePageViewModel vm)
            {
                vm.ItemTapped?.Execute(null);
            }
        }
    }
}