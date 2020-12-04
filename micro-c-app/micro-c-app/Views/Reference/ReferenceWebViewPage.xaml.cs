﻿using micro_c_app.ViewModels.Reference;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        }

        private void WebView_Navigating(object sender, WebNavigatingEventArgs e)
        {
            e.Cancel = true;
            Debug.WriteLine(e.Url);

            var match = Regex.Match(e.Url, "file:(?:.*?)/(search|reference)=(.*)?");
            if (match.Success)
            {
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
                    default:
                        //command names must be added to regex above
                        Debug.WriteLine($"Error: command {command} not found");
                        break;
                }
            }

            Task.Run(async() => await Xamarin.Essentials.Browser.OpenAsync(e.Url));
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
                string html = $"<meta name='viewport' content='width=device-width,height=device-height'>" +
                    $"<div id='content'></div>" +
                    $"<link rel=\"stylesheet\" href=\"https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css\" integrity=\"sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm\" crossorigin=\"anonymous\">" +
                    $"<script src=\"https://cdn.jsdelivr.net/npm/marked/marked.min.js\"></script>" +
                    $"<script>document.getElementById('content').innerHTML = marked(`{escaped}`);</script>" +
                    $"<script>var table = document.getElementsByTagName('table'); Array.from(table).forEach(function(t){{ t.classList.add('table-sm');t.classList.add('table-striped')}});</script>" +
                    $"<script>var table = document.getElementsByTagName('blockquote'); Array.from(table).forEach(function(t){{ t.classList.add('blockquote'); }});</script>" +
                    $"<style>img {{ max-width: 100% }}</style>";

                if(SettingsPage.Theme() == OSAppTheme.Dark || (SettingsPage.Theme() == OSAppTheme.Unspecified && Application.Current.RequestedTheme == OSAppTheme.Dark))
                {
                    html += $"<link rel=\"stylesheet\" href=\"https://bootswatch.com/4/darkly/bootstrap.min.css\" crossorigin=\"anonymous\">";
                }

                webView.Source = new HtmlWebViewSource()
                {
                    Html = html
                };
            }
        }
    }
}