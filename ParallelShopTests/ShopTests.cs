using System;
using NUnit.Framework;
using ParallelShop;

namespace ParallelShopTests
{
    class ShopTests
    {
        private Shop shop;
        private Product[] products;

        [SetUp]
        public void Setup()
        {
            this.shop = new Shop();

            // It is important that all products are unique for the tests.
            products = new[]
            {
                new Product("Apples"),
                new Product("Bananas"),
                new Product("Oranges"),
                new Product("Milk"),
                new Product("Bread"),
                new Product("TVs"),
                new Product("iPhones"),
                new Product("Samsungs"),
                new Product("Cows"),
            };
        }

        [Test]
        public void DoesNotAllowNegativeAvailableQuantity()
        {
            var product = this.products[0];
            shop.ProcessDelivery(new Delivery(product, 10));

            Assert.Throws<ArgumentException>(() =>
                shop.ProcessOrder(new Order(product, 11)));
        }

        [Test]
        public void UpdatesAvailableQuantityAfterOrder()
        {
            var product = this.products[0];

            shop.ProcessDelivery(new Delivery(product, 10));
            shop.ProcessOrder(new Order(product, 5));

            var available = shop.GetAvailableProducts();

            Assert.AreEqual(5, available[product]);
        }
    }
}
