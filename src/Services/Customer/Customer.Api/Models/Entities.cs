namespace Customer.Api.Models;

// Entities from the starter kit in doc/aspnet-mock-interview-exercises.md.
// They have no navigation properties on purpose, so the JSON matches the exercise test cases.

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Sku { get; set; } = "";
    public string Category { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
    public bool IsDeleted { get; set; }
    public int Version { get; set; } = 1;     // used for ETags
}

// Named CustomerEntity in C# because "Customer" is also the root namespace, so a class
// called Customer can't be referenced from Customer.Api.* code. The table is "Customers".
public class CustomerEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Balance { get; set; }
    public DateTime Birthday { get; set; }     // date only; stored as SQL "date" (GET-6)
}

public enum OrderStatus { Pending, Paid, Shipped, Delivered, Cancelled }

public class Order
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
}

public class User
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class Payment
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

// The starter kit keeps these two in dictionaries. As tables:
// Settings: key -> value (PUT-3). CartItems: cartId + productId (DELETE-2).
public class Setting
{
    public string Key { get; set; } = "";
    public string Value { get; set; } = "";
}

public class CartItem
{
    public int CartId { get; set; }
    public int ProductId { get; set; }
}
