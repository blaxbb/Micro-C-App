using MicroCLib.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
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
        }
        public static string GetSearchUrl(string query, string storeId, string categoryFilter, OrderByMode orderBy, int resultsPerPage, int page)
        {
            return $"https://www.microcenter.com/search/search_results.aspx?Ntt={query}&storeid={storeId}&myStore=false&Ntk=all&N={categoryFilter}&sortby={orderBy}&rpp={resultsPerPage}&page={page}";
        }

        public static async Task<SearchResults> LoadAll(string searchQuery, string storeID, string categoryFilter, OrderByMode orderBy)
        {
            int page = 1;
            var result = new SearchResults() { TotalResults = 1 };
            while(result.Items.Count < result.TotalResults)
            {
                var addResult = await LoadQuery(searchQuery, storeID, categoryFilter, orderBy, page);
                result.Items.AddRange(addResult.Items);
                result.TotalResults = addResult.TotalResults;
                page++;
            }


            return result;

        }

        public static async Task<SearchResults> LoadQuery(string searchQuery, string storeID, string categoryFilter, OrderByMode orderBy, int page)
        {
            var response = await client.GetAsync(GetSearchUrl(searchQuery, storeID, categoryFilter, orderBy, RESULTS_PER_PAGE, page));
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var body = response.Content.ReadAsStringAsync().Result;

                var result = await ParseBody(body);
                result.Page = page;
                return result;
            }

            return new SearchResults();
        }

        public static async Task<SearchResults> ParseBody(string body)
        {
            var result = new SearchResults();
            var shortMatches = Regex.Matches(body, "class=\"image\" data-name=\"(.*?)\" data-id=\"(.*?)\"(?:.*?)price=\"(.*?)\"(?:.*?)data-brand=\"(.*?)\"(?:.*?)href=\"(.*?)\"(?:.*?)src=\"(.*?)\"");
            var stockMatches = Regex.Matches(body, "<div class=\"stock\">(?:.+?)(\\d[\\d+]*?) <", RegexOptions.Singleline);
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

                var url = m.Groups[5].Value;
                string id = "000000";
                Match m_id = Regex.Match(url, "/product/(\\d+)/");
                if (m_id.Success)
                {
                    id = m_id.Groups[1].Value;
                }

                float.TryParse(m.Groups[3].Value, out float price);
                var item = new Item()
                {
                    Name = Item.HttpDecode(m.Groups[1].Value),
                    ID = id,
                    Price = price,
                    Brand = m.Groups[4].Value,
                    URL = url,
                    PictureUrls = new List<string>() { m.Groups[6].Value },
                    Stock = stock,
                    SKU = sku,
                };

                result.Items.Add(item);
            }
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
