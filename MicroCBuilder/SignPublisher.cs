using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Windows.Media.Capture;
using Windows.Web.Http;

namespace MicroCBuilder
{
    public static class SignPublisher
    {
        static HttpClient client;
        public static async Task<string?> Publish(List<string> skus, string baseUrl, string signType, string username, string password, string batchName)
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
            var auth = await Login(baseUrl, username, password);
            if (string.IsNullOrWhiteSpace(auth))
            {
                return default;
            }
            client.DefaultRequestHeaders.Authorization = new Windows.Web.Http.Headers.HttpCredentialsHeaderValue("bearer", auth);

            var batch = await CreateBatch(baseUrl, username, batchName);
            if (batch == null)
            {
                return default;
            }

            var items = await FindItems(baseUrl, skus, username);
            if (items == null || items.Count == 0)
            {
                return default;
            }

            await AddItems(baseUrl, items, signType, username, batch.batchID.ToString());
            client.Dispose();
            
            return $"{baseUrl.Replace("PublishingServices", "PublishingInstore")}/#/home/viewbatches/viewsign/{batch.batchID}";
        }

        class LoginOptions
        {
            public string grant_type { get; set; } = "password";
            public string username { get; set; }
            public string password { get; set; }
            public string app_id { get; set; } = "WSSettings, UISettings";

            public LoginOptions(string username, string password)
            {
                this.username = username;
                this.password = password;
            }
        }

        struct LoginResult
        {
            public string access_token { get; set; }
        }

        private static async Task<string?> Login(string baseUrl, string username, string password)
        {
            var opts = new LoginOptions(username, password);

            var result = await client.TryPostAsync(
                new Uri($"{baseUrl}/oauth/token"),
                new HttpStringContent(
                    JsonConvert.SerializeObject(opts),
                    Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"
                )
            );

            if (result.Succeeded)
            {
                var json = await result.ResponseMessage.Content.ReadAsStringAsync();
                var login = JsonConvert.DeserializeObject<LoginResult>(json);
                return login.access_token;
            }

            return default;
        }

        public class CreateBatchOptions
        {
            public string batchname { get; set; }
            public string effectiveDate { get; set; } = DateTime.Now.ToString("ddd MMM dd yyyy");
            public string comments { get; set; }
            public string expireDate { get; set; } = (DateTime.Now + TimeSpan.FromDays(2)).ToString("ddd MMM dd yyyy");
            public string batchtype { get; set; } = "SIGNS";
            public string area { get; set; }
            public string areaType { get; set; } = "3";

            [JsonProperty("event")]
            public string _event { get; set; } = "Price Changes";
            public string source { get; set; } = "My Print Sort";

            public CreateBatchOptions(string batchname, string comments, string area)
            {
                this.batchname = batchname;
                this.comments = comments;
                this.area = area;
            }
        }


        public class CreateBatchResult
        {
            public int batchID { get; set; }
            public string batchType { get; set; }
            public CreateBatchLink links { get; set; }
        }

        public class CreateBatchLink
        {
            public string rel { get; set; }
            public string href { get; set; }
        }


        private static async Task<CreateBatchResult?> CreateBatch(string baseUrl, string username, string batchName)
        {
            var opts = new List<CreateBatchOptions>() { new CreateBatchOptions(batchName, $"Created for signs by {username}", username) };
            var json2 = JsonConvert.SerializeObject(opts);
            var result = await client.TryPostAsync(
                new Uri($"{baseUrl}/api/SignBatches"),
                new HttpStringContent(
                    json2,
                    Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"
                )
            );

            if (result.Succeeded)
            {
                var json = await result.ResponseMessage.Content.ReadAsStringAsync();
                var batches = JsonConvert.DeserializeObject<List<CreateBatchResult>>(json);
                if (batches != null && batches.Count > 0)
                {
                    return batches[0];
                }
            }

            return default;
        }


        public class FindItemResult
        {
            public int libraryid { get; set; }
            public string printOrder { get; set; }
            public string itemID { get; set; }
            public int copies { get; set; }
            public string uPC { get; set; }
            public string categoryID { get; set; }
            public string categoryDesc { get; set; }
            public string subCategoryID { get; set; }
            public string subCategoryDesc { get; set; }
            public string departmentID { get; set; }
            public string departmentDesc { get; set; }
            public string shortName { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string brand { get; set; }
            public string manufacturer { get; set; }
            public string model { get; set; }
        }


        private static async Task<List<FindItemResult>> FindItems(string baseUrl, List<string> skus, string username)
        {
            var uri = new UriBuilder($"{baseUrl}/api/StoreItemPrice");
            uri.Port = -1;
            var query = HttpUtility.ParseQueryString(uri.Query);
            query["fields"] = "*";
            query["page"] = "1";
            query["per_page"] = "50";
            query["area"] = username;
            query["sort"] = "name%20DESC";
            query["filters"] = $"";

            uri.Query = query.ToString();
            var url = uri.Uri.AbsoluteUri;
            url = $"{baseUrl}/api/StoreItemPrice?fields=*&page=1&per_page=50&area={username}&sort=name%20DESC&filters=((%20areaType%20=%200%20)%20OR%20(%20AreaType%20=%202%20AND%20Area%20=%2019%20)%20OR%20(%20AreaType%20=%203%20AND%20Area%20=%20141%20))%20AND%20({string.Join("%20OR%20", skus.Select(sku => $"itemid%20like%20%27%25{sku}%25%27"))})";
            var result = await client.TryGetAsync(
                new Uri(url)
            );

            if (result.Succeeded)
            {
                var json = await result.ResponseMessage.Content.ReadAsStringAsync();
                var items = JsonConvert.DeserializeObject<List<FindItemResult>>(json);
                if (items != null && items.Count > 0)
                {
                    return items;
                }
            }

            return default;
        }


        public class AddItemsOptions
        {
            public string Area { get; set; }
            public string BatchId { get; set; }
            public string DefaultTemplate { get; set; }
            public string Filter { get; set; }

            public AddItemsOptions(string username, string batchId, string defaultTemplate, List<FindItemResult> items)
            {
                Area = username;
                BatchId = batchId;
                DefaultTemplate = defaultTemplate;
                Filter = $"(( areaType = 0 ) OR " +
                    $"( AreaType = 2 AND Area = 19 ) OR " +
                    $"( AreaType = 3 AND Area = {username} )) AND " +
                    $"({string.Join(" OR ", items.Select(i => $"ID IN ('{i.libraryid}')"))})";
            }
        }


        private static async Task AddItems(string baseUrl, List<FindItemResult> items, string signType, string username, string batchId)
        {
            var opts = new AddItemsOptions(username, batchId, signType, items);
            var json = JsonConvert.SerializeObject(opts);
            var result = await client.TryPostAsync(
                new Uri($"{baseUrl}/api/v2/SignItems/Create/all"),
                new HttpStringContent(
                    json,
                    Windows.Storage.Streams.UnicodeEncoding.Utf8, "application/json"
                )
            );

            return;
        }
    }
}
