using System.Collections.Generic;
using System.Threading;

namespace ParallelShop
{
    public class ParallelShopProcessor
    {
        private int orderConcurrency;
        private int shipmentConcurrency;

        private const int DEFAULT_ORDER_CONCURRENCY = 100;
        private const int DEFAULT_SHIPMENT_CONCURRENCY = 5;

        public ParallelShopProcessor(Shop shop) :
            this(shop, DEFAULT_ORDER_CONCURRENCY, DEFAULT_SHIPMENT_CONCURRENCY)
        { }

        public ParallelShopProcessor(Shop shop, int orderConcurrency, int shipmentConcurrency)
        {
            this.Shop = shop;
            this.shipmentConcurrency = shipmentConcurrency;
            this.orderConcurrency = orderConcurrency;
        }

        public Shop Shop { get; }

        public void Run(Delivery[] deliveries)
        {
            this.Run(deliveries, new Order[] { });
        }

        public void Run(Order[] orders)
        {
            this.Run(new Delivery[] { }, orders);
        }

        public void Run(Delivery[] deliveries, Order[] orders)
        {
            var deliveriesThread = new Thread(() => ProcessDeliveries(deliveries));
            var ordersThread = new Thread(() => ProcessOrders(orders));

            deliveriesThread.Start();
            ordersThread.Start();

            deliveriesThread.Join();
            ordersThread.Join();
        }

        // Most of the logic from here is shared with ProcessDeliveries so it
        // can be extracted to a helper method.
        private void ProcessOrders(Order[] orders)
        {
            var queue = new Queue<Order>(orders);

            var threads = new Thread[this.orderConcurrency];
            for (int i = 0; i < threads.Length; i++)
            {
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        Order nextOrder;
                        lock (queue)
                        {
                            if (!queue.TryDequeue(out nextOrder))
                            {
                                return;
                            }
                        }

                        this.Shop.ProcessOrder(nextOrder);
                    }
                });

                thread.Start();
                threads[i] = thread;
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }

        private void ProcessDeliveries(Delivery[] deliveries)
        {
            var queue = new Queue<Delivery>(deliveries);

            var threads = new Thread[this.shipmentConcurrency];
            for (int i = 0; i < threads.Length; i++)
            {
                var thread = new Thread(() =>
                {
                    while (true)
                    {
                        Delivery nextDelivery;
                        lock (queue)
                        {
                            if (!queue.TryDequeue(out nextDelivery))
                            {
                                return;
                            }
                        }

                        this.Shop.ProcessDelivery(nextDelivery);
                    }
                });

                thread.Start();
                threads[i] = thread;
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
