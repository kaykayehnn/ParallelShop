using System;

namespace ParallelShop
{
    class Program
    {
        // Shop system
        // Can accept orders (item, quantity) from multiple clients at once
        // Can get shipments (item, quantity) from multiple places at once

        // Models:
        // Shipment
        // Customer
        // Product
        static void Main(string[] args)
        {
            var shop = new Shop();

            var delivery = new Delivery(new[]
            {
                new ProductShipment(new Product("Apples"), 100),
                new ProductShipment(new Product("Bananas"), 100),
                new ProductShipment(new Product("Milk"), 100),
            });

            ParallelShopProcessor psp = new ParallelShopProcessor(shop, 100, 5);
            
            psp.Run(new[] { delivery }, new Order[] { });
            
            psp.Run(new[] { delivery }, new[] {new Order(new[]
            {
                new OrderItem(new Product("Apples"), 100),
                new OrderItem(new Product("Bananas"), 100),
                new OrderItem(new Product("Milk"), 100),
            })});
        }
    }
}
