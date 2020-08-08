using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;
using System.Web;
using micro_c_app.ViewModels;
using Xamarin.Essentials;
using System.Linq;
using micro_c_app.Views;

namespace micro_c_app.Models
{
    public class Item : NotifyPropertyChangedItem
    {

        public string SKU { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string Stock { get; set; }
        public float Price { get; set; }
        public float OriginalPrice { get; set; }
        public float Discount => Price - OriginalPrice;
        public bool OnSale => Price != OriginalPrice;
        public Dictionary<string, string> Specs { get; set; }

        private int quantity = 1;
        public int Quantity { get => quantity; set => SetProperty(ref quantity, value); }
        public Item()
        {
            Specs = new Dictionary<string, string>();
        }

        public static async Task<Item> FromId(string productID)
        {
            var url = $"/product/{productID}";
            var item = new Item();

            using (HttpClient client = new HttpClient())
            {
                var storeId = Preferences.Get(SettingsPage.PREF_SELECTED_STORE, "141");
                var response = await client.GetAsync($"https://www.microcenter.com{url}?storeid={storeId}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return default(Item);
                }

                var body = await response.Content.ReadAsStringAsync();
                //body = body.Replace("\r", "");
                //body = body.Replace("\n", "");
                var matches = Regex.Matches(body, "<div class=\"spec-body\"><div(?: class=)?[a-zA-Z\"=]*?>(.*?)(.*?)</div>.?<div(?: class=)?[a-zA-Z\"=]*?>(.*?)</div", RegexOptions.Singleline);
                if (matches.Count > 0)
                {
                    foreach (Match m in matches)
                    {
                        item.Specs[HttpUtility.HtmlDecode(m.Groups[2].Value)] = HttpUtility.HtmlDecode(m.Groups[3].Value.Replace("<br /> ", "\n"));
                    }
                }

                item.SKU = item.Specs.ContainsKey("SKU") ? item.Specs["SKU"] : null;

                var match = Regex.Match(body, "'productPrice':'(.*?)',");
                if (match.Success)
                {
                    if (float.TryParse(match.Groups[1].Value, out float price))
                    {
                        item.Price = price;
                    }
                }

                match = Regex.Match(body, "data-price=\"(.*?)\"");
                if (match.Success)
                {
                    if (float.TryParse(match.Groups[1].Value, out float price))
                    {
                        item.OriginalPrice = price;
                    }
                }
                else
                {
                    item.OriginalPrice = item.Price;
                }

                match = Regex.Match(body, "<span class=\"inventoryCnt\">(.*?)</span>");
                if (match.Success)
                {
                    item.Stock = match.Groups[1].Value;
                }

                match = Regex.Match(body, "data-name=\"(.*?)\"");
                if (match.Success)
                {
                    item.Name = HttpUtility.HtmlDecode(match.Groups[1].Value);
                }

                match = Regex.Match(body, "<img class= ?\"productImageZoom\" src=\"(.*?)\"");
                if (match.Success)
                {
                    item.PictureUrl = match.Groups[1].Value;
                }

                match = Regex.Match(body, "Add to Cart to see price");
                if (match.Success)
                {
                    var values = new List<KeyValuePair<string, string>>()
                    {
                        new KeyValuePair<string, string>("store_id", storeId),
                        new KeyValuePair<string, string>("sku", item.SKU),
                        new KeyValuePair<string, string>("productID", productID)
                    };
                    var postContent = new FormUrlEncodedContent(values);

                    var postResponse = await client.PostAsync("https://www.microcenter.com/store/add_product.aspx", postContent);
                    if (postResponse.StatusCode == System.Net.HttpStatusCode.OK)
                    {

                    }

                }

            }


            return item;
        }
    }


}