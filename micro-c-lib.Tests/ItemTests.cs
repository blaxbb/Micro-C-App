using MicroCLib.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.InteropServices.ComTypes;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace micro_c_lib.Tests
{
    [TestClass]
    public class ItemTests
    {
        private Item item;
        public ItemTests()
        {
            item = Item.FromUrl("/product/486774/inland-air-duster-16-oz", "141").Result;
        }

        [TestMethod("Item not null")]
        public void FromUrlReturnsItemAsync()
        {
            Assert.IsNotNull(item);
        }

        [TestMethod("Item found")]
        public void FromUrlItemFound()
        {
            Assert.IsTrue(item.SKU != "000000");
        }

        [TestMethod("Item has name")]
        public void FromUrlSetsName ()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Name));
        }

        [TestMethod("Item has specs")]
        public void FromUrlHasSpecs()
        {
            Assert.IsNotNull(item.Specs);
            Assert.IsTrue(item.Specs.Count > 0);
        }

        [TestMethod("Item has SKU")]
        public void FromUrlHasSKU()
        {
            Assert.IsTrue(item.SKU.Length == 6);
        }

        [TestMethod("Item has price")]
        public void FromUrlHasPrice()
        {
            Assert.IsTrue(item.Price > 0f);
        }

        [TestMethod("Item has original price")]
        public void FromUrlHasOriginalPrice()
        {
            Assert.IsTrue(item.OriginalPrice > 0f);
        }


        [TestMethod("Item has URL")]
        public void FromUrlHasURL()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.URL));
            Assert.IsTrue(Regex.Match(item.URL, "/product/\\d{6}/.*").Success);
        }

        [TestMethod("Item has stock")]
        public void FromUrlHasStock()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Stock));
        }

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

        [TestMethod("Item has location")]
        public void FromUrlHasLocation()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Location));
        }

        [TestMethod("Item has ID")]
        public void FromUrlHasID()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.ID));
            Assert.IsTrue(item.ID.Length == 6);
        }

        [TestMethod("Item has brand")]
        public void FromUrlHasBrand()
        {
            Assert.IsTrue(!string.IsNullOrWhiteSpace(item.Brand));
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
    }
}
