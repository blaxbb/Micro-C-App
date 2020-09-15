
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace MicroCLib.Models
{
    public class Item : NotifyPropertyChangedItem
    {
        public string URL { get; set; } = "";
        public string SKU { get; set; } = "000000";
        public string Name { get; set; } = "";
        public List<string> PictureUrls { get; set; }
        public string Stock { get; set; } = "";
        public float Price { get => price; set => SetProperty(ref price, value); }
        public float OriginalPrice { get; set; } = 0f;
        public float Discount => Price - OriginalPrice;
        public bool OnSale => Price != OriginalPrice;
        public Dictionary<string, string> Specs { get; set; }

        private int quantity = 1;
        private float price = 0f;
        private string brand = "";

        public int Quantity { get => quantity; set => SetProperty(ref quantity, value); }
        public string Location { get; set; } = "";
        public List<Plan> Plans { get; set; }
        public string ID { get; set; } = "";
        public string Brand { get => brand; set => SetProperty(ref brand, value); }

        public Item()
        {
            Specs = new Dictionary<string, string>();
            PictureUrls = new List<string>();
            Plans = new List<Plan>();

        }

        public static async Task<Item> FromUrl(string url, string storeId)
        {
            var item = new Item();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetAsync($"https://www.microcenter.com{url}?storeid={storeId}");
                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new Item(){ Name = "Product not found", SKU = "000000" };
                }

                var body = await response.Content.ReadAsStringAsync();
                //body = body.Replace("\r", "");
                //body = body.Replace("\n", "");
                var matches = Regex.Matches(body, "<div class=\"spec-body\"><div(?: class=)?[a-zA-Z\"=]*?>(.*?)(.*?)</div>.?<div(?: class=)?[a-zA-Z\"=]*?>(.*?)</div", RegexOptions.Singleline);
                if (matches.Count > 0)
                {
                    foreach (Match m in matches)
                    {
                        item.Specs[HttpDecode(m.Groups[2].Value)] = HttpDecode(m.Groups[3].Value.Replace("<br /> ", "\n"));
                    }
                }

                if (item.Specs != null && item.Specs.ContainsKey("SKU"))
                {
                    item.SKU = item.Specs["SKU"] ?? "000000";
                }

                var match = Regex.Match(body, "'productPrice':'(.*?)',");
                if (match.Success)
                {
                    if (float.TryParse(match.Groups[1].Value, out float price))
                    {
                        item.Price = price;
                    }
                }

                match = Regex.Match(body, "'pageUrl':'(.*?)',");
                if (match.Success)
                {
                    item.URL = match.Groups[1].Value;
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
                    item.Name = HttpDecode(match.Groups[1].Value);
                }

                matches = Regex.Matches(body, "<img class= ?\"productImageZoom\" src=\"(.*?)\"");
                if (matches.Count > 0)
                {
                    item.PictureUrls = new List<string>();
                    foreach (Match m in matches)
                    {
                        item.PictureUrls.Add(m.Groups[1].Value);
                    }
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
                    for (int i = 0; i < matches.Count / 2; i++)
                    {
                        var m = matches[i];
                        b.Append(m.Groups[1]);
                    }

                    item.Location = b.ToString();
                }

                matches = Regex.Matches(body, "#planDetails(?:.*?)>(.*?)<(?:.*?)pricing\"> \\$(.*?)<");
                if (matches.Count > 0)
                {
                    item.Plans = new List<Plan>();
                    foreach (Match m in matches)
                    {
                        if (float.TryParse(m.Groups[2].Value, out float price))
                        {
                            item.Plans.Add(new Plan()
                            {
                                Name = m.Groups[1].Value,
                                Price = price
                            });
                        }
                    }
                }

                match = Regex.Match(url, "\\/product\\/(\\d+?)\\/");
                if (match.Success)
                {
                    item.ID = match.Groups[1].Value;
                }

                match = Regex.Match(body, "data-brand=\"(.*?)\"");
                if (match.Success)
                {
                    item.Brand = match.Groups[1].Value;
                }

            }

            return item;
        }

        public static string HttpDecode(string s)
        {
            var decoded = HttpUtility.HtmlDecode(s);
            if (decoded == s)
            {
                return decoded;
            }
            return HttpDecode(decoded);
        }

        public override string ToString()
        {
            return Name;
        }
    }


}