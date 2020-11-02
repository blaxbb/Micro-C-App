using MicroCLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net.Http;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace micro_c_lib.Tests
{
    [TestClass]
    public class ItemTests
    {
        private Item item;
        private string body;
        //
        //should have a list of different products that hit different conditions
        //
        private const string URL = "/product/622100/asus-rt-ax3000-ax3000-dual-band-gigabit-wireless-ax-router---w--aimesh-support";
        private const string STORE_ID = "141";
        public ItemTests()
        {
            item = Item.FromUrl(URL, STORE_ID).Result;
            using (HttpClient client = new HttpClient())
            {
                var response = client.GetAsync($"https://www.microcenter.com{URL}?storeid={STORE_ID}").Result;
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    body = response.Content.ReadAsStringAsync().Result;
                }
            }
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item not null")]
        public void FromUrlReturnsItemAsync()
        {
            Assert.IsNotNull(item);
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item found")]
        public void FromUrlItemFound()
        {
            Assert.IsTrue(item.SKU != "000000");
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has name")]
        public void FromUrlSetsName ()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Name));
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has specs")]
        public void FromUrlHasSpecs()
        {
            Assert.IsNotNull(item.Specs);
            Assert.IsTrue(item.Specs.Count > 0);
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has SKU")]
        public void FromUrlHasSKU()
        {
            Assert.IsTrue(item.SKU.Length == 6);
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has price")]
        public void FromUrlHasPrice()
        {
            Assert.IsTrue(item.Price > 0f);
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has original price")]
        public void FromUrlHasOriginalPrice()
        {
            Assert.IsTrue(item.OriginalPrice > 0f);
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has URL")]
        public void FromUrlHasURL()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.URL));
            Assert.IsTrue(Regex.Match(item.URL, "/product/\\d{6}/.*").Success);
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has stock")]
        public void FromUrlHasStock()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Stock));
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has picture URLs")]
        public void FromUrlHasPictures()
        {
            Assert.IsNotNull(item.PictureUrls);
            Assert.IsTrue(item.PictureUrls.Count > 0);
            foreach(var url in item.PictureUrls)
            {
                //Assert.IsTrue(Regex.Match(url, ))
            }
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has location")]
        public void FromUrlHasLocation()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Location));
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has ID")]
        public void FromUrlHasID()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.ID));
            Assert.IsTrue(item.ID.Length == 6);
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has brand")]
        public void FromUrlHasBrand()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Brand));
        }

        [TestCategory("FromUrl")]
        [TestMethod("Item has Coming Soon")]
        public void FromUrlHasComingSoon()
        {
            Assert.IsFalse(item.ComingSoon);
        }

        [TestMethod]
        public void CloneVerification()
        {
            var clone = item.CloneAndResetQuantity();
            Assert.AreEqual(item.Name, clone.Name);
            Assert.AreEqual(item.Price, clone.Price);
            Assert.AreEqual(item.OriginalPrice, clone.OriginalPrice);

            Assert.AreEqual(clone.Quantity, 1);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex ID")]
        public void RegexUrl()
        {
            Assert.AreEqual(Item.ParseURL(body), URL);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex ID")]
        public void RegexID()
        {
            Assert.AreEqual(Item.ParseIDFromURL(URL), "622100");
        }

        [TestCategory("Regex")]
        [TestMethod("Regex Name")]
        public void RegexName()
        {
            var name = Item.ParseName(body);
            Assert.IsNotNull(name);
            Assert.IsTrue(name.Length > 0);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex Brand")]
        public void RegexBrand()
        {
            var brand = Item.ParseBrand(body);
            Assert.IsNotNull(brand);
            Assert.IsTrue(brand.Length > 0);
        }
        [TestCategory("Regex")]
        [TestMethod("Regex SKU")]
        public void RegexSKU()
        {
            var sku = Item.ParseSKU(item);
            Assert.IsNotNull(sku);
            Assert.IsTrue(sku.Length == 6);
        }
        [TestCategory("Regex")]
        [TestMethod("Regex Specs")]
        public void RegexSpecs()
        {
            Assert.IsTrue(Item.ParseSpecs(body).Count > 1);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex Stock")]
        public void RegexStock()
        {
            var stock = Item.ParseStock(body);
            Assert.IsNotNull(stock);
            Assert.IsTrue(stock.Length > 0);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex Price")]
        public void RegexPrice()
        {
            var price = Item.ParsePrice(body);
            Assert.IsTrue(price > 0f);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex Original Price")]
        public void RegexOriginalPrice()
        {
            ////////////////////////
            var price = Item.ParseOriginalPrice(body, item);
            Assert.IsTrue(price > 0f);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex Location")]
        public void RegexLocation()
        {
            var location = Item.ParseLocations(body);
            Assert.IsNotNull(location);
            Assert.IsTrue(location.Length > 0);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex Picture URLs")]
        public void RegexPictures()
        {
            var pictures = Item.ParsePictures(body);
            Assert.IsNotNull(pictures);
            Assert.IsTrue(pictures.Count > 0);
        }
        [TestCategory("Regex")]
        [TestMethod("Regex Plans")]
        public void RegexPlans()
        {
            var plans = Item.ParsePlans(body);
            Assert.IsNotNull(plans);
            Assert.IsTrue(plans.Count > 0);
        }

        [TestCategory("Regex")]
        [TestMethod("Regex Coming Soon")]
        public void RegexComingSoon()
        {
            var comingSoon = Item.ParseComingSoon(body);
            Assert.IsFalse(comingSoon);
        }
    }
}