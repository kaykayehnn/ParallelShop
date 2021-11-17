using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace ParallelShop
{
    public class Shop
    {
        // Stores products and available quantities
        private Dictionary<Product, double> products;
        // ProductLocks acts as an intermediary before acquiring any row locks.
        // In order to acquire a row lock, we MUST first lock productLocks and
        // fetch the required locks.
        // This ensures it is not possible to modify existing rows while an add
        // operation is in progress.
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
            return LockAllProducts(() =>
            {
                // Now that no changes can be made to the dictionary, clone it
                // and return the new copy.
                var copy = new Dictionary<Product, double>(this.products);
                return copy;
            });
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
                    // We are modifying the products dictionary so we have to lock
                    // all product rows in order to insert a new product.
                    lockObj = LockAllProducts(() =>
                    {
                        lockObj = new object();
                        this.productLocks[product] = lockObj;
                        this.products[product] = 0;
                        return lockObj;
                    });
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

        private T LockAllProducts<T>(Func<T> func)
        {
            // Prevent any threads from acquiring any lock objects until we are done.
            lock (this.productLocks)
            {
                // Keep the initially locked rows in an array, in order not to
                // call Monitor.Exit with locks which had not been locked previously.
                Product[] rowsToLock = null;
                try
                {
                    rowsToLock = productLocks
                        .Select(kvp => kvp.Key)
                        .ToArray();
                    // Lock all rows of the dictionary, thus locking the entire dictionary
                    foreach (var productLock in rowsToLock)
                    {
                        Monitor.Enter(productLock);
                    }

                    return func();
                }
                finally
                {
                    // Unlock all rows
                    foreach (var productLock in rowsToLock)
                    {
                        Monitor.Exit(productLock);
                    }
                }
            }
        }
    }
}
