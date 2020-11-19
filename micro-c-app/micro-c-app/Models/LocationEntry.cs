using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace micro_c_app.Models
{
    public class LocationEntry
    {
        public long ID { get; set; }
        public string Author { get; set; }
        public DateTime Created { get; set; }
        public string Location { get; set; }
        public string SKU { get; set; }
        public double X { get; set; }
        public double Y { get; set; }

        public LocationEntry()
        {

        }

        public LocationEntry(string location, string sKU, double x, double y)
        {
            Location = location;
            SKU = sKU;
            X = x;
            Y = y;
        }

        public async Task Post(string token)
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
                    var baseUrl = micro_c_app.Views.SettingsPage.LOCATOR_BASE_URL;
                    cookies.Add(new Uri(baseUrl), new Cookie(".AspNetCore.Identity.Application", token));

                    var json = JsonSerializer.Serialize(this);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var result = await client.PostAsync($"{baseUrl}api/location", content);
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsStringAsync();
                        Debug.WriteLine("Success");

                    }
                    else
                    {
                        Debug.WriteLine("Failure");
                    }
                }
            }
        }

        public static async Task<List<LocationEntry>> Search(string sku, string location, string token)
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
                    var baseUrl = micro_c_app.Views.SettingsPage.LOCATOR_BASE_URL;
                    cookies.Add(new Uri(baseUrl), new Cookie(".AspNetCore.Identity.Application", token));

                    var payload = new Dictionary<string, string>()
                    {
                        {"location", location},
                        {"sku", sku}
                    };
                    
                    

                    var result = await client.GetAsync(QueryHelpers.AddQueryString($"{baseUrl}api/location/search", payload));
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsStringAsync();
                        var ret = JsonSerializer.Deserialize<List<LocationEntry>>(response);
                        return ret;

                    }
                    else
                    {
                        Debug.WriteLine("Failure");
                    }
                }

            }
            return new List<LocationEntry>();
        }

        public static Stream GetMapImageStream(string location, string token)
        {
            return GetMapImage(location, token).Result;
        }

        public static async Task<Stream> GetMapImage(string location, string token)
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
                    var baseUrl = micro_c_app.Views.SettingsPage.LOCATOR_BASE_URL;
                    cookies.Add(new Uri(baseUrl), new Cookie(".AspNetCore.Identity.Application", token));

                    var result = await client.GetAsync($"{baseUrl}api/securefile/{location}_map.png");
                    if (result.IsSuccessStatusCode)
                    {
                        var response = await result.Content.ReadAsStreamAsync();
                        return response;
                    }
                    else
                    {
                        Debug.WriteLine("Failure");
                    }
                }

            }

            return null;
        }
    }
}
