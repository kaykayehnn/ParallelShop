namespace ParallelShop
{
    public class OrderItem
    {
        public OrderItem(Product product, double quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public Product Product { get; }

        public double Quantity { get; }
    }
}
