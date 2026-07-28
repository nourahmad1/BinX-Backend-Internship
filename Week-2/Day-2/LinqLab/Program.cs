using LinqAdvancedLab;
var customers = new List<Customer>
{
    new Customer(1, "Nour"),
    new Customer(2, "Ola"),
    new Customer(3, "Sara"),
    new Customer(4, "Almad"),
    new Customer(5, "Lina"),
    new Customer(6, "Omar")


};
var orders = new List<Order>
{
    new Order
    (
        1,
        1,
        100,
        new List<string> { "Laptop", "Mouse" }
    ),

    new Order(
        2,
        1,
        50,
        new List<string> { "Keyboard" }
        ),

    new Order(
        3, 
        2, 
        200,
        new List<string> { "Phone", "Charger" }
        ),
    new Order(
        4, 
        2, 
        80,
        new List<string> { "Headset" }
    ),
    new Order(
        5, 
        3, 
        150,
        new List<string> { "Tablet" }
    ),
    new Order(
        6, 
        4, 
        70,
        new List<string> { "USB" }
    ),    
};

    Console.WriteLine("GroupBy Result:");
    var totalOrders= orders.GroupBy(o => o.CustomerId)
        .Select(g => new
        {
            CustomerId = g.Key,
            TotalAmount = g.Sum(o => o.Amount),
        });
        foreach(var customer in totalOrders)
        {
            Console.WriteLine($"CustomerId: {customer.CustomerId}, TotalAmount: {customer.TotalAmount}");
        }
    Console.WriteLine("Join Result:");
    var customerOrders = customers.Join(orders, customer => customer.Id, order => order.CustomerId, (customer, order) => new
    {
        CustomerName = customer.Name,
        OrderAmount = order.Amount
    });

    foreach(var customerOrder in customerOrders)
    {
        Console.WriteLine($"CustomerName: {customerOrder.CustomerName}, OrderAmount: {customerOrder.OrderAmount}");
    }
    Console.WriteLine("SelectMany Result:");
    var allItems = orders.SelectMany(order=> order.Items);
    foreach(var item in allItems)
    {
        Console.WriteLine($"Item: {item}");
    }
    Console.WriteLine("Distinct Result:");
    var distinctItems = orders.SelectMany(order => order.Items).Distinct();
    foreach(var item in distinctItems)
    {
        Console.WriteLine($"Item: {item}");
    }
    Console.WriteLine("Deferred Result:");
    var numbers= new List<int> { 1, 2, 3, 4, 5 };
    var query= numbers.Where(n => n > 3);
    numbers.Add(10);
    foreach(var number in query)
    {
        Console.WriteLine($"Number: {number}");
    }