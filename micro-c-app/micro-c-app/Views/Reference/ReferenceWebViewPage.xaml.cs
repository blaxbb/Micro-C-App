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
                //webView.Source = new HtmlWebViewSource() { Html = vm.Text };
                var escaped = vm.Text.Replace("`", "\\`");
                webView.Source = new HtmlWebViewSource()
                {
                    Html = $"<meta name='viewport' content='width=device-width,height=device-height'>" +
                    $"<div id='content'></div>" +
                    $"<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css\" integrity=\"sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm\" crossorigin=\"anonymous\">" +
                    $"<link rel=\"stylesheet\" href=\"https://bootswatch.com/4/darkly/bootstrap.min.css\" crossorigin=\"anonymous\">" +
                    $"<script src=\"https://cdn.jsdelivr.net/npm/marked/marked.min.js\"></script>" +
                    $"<script>document.getElementById('content').innerHTML = marked(`{escaped}`);</script>" +
                    $"<script>var table = document.getElementsByTagName('table'); Array.from(table).forEach(function(t){{ t.classList.add('table-sm');t.classList.add('table-striped')}});</script>" +
                    $"<script>var table = document.getElementsByTagName('blockquote'); Array.from(table).forEach(function(t){{ t.classList.add('blockquote'); }});</script>" +
                    $"<style>img {{ max-width: 100% }}</style>"
                };
            }
        }
    }
}