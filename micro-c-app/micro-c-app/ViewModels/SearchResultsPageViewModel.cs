using micro_c_app.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace micro_c_app.ViewModels
{
    public class SearchResultsPageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; }

        public SearchResultsPageViewModel()
        {
            Title = "Search";
            Items = new ObservableCollection<Item>();
        }

        public async Task ParseBody(string body)
        {
            var shortMatches = Regex.Matches(body, "class=\"image\" data-name=\"(.*?)\" data-id=\"(.*?)\"(?:.*?)price=\"(.*?)\"(?:.*?)data-brand=\"(.*?)\"(?:.*?)href=\"(.*?)\"(?:.*?)src=\"(.*?)\"");
            var stockMatches = Regex.Matches(body, "<div class=\"stock\">(?:.*?)>([\\d+ ]*?)<", RegexOptions.Singleline);
            var skuMatches = Regex.Matches(body, "<p class=\"sku\">SKU: (\\d{6})</p>");
            for (int i = 0; i < shortMatches.Count; i++)
            {
                Match m = shortMatches[i];
                string stock = "0";
                if (i < stockMatches.Count)
                {
                    Match stockMatch = stockMatches[i];
                    stock = string.IsNullOrWhiteSpace(stockMatch.Groups[1].Value) ? "0" : stockMatch.Groups[1].Value;
                }

                string sku = "000000";
                if (i < skuMatches.Count)
                {
                    var skuMatch = skuMatches[i];
                    sku = skuMatch.Groups[1].Value ?? "000000";
                }

                float.TryParse(m.Groups[3].Value, out float price);
                var item = new Models.Item()
                {
                    Name = Item.HttpDecode(m.Groups[1].Value),
                    Price = price,
                    Brand = m.Groups[4].Value,
                    URL = m.Groups[5].Value,
                    PictureUrls = new List<string>() { m.Groups[6].Value },
                    Stock = stock,
                    SKU = sku,
                };

                await Device.InvokeOnMainThreadAsync(() =>
                {
                    Items.Add(item);
                });
            }
        }
    }
}