﻿using micro_c_app.ViewModels.Reference;
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
using static MicroCLib.Models.Reference.PlanReference;

namespace micro_c_app.Views.Reference
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ReferenceWebViewPage : ContentPage
    {
        private string contentMarkdown;
        private string footerMarkdown;
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
            await webView.EvaluateJavaScriptAsync($"HandleMD(`{contentMarkdown}`, 'content');");
            await webView.EvaluateJavaScriptAsync($"HandleMD(`{footerMarkdown}`, 'footer');");
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
                        if(Enum.TryParse<PlanType>(argument, out PlanType planType))
                        {
                            ReferenceIndexPage.NavigateTo(planType);
                        }
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

                //escape backtick for js string literal
                contentMarkdown = vm.Text.Replace("`", "\\`");
                //escape # for js comment
                contentMarkdown = contentMarkdown.Replace("#", "\\#");

                //there is a escape character before #footer because we added one above
                var reg = "\\[(.*?)\\]\\(\\\\#footer\\)";
                var match = Regex.Match(contentMarkdown, reg);
                if (match.Success)
                {
                    footerMarkdown = match.Groups[1].Value;
                }

                //iOS really doesn't like newlines in js
                contentMarkdown = Regex.Replace(contentMarkdown, @"\r\n?|\n", "\\n");


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