namespace LinqAdvancedLab;

    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public decimal Amount { get; set; }
        public List<string> Items { get; set; }
        public Order(int id, int customerId, decimal amount, List<string> items)
        {
            Id = id;
            CustomerId = customerId;
            Amount = amount;
            Items = items;
        }
    }