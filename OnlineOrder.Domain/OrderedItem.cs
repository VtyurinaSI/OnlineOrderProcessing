using System.Diagnostics;

namespace OnlineOrder.Domain
{
    public class OrderedItem
    {
        public OrderedItem(string name, decimal price, int quantity = 1)
        {
            Name = name;
            Price = price > 0 ? price : throw new ArgumentException("Неверная цена товара");
            Quantity = quantity > 0 ? quantity : throw new ArgumentException("Неверное количество товара");
        }
        public string Name { get; }
        public decimal Price { get; }
        public int Quantity { get; private set; }
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        internal void IncreaseQuantity(int val = 1) => Quantity += val;

        internal bool DecreaseQuantity()
        {
            bool res = false;
            if (Quantity > 1)
            {
                Quantity--;
                res = true;
            }
            return res;
        }
    }
}