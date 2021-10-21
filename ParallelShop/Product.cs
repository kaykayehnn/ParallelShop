namespace ParallelShop
{
    public class Product
    {
        public Product(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            var temp = obj as Product;
            if (temp == null) return false;

            return this.Name.Equals(temp.Name);
        }
    }
}
