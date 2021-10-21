namespace ParallelShop
{
    public class Delivery
    {
        public Delivery(Product product, double quantity)
        {
            Shipments = new[]
            {
                new ProductShipment(product, quantity)
            };
        }

        public Delivery(ProductShipment[] shipments)
        {
            Shipments = shipments;
        }

        public ProductShipment[] Shipments { get; }
    }
}
