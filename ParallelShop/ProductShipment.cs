namespace ParallelShop
{
    public class ProductShipment
    {
        public ProductShipment(Product product, double quantity)
        {
            Product = product;
            Quantity = quantity;
        }

        public Product Product { get; }
        public double Quantity { get; }
    }
}
