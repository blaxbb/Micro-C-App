using micro_c_app.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace micro_c_app.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BuildPage : ContentPage
    {
        BuildViewModel viewModel => (BuildViewModel)BindingContext;
        HttpClient client;
        public BuildPage()
        {

            //if(viewModel == null)
            //{
            //    BindingContext = new BuildViewModel();
            //}
            InitializeComponent();
            viewModel.Navigation = Navigation;
            viewModel.ConfigID = "CONFIG_ID";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            client = new HttpClient();
            if (string.IsNullOrWhiteSpace(viewModel.ConfigID))
            {
                Task.Run(async () =>
                {
                    //viewModel.ConfigID = await GetBuildID();
                });
            }
        }

        protected override void OnDisappearing()
        {
            client?.Dispose();
            base.OnDisappearing();
        }

        private async Task<string> GetBuildID()
        {
            if(client == null)
            {
                return default;
            }

            var response = await client.GetAsync("https://www.microcenter.com/site/content/custom-pc-builder.aspx");
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return default;
            }

            var cookies = response.Headers.GetValues("Set-Cookie");
            var configurator = GetCookieValue(cookies, "configuratorCookie");
            if (!string.IsNullOrWhiteSpace(configurator))
            {
                return GetCookieValue(cookies, $"configuratorGUID{configurator}");
            }
            return default;
        }

        private static string GetCookieValue(IEnumerable<string> cookieStrings, string cookieName)
        {
            foreach(var s in cookieStrings)
            {
                var val = GetCookieValue(s, cookieName);
                if (!string.IsNullOrWhiteSpace(val))
                {
                    return val;
                }
            }

            return "";
        }
        private static string GetCookieValue(string cookieString, string cookieName)
        {
            var match = Regex.Match(cookieString, $"{cookieName}=(.*?);");
            if(match.Success)
            {
                return match.Groups[1].Value;
            }

            return "";
        }
    }
}