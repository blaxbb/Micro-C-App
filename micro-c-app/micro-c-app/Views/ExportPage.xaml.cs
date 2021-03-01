using micro_c_app.ViewModels;
using MicroCLib.Models;
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
    public partial class ExportPage : ContentPage
    {
        public ExportPage()
        {
            InitializeComponent();
        }

        public ExportPage(List<BuildComponent> components)
        {
            InitializeComponent();
            if(BindingContext is ExportPageViewModel vm)
            {
                vm.Components = components;
            }
        }
    }
}