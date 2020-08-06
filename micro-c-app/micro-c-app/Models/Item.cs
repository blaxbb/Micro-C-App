using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Text.Json;

namespace micro_c_app.Models
{
    public class Item
    {
        public string SKU { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public string Stock { get; set; }
        public float Price { get; set; }
        public Dictionary<string, string> Specs { get; set; }

        public Item()
        {
            Specs = new Dictionary<string, string>();
        }

        public static async Task<Item> FromUrl(string url)
        {
            var item = new Item();

            using(HttpClient client = new HttpClient())
            {
                string storeId = "141";
                var response = await client.GetAsync($"https://www.microcenter.com{url}?storeid={storeId}");
                if(response.StatusCode != System.Net.HttpStatusCode.OK)
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
                        item.Specs[m.Groups[2].Value] = m.Groups[3].Value.Replace("<br /> ", "\n");
                    }
                }

                var match = Regex.Match(body, "data-price=\"(.*?)\"");
                if (match.Success)
                {
                    if (float.TryParse(match.Groups[1].Value, out float price))
                    {
                        item.Price = price;
                    }
                }

                match = Regex.Match(body, "<span class=\"inventoryCnt\">(.*?)</span>");
                if (match.Success)
                {
                    item.Stock = match.Groups[1].Value;
                }

                match = Regex.Match(body, "data-name=\"(.*?)\"");
                if (match.Success)
                {
                    item.Name = match.Groups[1].Value;
                }

                match = Regex.Match(body, "<img class= ?\"productImageZoom\" src=\"(.*?)\"");
                if (match.Success)
                {
                    item.PictureUrl = match.Groups[1].Value;
                }

            }


            return item;
        }
    }

    
}