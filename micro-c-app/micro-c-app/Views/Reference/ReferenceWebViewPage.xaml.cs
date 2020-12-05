using micro_c_app.ViewModels.Reference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views.Reference
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReferenceWebViewPage : ContentPage
    {
        private string markdown;
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
            webView.Navigating += WebView_Navigating;
            webView.Navigated += WebView_Navigated;
        }

        private async void WebView_Navigated(object sender, WebNavigatedEventArgs e)
        {
            var sub = markdown.Substring(0, markdown.Length / 4);
            var js = $"HandleMD(`{markdown}`);";
            await webView.EvaluateJavaScriptAsync(js);
            if (SettingsPage.Theme() == OSAppTheme.Dark || (SettingsPage.Theme() == OSAppTheme.Unspecified && Application.Current.RequestedTheme == OSAppTheme.Dark))
            {
                await webView.EvaluateJavaScriptAsync("var css = document.createElement('link'); css.href='darkly.bootstrap.min.css'; css.type = 'text/css'; css.rel='stylesheet'; document.getElementsByTagName('head')[0].appendChild(css);");
            }
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            Debug.WriteLine(e.Url);

            var match = Regex.Match(e.Url, "file:(?:.*?)/(search|reference|plan)=(.*)?");
            if (match.Success)
            {
                e.Cancel = true;
                var command = match.Groups[1].Value.ToLower();
                var argument = match.Groups[2].Value;
                argument = Uri.UnescapeDataString(argument);

                switch (command)
                {
                    case "search":
                        Shell.Current.GoToAsync($"//SearchPage?search={argument}");
                        break;
                    case "reference":
                        ReferenceIndexPage.NavigateTo(argument);
                        break;
                    case "plan":
                        break;
                    default:
                        //command names must be added to regex above
                        Debug.WriteLine($"Error: command {command} not found");
                        break;
                }
            }
            if(!e.Url.StartsWith("file:"))
            {
                Task.Run(async () => await Xamarin.Essentials.Browser.OpenAsync(e.Url, Xamarin.Essentials.BrowserLaunchMode.External));
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
                markdown = vm.Text.Replace("`", "\\`");
                markdown = markdown.Replace("#", "\\#");
                markdown = Regex.Replace(markdown, @"\r\n?|\n", "\\n");
                
                var baseUrl = DependencyService.Get<IBaseUrl>()?.Get ?? "/";
                var p = Path.Combine(baseUrl, "Content/reference.html");
                webView.Source = new UrlWebViewSource()
                {
                    Url = p
                };
            }
        }
    }
}