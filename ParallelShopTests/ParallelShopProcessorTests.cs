using NUnit.Framework;
using ParallelShop;

namespace ParallelShopTests
{
    public class ParallelShopProcessorTests
    {
        private ParallelShopProcessor psp;
        private Product[] products;

        [SetUp]
        public void Setup()
        {
            psp = new ParallelShopProcessor(new Shop(), 100, 5);

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
        public void TestOrders()
        {
            // Load 1 million units of each product
            var baseDelivery = this.GenerateDeliveries(1, this.products, 1_000_000);

            psp.Run(baseDelivery);

            // Generate 1 million orders, each ordering 1 unit of each product.
            var orders = this.GenerateOrders(1_000_000, this.products, 1);
            psp.Run(orders);

            // This should leave each product at 0 available quantity.
            var available = psp.Shop.GetAvailableProducts();
            foreach (var product in available)
            {
                Assert.AreEqual(0, product.Value);
            }
        }

        [Test]
        public void TestDeliveries()
        {
            // Generate 1 million deliveries, each stocking 1 unit of each product.
            var deliveries = this.GenerateDeliveries(1_000_000, this.products, 1);
            psp.Run(deliveries);

            // This should leave each product at 1 million available quantity.
            var available = psp.Shop.GetAvailableProducts();
            foreach (var product in available)
            {
                Assert.AreEqual(1_000_000, product.Value);
            }
        }

        [Test]
        public void TestOrdersAndDeliveries()
        {
            // Load 1 million units of each product
            var baseDelivery = this.GenerateDeliveries(1, this.products, 1_000_000);

            psp.Run(baseDelivery);

            // Generate 1 million orders, each ordering 1 unit of each product.
            var orders = this.GenerateOrders(1_000_000, this.products, 1);

            // Generate 1 million deliveries, each stocking 1 unit of each product.
            var deliveries = this.GenerateDeliveries(1_000_000, this.products, 1);
            psp.Run(deliveries, orders);

            // This should leave each product at 1 million available quantity.
            var available = psp.Shop.GetAvailableProducts();
            foreach (var product in available)
            {
                Assert.AreEqual(1_000_000, product.Value);
            }
        }

        private Order[] GenerateOrders(int orderCount, Product[] products, double quantity)
        {
            var orders = new Order[orderCount];
            for (int i = 0; i < orderCount; i++)
            {
                var items = new OrderItem[products.Length];
                for (int j = 0; j < items.Length; j++)
                {
                    items[j] = new OrderItem(products[j], quantity);
                }

                var order = new Order(items);
                orders[i] = order;
            }

            return orders;
        }

        private Delivery[] GenerateDeliveries(int deliveryCount, Product[] products, double quantity)
        {
            var deliveries = new Delivery[deliveryCount];

            for (int i = 0; i < deliveryCount; i++)
            {
                var shipments = new ProductShipment[products.Length];
                for (int j = 0; j < shipments.Length; j++)
                {
                    shipments[j] = new ProductShipment(products[j], quantity);
                }

                deliveries[i] = new Delivery(shipments);
            }

            return deliveries;
        }
    }
}