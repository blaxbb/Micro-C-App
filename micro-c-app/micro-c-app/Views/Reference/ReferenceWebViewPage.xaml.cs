using micro_c_app.ViewModels.Reference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views.Reference
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReferenceWebViewPage : ContentPage
    {
        public ReferenceWebViewPage()
        {
            InitializeComponent();
            if(BindingContext is ReferenceWebViewPageViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
                if (!string.IsNullOrWhiteSpace(vm.Text))
                {
                    UpdateWebView();
                }
            }
        }

        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ReferenceWebViewPageViewModel.Text))
            {
                UpdateWebView();
            }
        }

        private void UpdateWebView()
        {
            if (BindingContext is ReferenceWebViewPageViewModel vm)
            {
                webView.Source = new HtmlWebViewSource() { Html = vm.Text };
            }
        }
    }
}