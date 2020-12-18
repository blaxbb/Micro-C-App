using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace micro_c_lib.Models
{
    public static class Search
    {
        static HttpClient client;
        public const int RESULTS_PER_PAGE = 96;
        public enum OrderByMode
        {
            match,
            rating,
            numreviews,
            pricelow,
            pricehigh
        }

        static Search()
        {
            client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(15);
        }
        public static string GetSearchUrl(string query, string storeId, string categoryFilter, OrderByMode orderBy, int resultsPerPage, int page)
        {
            return $"https://www.microcenter.com/search/search_results.aspx?Ntt={query}&storeid={storeId}&myStore=false&Ntk=all&N={categoryFilter}&sortby={orderBy}&rpp={resultsPerPage}&page={page}";
        }

        public static async Task<SearchResults> LoadAll(string searchQuery, string storeID, string categoryFilter, OrderByMode orderBy, CancellationToken? token = null)
        {
            int page = 1;
            var result = new SearchResults() { TotalResults = 1 };

            while(result.Items.Count < result.TotalResults)
            {
                if(token != null && token.Value.IsCancellationRequested)
                {
                    return new SearchResults() { };
                }

                var addResult = await LoadQuery(searchQuery, storeID, categoryFilter, orderBy, page, token);
                result.Items.AddRange(addResult.Items);
                result.TotalResults = addResult.TotalResults;
                page++;
            }

            token?.ThrowIfCancellationRequested();
            return result;

        }

        public static async Task<SearchResults> LoadQuery(string searchQuery, string storeID, string categoryFilter, OrderByMode orderBy, int page, CancellationToken? token = null, IProgress<ProgressInfo> progress = null)
        {
            token?.Register(() =>
            {
                client?.CancelPendingRequests();
            });

            progress?.Report(new ProgressInfo($"Loading query {searchQuery}", .3));

            var url = GetSearchUrl(searchQuery, storeID, categoryFilter, orderBy, RESULTS_PER_PAGE, page);
            var response = await (token != null ? client.GetAsync(url, token.Value) :  client.GetAsync(url));
            token?.ThrowIfCancellationRequested();
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                progress?.Report(new ProgressInfo($"Parsing query {searchQuery}", .5));

                var body = await response.Content.ReadAsStringAsync();

                var result = await ParseBody(body, token);
                result.Page = page;
                token?.ThrowIfCancellationRequested();
                return result;
            }

            return new SearchResults();
        }

        public static async Task<SearchResults> ParseBody(string body, CancellationToken? token = null)
        {
            var result = new SearchResults();
            var shortMatches = Regex.Matches(body, "class=\"image\" data-name=\"(.*?)\" data-id=\"(.*?)\"(?:.*?)price=\"(.*?)\"(?:.*?)data-brand=\"(.*?)\"(?:.*?)href=\"(.*?)\"(?:.*?)src=\"(.*?)\"");
            var stockMatches = Regex.Matches(body, "<div class=\"stock\">(?:.+?)strong>\\s*(.*?)<span", RegexOptions.Singleline);
            var skuMatches = Regex.Matches(body, "<p class=\"sku\">SKU: (\\d{6})</p>");
            var newItems = new List<Item>();

            var match = Regex.Match(body, "(\\d+) items found");
            if (match.Success)
            {
                if (int.TryParse(match.Groups[1].Value, out int totalResults))
                {
                    result.TotalResults = totalResults;
                }
            }

            for (int i = 0; i < shortMatches.Count; i++)
            {

                token?.ThrowIfCancellationRequested();

                bool comingSoon = false;
                Match m = shortMatches[i];
                string stock = "0";
                if (i < stockMatches.Count)
                {
                    Match stockMatch = stockMatches[i];
                    stock = string.IsNullOrWhiteSpace(stockMatch.Groups[1].Value) ? "0" : stockMatch.Groups[1].Value;
                    if (stock.Contains("<"))
                    {
                        stock = "Soon";
                        comingSoon = true;
                    }
                }

                string sku = "000000";
                if (i < skuMatches.Count)
                {
                    var skuMatch = skuMatches[i];
                    sku = skuMatch.Groups[1].Value ?? "000000";
                }

                var url = m.Groups[5].Value;
                string id = "000000";
                Match m_id = Regex.Match(url, "/product/(\\d+)/");
                if (m_id.Success)
                {
                    id = m_id.Groups[1].Value;
                }
                else
                {
                    Debug.WriteLine("ID NOT FOUND FOR SEARCH RESULT");
                    Debug.WriteLine(m.Value);
                }

                float.TryParse(m.Groups[3].Value, out float price);
                var item = new Item()
                {
                    Name = Item.HttpDecode(m.Groups[1].Value),
                    ID = id,
                    Price = price,
                    OriginalPrice = price,
                    Brand = m.Groups[4].Value,
                    URL = url,
                    PictureUrls = new List<string>() { m.Groups[6].Value },
                    Stock = stock,
                    SKU = sku,
                    ComingSoon = comingSoon
                };

                result.Items.Add(item);
            }
            token?.ThrowIfCancellationRequested();
            return result;
        }
    }

    public class SearchResults
    {
        public int TotalResults { get; set; }
        public int Page { get; set; }
        public List<Item> Items { get; set; }

        public SearchResults()
        {
            Items = new List<Item>();
        }
    }

}
