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

        const string QUERY = "motherboards";
        const string STORE = "141";

        public SearchTests()
        {
            results = Search.LoadQuery(QUERY, STORE, null, Search.OrderByMode.pricelow, 1).Result;
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
            foreach(var item in results.Items)
            {
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Name));
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.ID) && item.ID.Length == 6);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.SKU) && item.SKU.Length == 6);
                Assert.IsTrue(item.Price > 0);
                Assert.IsTrue(item.OriginalPrice > 0);
                Assert.IsNotNull(item.PictureUrls);
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Stock));
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Brand));
                Assert.IsTrue(!string.IsNullOrWhiteSpace(item.URL));
                Assert.IsTrue(Regex.Match(item.URL, "/product/\\d{6}/.*").Success);
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
