
using micro_c_lib.Models;
using micro_c_lib.Models.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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
        public bool ComingSoon { get; set; }
        public List<CategoryInfo> Categories { get; private set; }

        public Item()
        {
            Specs = new Dictionary<string, string>();
            PictureUrls = new List<string>();
            Plans = new List<Plan>();
        }

        public static async Task<Item> FromUrl(string urlIdStub, string storeId, CancellationToken? token = null, IProgress<ProgressInfo> progress = null)
        {
            var item = new Item();

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(15);
                token?.Register(() =>
                {
                    client.CancelPendingRequests();
                });

                progress?.Report(new ProgressInfo($"Found item, fetching details", .7d));

                var url = $"https://www.microcenter.com{urlIdStub}?storeid={storeId}";
                var response = await (token == null ? client.GetAsync(url) : client.GetAsync(url, token.Value));
                token?.ThrowIfCancellationRequested();

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return new Item(){ Name = "Product not found", SKU = "000000" };
                }

                progress?.Report(new ProgressInfo($"Parsing item details", .9d));

                var body = await response.Content.ReadAsStringAsync();
                token?.ThrowIfCancellationRequested();
                item.ID = ParseIDFromURL(urlIdStub);
                item.URL = ParseURL(body);

                item.Name = ParseName(body);
                item.Brand = ParseBrand(body);
                item.Specs = ParseSpecs(body);
                item.SKU = ParseSKU(item);

                item.Stock = ParseStock(body);
                item.Price = ParsePrice(body);
                item.OriginalPrice = ParseOriginalPrice(body, item);

                item.Location = ParseLocations(body);
                item.PictureUrls = ParsePictures(body);

                item.Plans = ParsePlans(body);
                item.ComingSoon = ParseComingSoon(body);
                if (item.ComingSoon)
                {
                    item.Stock = "Soon";
                }

                var categories = ParseCategories(body);
            }

            token?.ThrowIfCancellationRequested();
            return item;
        }

        public static Dictionary<string, string> ParseSpecs(string body)
        {
            var results = new Dictionary<string, string>();
            var matches = GetSpecs.Matches(body);
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    results[HttpDecode(m.Groups[2].Value)] = HttpDecode(m.Groups[3].Value.Replace("<br /> ", "\n"));
                }
            }

            return results;
        }
        public static string ParseSKU(Item item)
        {
            if (item.Specs != null && item.Specs.ContainsKey("SKU"))
            {
                return item.Specs["SKU"] ?? "";
            }

            return "";
        }

        public static float ParsePrice(string body)
        {
            var match = GetPrice.Match(body);
            if (match.Success)
            {
                if (float.TryParse(match.Groups[1].Value, out float price))
                {
                    return price;
                }
            }

            return 0f;
        }

        public static float ParseOriginalPrice(string body, Item item)
        {
            var regexes = new Regex[] { GetOriginalPrice, GetOriginalPriceAlt };
            foreach(var reg in regexes)
            {
                var match = reg.Match(body);
                if (match.Success)
                {
                    if (float.TryParse(match.Groups[1].Value, out float price))
                    {
                        return price;
                    }
                }
            }

            return item.price;
        }

        public static string ParseURL(string body)
        {
            var match = GetURL.Match(body);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "";
        }
        public static string ParseStock(string body)
        {
            var match = GetStock.Match(body);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }
            return "";
        }

        public static string ParseName(string body)
        {
            var match = GetName.Match(body);
            if (match.Success)
            {
                return HttpDecode(match.Groups[1].Value);
            }
            return "";
        }

        public static List<string> ParsePictures(string body)
        {
            var matches = GetPictures.Matches(body);
            var result = new List<string>();
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    result.Add(m.Groups[1].Value);
                }
            }

            return result;
        }

        public static string ParseLocations(string body)
        {
            var match = GetFirstLocation.Match(body);
            if (match.Success)
            {
                StringBuilder b = new StringBuilder();
                b.Append(match.Groups[1]);
                var matches = GetOtherLocations.Matches(body);
                //
                // There is an invisible element with another findit panel in the html, so only grab the first half...
                //
                for (int i = 0; i < matches.Count / 2; i++)
                {
                    var m = matches[i];
                    b.Append(m.Groups[1]);
                }

                return b.ToString();
            }

            return "";
        }
        public static List<Plan> ParsePlans(string body)
        {
            var matches = GetPlans.Matches(body);
            var result = new List<Plan>();
            if (matches.Count > 0)
            {
                foreach (Match m in matches)
                {
                    if (float.TryParse(m.Groups[2].Value, out float price))
                    {
                        result.Add(new Plan()
                        {
                            Name = m.Groups[1].Value,
                            Price = price
                        });
                    }
                }
            }

            return result;
        }

        public static string ParseBrand(string body)
        {
            var match = GetBrand.Match(body);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "";
        }
        public static bool ParseComingSoon(string body)
        {
            var match = GetComingSoon.Match(body);
            if (match.Success)
            {
                return true;
            }

            return false;
        }

        public static List<CategoryInfo> ParseCategories(string body)
        {
            var match = Regex.Match(body, "<nav aria-labelledby=\"breadcrumb-label\"(?:.*?)<script type=\"application/ld\\+json\">(.*?)</script>", RegexOptions.Singleline);
            if (match.Success)
            {
                var info = JsonConvert.DeserializeObject<CategoryJsonResult>(match.Groups[1].Value);
                return info.Categories.Skip(1).ToList();
            }

            return new List<CategoryInfo>();
        }

        public static string ParseIDFromURL(string url)
        {
            var match = GetIDFromURL.Match(url);
            if (match.Success)
            {
                return match.Groups[1].Value;
            }

            return "";
        }


        private static Regex GetSpecs => new Regex("<div class=\"spec-body\"><div(?: class=)?[a-zA-Z\"=]*?>(.*?)(.*?)</div>.?<div(?: class=)?[a-zA-Z\"=]*?>(.*?)</div", RegexOptions.Singleline);
        private static Regex GetPrice => new Regex("'productPrice':'(.*?)',");
        private static Regex GetURL => new Regex("'pageUrl':'(.*?)',");
        private static Regex GetOriginalPrice => new Regex("\"savings\"><span>\\$([\\d\\.]+)");
        private static Regex GetOriginalPriceAlt => new Regex("<span id='pricing' content=\"(.*?)\">");
        private static Regex GetStock => new Regex("<span class=\"inventoryCnt\">(.*?)</span>");
        private static Regex GetName => new Regex("data-name=\"(.*?)\"");
        private static Regex GetPictures => new Regex("<img class= ?\"productImageZoom\" src=\"(.*?)\"");
        private static Regex GetFirstLocation => new Regex("class=\"findItLink\"(?:.*?)>(.*?)<");
        private static Regex GetOtherLocations => new Regex("class=\"otherLocation\">(.*?)<");
        private static Regex GetPlans => new Regex("#planDetails(?:.*?)>(.*?)<(?:.*?)pricing\"> \\$(.*?)<");
        private static Regex GetIDFromURL => new Regex("\\/product\\/(\\d+?)\\/");
        private static Regex GetBrand => new Regex("data-brand=\"(.*?)\"");
        private static Regex GetComingSoon => new Regex("<div class=\"comingsoon\"");

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
        public Item CloneAndResetQuantity()
        {
            var json = JsonConvert.SerializeObject(this);
            var ret = JsonConvert.DeserializeObject<Item>(json);
            ret.Quantity = 1;
            return ret;
        }
    }


}