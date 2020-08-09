﻿using System;
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
using System.Text;

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
        public string Location { get; set; }
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
                var storeId = SettingsPage.StoreID();
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

                match = Regex.Match(body, "class=\"findItLink\"(?:.*?)>(.*?)<");
                if (match.Success)
                {
                    StringBuilder b = new StringBuilder();
                    b.Append(match.Groups[1]);
                    matches = Regex.Matches(body, "class=\"otherLocation\">(.*?)<");
                    //
                    // There is an invisible element with another findit panel in the html, so only grab the first half...
                    //
                    for(int i = 0; i < matches.Count / 2; i++)
                    {
                        var m = matches[i];
                        b.Append(m.Groups[1]);
                    }

                    item.Location = b.ToString();
                }
            }

            return item;
        }
    }


}