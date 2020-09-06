using micro_c_app.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using static micro_c_app.Views.SearchView;

namespace micro_c_app.ViewModels
{
    public class SearchResultsPageViewModel : BaseViewModel
    {
        public ObservableCollection<Item> Items { get; }

        public string SearchQuery { get; set; }
        public string StoreID { get; set; }
        public string CategoryFilter { get; set; }
        public OrderByMode OrderBy { get; set; }

        public ICommand ChangeOrderBy { get; }

        HttpClient client;
        private int itemThreshold = 10;
        private int totalResults;
        private int page;
        public const int RESULTS_PER_PAGE = 96;
        public int ItemThreshold { get => itemThreshold; set => SetProperty(ref itemThreshold, value); }
        public ICommand LoadMore { get; }
        public int Page { get => page; set => SetProperty(ref page, value); }
        public int TotalPages => (int)Math.Ceiling((double)totalResults / RESULTS_PER_PAGE);
        public int TotalResults { get => totalResults; set => SetProperty(ref totalResults, value); }

        public SearchResultsPageViewModel()
        {
            Title = "Search";
            client = new HttpClient();
            Items = new ObservableCollection<Item>();

            LoadMore = new Command(async () =>
            {
                if(page < TotalPages)
                {
                    page++;
                    await LoadQuery();
                }
            });

            ChangeOrderBy = new Command(async () =>
            {
                await Device.InvokeOnMainThreadAsync(async () =>
                {
                    var result = await Shell.Current.DisplayActionSheet("Order Mode", "Cancel", null, Enum.GetNames(typeof(OrderByMode)));
                    if (result != null && result != "Cancel")
                    {
                        if (Enum.TryParse<OrderByMode>(result, out var newMode))
                        {
                            if (OrderBy != newMode)
                            {
                                OrderBy = newMode;
                                Items.Clear();
                                page = 1;
                                await LoadQuery();
                            }
                        }
                    }
                });
            });
        }

        private async Task LoadQuery()
        {
            var response = await client.GetAsync(GetSearchUrl(SearchQuery, StoreID, CategoryFilter, OrderBy, RESULTS_PER_PAGE, page));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var body = response.Content.ReadAsStringAsync().Result;

                await ParseBody(body);
            }
        }

        public async Task ParseBody(string body)
        {
            var shortMatches = Regex.Matches(body, "class=\"image\" data-name=\"(.*?)\" data-id=\"(.*?)\"(?:.*?)price=\"(.*?)\"(?:.*?)data-brand=\"(.*?)\"(?:.*?)href=\"(.*?)\"(?:.*?)src=\"(.*?)\"");
            var stockMatches = Regex.Matches(body, "<div class=\"stock\">(?:.*?)>([\\d+ ]*?)<", RegexOptions.Singleline);
            var skuMatches = Regex.Matches(body, "<p class=\"sku\">SKU: (\\d{6})</p>");
            var newItems = new List<Item>();
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
                var item = new Item()
                {
                    Name = Item.HttpDecode(m.Groups[1].Value),
                    Price = price,
                    Brand = m.Groups[4].Value,
                    URL = m.Groups[5].Value,
                    PictureUrls = new List<string>() { m.Groups[6].Value },
                    Stock = stock,
                    SKU = sku,
                };

                newItems.Add(item);
            }

            await Device.InvokeOnMainThreadAsync(() =>
            {
                foreach(var item in newItems)
                {
                    Items.Add(item);
                }
            });

            var match = Regex.Match(body, "(\\d+) items found");
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int totalResults))
                {
                    TotalResults = totalResults;
                }
            }
        }
    }
}