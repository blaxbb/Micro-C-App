using micro_c_lib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace micro_c_lib.Tests
{
    [TestClass]
    public class SearchTests
    {
        SearchResults results;

        const string QUERY = "ryzen 5";
        const string STORE = "141";

        public SearchTests()
        {
            var task = Search.LoadQuery(QUERY, STORE, null, Search.OrderByMode.pricelow, 1);
            task.Wait();
            results = task.Result;
        }

        [TestMethod]
        public void ResultsExist()
        {
            Assert.IsNotNull(results);
        }

        [TestMethod]
        public void ResultsGotItems()
        {
            Assert.IsTrue(results.Items.Count > 0);
        }

        [TestMethod]
        public void ResultsGotTotalResults()
        {
            Assert.IsTrue(results.TotalResults > 0 && results.TotalResults >= results.Items.Count);
        }

        [TestMethod]
        public void ResultsItemsHaveFields()
        {
            //this should be broken into individual functions probably...
            foreach(var item in results.Items)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Name));
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.ID) && (item.ID.Length == 6 || item.ID.Length == 7));
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.SKU) && item.SKU.Length == 6);
                Assert.IsTrue(item.Price > 0);
                Assert.IsTrue(item.OriginalPrice > 0);
                Assert.IsNotNull(item.PictureUrls);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Stock));
                //
                // Brand can actually be empty ex https://www.microcenter.com/product/5003878/-amd-ryzen-5-3600-with-wraith-stealth-cooler,-asus-b450m-a-csm-prime,-cpu---motherboard-bundle
                // CPU+Mobo Bundles
                //Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Brand));
                //
                //
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.URL));
                Assert.IsTrue(Regex.Match(item.URL, "/product/\\d{6,7}/.*").Success);
            }
        }

        [TestMethod]
        public void ResultsLoadAll()
        {
            var allResults = Search.LoadAll(QUERY, STORE, null, Search.OrderByMode.pricelow).Result;
            Assert.IsTrue(allResults.Items.Count == allResults.TotalResults);
        }
    }
}
