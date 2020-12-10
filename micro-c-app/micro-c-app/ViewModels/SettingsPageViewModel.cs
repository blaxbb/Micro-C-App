using micro_c_app.Views;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class SettingsPageViewModel : BaseViewModel
    {
        public List<string> StoreNames { get; set; }
        public string SelectedStoreName { get; set; }

        public static Dictionary<string, string> Stores { get; private set; }
        public string StoreID { get; set; }
        public string SalesID { get; set; }
        public float TaxRate { get; set; }
        public bool IncludeCSVWithQuote { get; set; }

        public List<OSAppTheme> ThemeOptions { get; set; }
        public OSAppTheme Theme { get; set; }

        public bool Vibrate { get; set; }

        public string LocatorUsername { get; set; }
        public string LocatorPassword { get; set; }
        public string LocatorCookie { get => locatorCookie; set => SetProperty(ref locatorCookie, value); }



        public ICommand Save { get; }
        public ICommand Cancel { get; }
        public ICommand LocatorLogin { get; }
        public ICommand LocatorLogout { get; }
        public ICommand LocatorRegister { get; }

        public const string SETTINGS_UPDATED_MESSAGE = "updated";
        private string locatorCookie;

        public SettingsPageViewModel()
        {
            Save = new Command(async (o) => await DoSave(o));
            Cancel = new Command(async () => await ExitSettings());
            LocatorLogin = new Command(async () => await DoLogin());
            LocatorLogout = new Command(async () => await DoLogout());

            Title = "Settings";
            StoreID = SettingsPage.StoreID();
            SalesID = SettingsPage.SalesID();
            TaxRate = SettingsPage.TaxRate();
            IncludeCSVWithQuote = SettingsPage.IncludeCSVWithQuote();

            ThemeOptions = System.Enum.GetValues(typeof(OSAppTheme)).Cast<OSAppTheme>().ToList();
            Theme = SettingsPage.Theme();

            Stores = micro_c_lib.Models.Stores.AllStores;

            StoreNames = Stores.Keys.ToList();
            SelectedStoreName = Stores.FirstOrDefault(kvp => kvp.Value == StoreID).Key;

            Vibrate = SettingsPage.Vibrate();
            LocatorCookie = SettingsPage.LocatorCookie();

            LocatorRegister = new Command(() => { Xamarin.Essentials.Launcher.OpenAsync($"{ SettingsPage.LOCATOR_BASE_URL}Identity/Account/Register"); });

        }

        struct LoginPacket
        {
            [JsonPropertyName("Input.Email")]
            public string UserName { get; set; }
            [JsonPropertyName("Input.Password")]
            public string Password { get; set; }
            [JsonPropertyName("__RequestVerificationToken")]
            public string __RequestVerificationToken { get; set; }
            [JsonPropertyName("Input.RememberMe")]
            public bool RememberMe { get; set; }

            public LoginPacket(string userName, string password, string token)
            {
                UserName = userName;
                Password = password;
                __RequestVerificationToken = token;
                RememberMe = true;
            }
        }

        private async Task DoLogin()
        {
            async void DoError(string error)
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    await Shell.Current.DisplayAlert("ERROR", error, "Ok");
                });
            }

            Debug.WriteLine($"{LocatorUsername} - {LocatorPassword}");
            try
            {
                var cookies = new CookieContainer();
                using (HttpClientHandler handler = new HttpClientHandler())
                {
                    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
                    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
                    {
                        return true;
                    };
                    handler.CookieContainer = cookies;
                    handler.UseCookies = true;

                    using (HttpClient client = new HttpClient(handler))
                    {
                        var baseUrl = SettingsPage.LOCATOR_BASE_URL;

                        var getResult = await client.GetAsync($"{baseUrl}Identity/Account/Login");
                        if (!getResult.IsSuccessStatusCode)
                        {
                            DoError("Failed to connect to locator server");
                            return;
                        }

                        var textResult = await getResult.Content.ReadAsStringAsync();
                        var match = Regex.Match(textResult, "__RequestVerificationToken.*?value=\"(.*?)\"");

                        if (!match.Success)
                        {
                            DoError("Failed to grab verification token");
                            return;
                        }
                        var token = match.Groups[1].Value;

                        var obj = new LoginPacket(LocatorUsername, LocatorPassword, token);
                        var json = JsonSerializer.Serialize(obj);
                        var content = new StringContent(json, Encoding.UTF8, "application/json");
                        var ct = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>()
                        {
                            new KeyValuePair<string, string>("Input.Password", LocatorPassword),
                            new KeyValuePair<string, string>("Input.Email", LocatorUsername),
                            new KeyValuePair<string, string>("Input.RememberMe", "True"),
                            new KeyValuePair<string, string>("__RequestVerificationToken", token),
                        });
                        var str = ct.ToString();
                        var result = await client.PostAsync($"{baseUrl}Identity/Account/Login", ct);
                        if (result.IsSuccessStatusCode)
                        {
                            var response = await result.Content.ReadAsStringAsync();
                            var siteCookies = cookies.GetCookies(new Uri(baseUrl));
                            if (siteCookies != null)
                            {
                                var appCookie = siteCookies[".AspNetCore.Identity.Application"];
                                if (!string.IsNullOrWhiteSpace(appCookie?.Value))
                                {
                                    LocatorCookie = appCookie.Value;
                                    SettingsPage.LocatorCookie(appCookie.Value);
                                }
                                foreach(Cookie cookie in siteCookies)
                                {
                                    Debug.WriteLine(cookie.Name);
                                }
                            }

                            Debug.WriteLine("Success");
                        }
                        else
                        {
                            Debug.WriteLine("Failure");
                        }
                    }
                }
            }
            catch(Exception e)
            {
                DoError(e.Message);
            }
            MessagingCenter.Send(this, SETTINGS_UPDATED_MESSAGE);
        }

        private async Task DoLogout()
        {
            //var cookies = new CookieContainer();
            //using (HttpClientHandler handler = new HttpClientHandler())
            //{
            //    handler.ClientCertificateOptions = ClientCertificateOption.Manual;
            //    handler.ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) =>
            //    {
            //        return true;
            //    };
            //    handler.CookieContainer = cookies;
            //    handler.UseCookies = true;

            //    using (HttpClient client = new HttpClient(handler))
            //    {
            //        var baseUrl = SettingsPage.LOCATOR_BASE_URL;
            //        cookies.Add(new Uri(baseUrl), new Cookie(".AspNetCore.Identity.Application", LocatorCookie));
            //        var result = await client.GetAsync($"{baseUrl}api/LocationEntriesApi");
            //        if (result.IsSuccessStatusCode)
            //        {
            //            var content = await result.Content.ReadAsStringAsync();
            //            Debug.WriteLine("Success");

            //        }
            //        else
            //        {
            //            Debug.WriteLine("Failure");
            //        }
            //    }
            //}
            MessagingCenter.Send(this, SETTINGS_UPDATED_MESSAGE);
            LocatorCookie = "";
            SettingsPage.LocatorCookie("");
        }

        private async Task DoSave(object obj)
        {
            SettingsPage.SalesID(SalesID);
            SettingsPage.TaxRate(TaxRate);

            var storeId = Stores[SelectedStoreName];
            SettingsPage.StoreID(storeId);
            SettingsPage.IncludeCSVWithQuote(IncludeCSVWithQuote);

            if (Theme != Application.Current.UserAppTheme)
            {
                Application.Current.UserAppTheme = Theme;
            }

            SettingsPage.Theme(Theme);
            SettingsPage.Vibrate(Vibrate);

            MessagingCenter.Send(this, SETTINGS_UPDATED_MESSAGE);
            await ExitSettings();
        }

        private async Task ExitSettings()
        {
            await Device.InvokeOnMainThreadAsync(async () =>
            {
                await Shell.Current.Navigation.PopAsync();
            });
        }
    }
}