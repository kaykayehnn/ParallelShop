namespace ParallelShop
{
    public class Order
    {
        public Order(Product product, double quantity)
        {
            Items = new[]
            {
                new OrderItem(product, quantity),
            };
        }
        public Order(OrderItem[] items)
        {
            Items = items;
        }

        public OrderItem[] Items { get; }
    }
}
