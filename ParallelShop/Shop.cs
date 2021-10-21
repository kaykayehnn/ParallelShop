using System;
using System.Collections.Generic;
using System.Linq;

namespace ParallelShop
{
    public class Shop
    {
        // Stores products and available quantities
        private Dictionary<Product, double> products;
        private Dictionary<Product, object> productLocks;

        public Shop()
        {
            this.products = new Dictionary<Product, double>();
            this.productLocks = new Dictionary<Product, object>();
        }

        public void ProcessDelivery(Delivery delivery)
        {
            foreach (var product in delivery.Shipments)
            {
                this.AddProduct(product);
            }
        }

        public void ProcessOrder(Order order)
        {
            foreach (var product in order.Items)
            {
                this.RemoveProduct(product);
            }
        }

        public Dictionary<Product, double> GetAvailableProducts()
        {
            lock (this.products)
            {
                var copy = new Dictionary<Product, double>(this.products);
                return copy;
            }
        }

        private void AddProduct(ProductShipment shipment)
        {
            this.UpdateProductQuantity(shipment.Product, shipment.Quantity);
        }

        private void RemoveProduct(OrderItem item)
        {
            this.UpdateProductQuantity(item.Product, -item.Quantity);
        }

        private void UpdateProductQuantity(Product product, double quantityChange)
        {
            object lockObj = null;

            // First check if we have the product in the dictionary
            lock (this.productLocks)
            {
                bool containsProduct = this.productLocks.TryGetValue(product, out lockObj);
                // Create a lock object if this is the first time we encounter
                // the product and add it to the dictionary.
                if (!containsProduct)
                {
                    lockObj = new object();
                    this.productLocks[product] = lockObj;
                    this.products[product] = 0;
                }
            }

            lock (lockObj)
            {
                var value = this.products[product];

                if (value + quantityChange < 0)
                {
                    throw new ArgumentException(
$@"There is not enough {product} available to complete this operation.
Available quantity: {value}");
                }

                this.products[product] = value + quantityChange;
            }
        }
    }
}
