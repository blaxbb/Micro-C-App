using micro_c_app.Models;
using micro_c_app.Views;
using MicroCLib.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class LocationPageViewModel : BaseViewModel
    {
        private Point clickPosition;
        private bool searchMode;
        private Point cursorPosition;
        private Point panPercentage;
        private List<LocationEntry> markers;

        public Point PanPercentage { get => panPercentage; set => SetProperty(ref panPercentage, value); }
        //
        //accessed in LocationPage.xaml.cs
        public List<LocationEntry> Markers { get => markers; set => SetProperty(ref markers, value); }

        public bool SearchMode { get => searchMode; set { SetProperty(ref searchMode, value); OnPropertyChanged(nameof(ScanMode)); } }
        public bool ScanMode => !SearchMode;

        string locatorCookie;
        private string locatorPassword;
        private string locatorUsername;

        public string LocatorUsername { get => locatorUsername; set => SetProperty(ref locatorUsername, value); }
        public string LocatorPassword { get => locatorPassword; set => SetProperty(ref locatorPassword, value); }
        public string LocatorCookie { get => locatorCookie; set => SetProperty(ref locatorCookie, value); }

        public ICommand LocatorLogin { get; }
        public ICommand LocatorLogout { get; }
        public ICommand LocatorRegister { get; }

        public ICommand DoScanMode { get; }
        public ICommand DoSearchMode { get; }
        public ICommand ProductFound { get; }

        public LocationPageViewModel()
        {
            DoScanMode = new Command(() =>
            {
                SearchMode = false;
            });

            DoSearchMode = new Command(() =>
            {
                Markers = new List<LocationEntry>();
                SearchMode = true;
            });

            ProductFound = new Command<Item>(async (item) => await DoProductFound(item));

            LocatorLogin = new Command(async () => await DoLogin());
            LocatorLogout = new Command(async () => await DoLogout());
            LocatorCookie = SettingsPage.LocatorCookie();
            LocatorRegister = new Command(() => { Xamarin.Essentials.Launcher.OpenAsync($"{ SettingsPage.LOCATOR_BASE_URL}Identity/Account/Register"); });

            PropertyChanged += LocationPageViewModel_PropertyChanged;
        }

        private void LocationPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PanPercentage))
            {
                Debug.WriteLine(PanPercentage);
            }
        }

        private async Task DoProductFound(Item item)
        {
            if (SearchMode)
            {
                await DoProductSearch(item);
            }
            else
            {
                await DoProductScan(item);
            }
        }

        private async Task DoProductScan(Item item)
        {
            var entry = new LocationEntry(SettingsPage.StoreID(), item.SKU, PanPercentage.X, PanPercentage.Y);
            await entry.Post(SettingsPage.LocatorCookie());
        }

        private async Task DoProductSearch(Item item)
        {
            Markers = await LocationEntry.Search(item.SKU, SettingsPage.StoreID(), SettingsPage.LocatorCookie());

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
            if (string.IsNullOrWhiteSpace(LocatorUsername) || string.IsNullOrWhiteSpace(LocatorPassword))
            {
                DoError("You must enter a username and password for locator services!");
                return;
            }

            //Debug.WriteLine($"{LocatorUsername} - {LocatorPassword}");
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
                            DoError("Failed to grab verification token from locator server.");
                            return;
                        }
                        var token = match.Groups[1].Value;

                        var obj = new LoginPacket(LocatorUsername, LocatorPassword, token);
                        var json = JsonConvert.SerializeObject(obj);
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
                                if (appCookie != null && !string.IsNullOrWhiteSpace(appCookie?.Value))
                                {
                                    SettingsPage.LocatorCookie(appCookie.Value);
                                    LocatorCookie = appCookie.Value;
                                    MessagingCenter.Send(this, SettingsPageViewModel.SETTINGS_UPDATED_MESSAGE);
                                    return;
                                }
                            }

                            DoError("Failed to login.\n\nLikely an invalid username or password.");
                            return;
                        }
                        else
                        {
                            Debug.WriteLine("Failure");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                DoError(e.Message);
            }
        }

        private async Task DoLogout()
        {
            SettingsPage.LocatorCookie("");
            LocatorCookie = "";
            MessagingCenter.Send(this, SettingsPageViewModel.SETTINGS_UPDATED_MESSAGE);
        }
    }
}
