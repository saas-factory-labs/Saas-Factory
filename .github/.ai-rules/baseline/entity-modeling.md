<!-- # Entity Modeling

When implementing Domain-Driven Design (DDD) patterns, follow these rules very carefully.

## Structure

All DDD models like aggregates, entities, value objects, strongly typed IDs, repositories, and repository configurations should be created in the `/[scs-name]/Core/Features/[Feature]/Domain` directory.

## Domain Modeling Guidelines

Aggregates are the root of the DDD model, and map 1:1 to database tables. Repositories are used to read and write aggregates in the database. Aggregates can contain entities, but not other aggregates. E.g. `Order` and `Customer` in an e-commerce application are aggregates, but `OrderLine` is an entity owned by the `Order` aggregate. Value objects are used to model immutable values that are conceptually a single value, like `Address`, `Amount`, and `Email`.

By default entities and value objects should be stored as JSON columns on the Aggregate in the database. This might seem controversial, but it's very performant and allows for complex Aggregates without having to use EF Core's `Include()` method. And without risking having an Aggregate where a navigation property is not loaded. EF Core 8 and 9 have great support for complex LINQ queries on Json columns.

### Aggregate Implementation

1. Use public sealed classes for Aggregates and inherit from `AggregateRoot<TId>`.
2. Create a strongly typed ID for aggregates (in the same class); always consult [Strongly Typed IDs](./strongly-typed-ids.md) for details.
3. Never use navigational properties to other aggregates (e.g. don't use `User.Tenant`, or `Order.Customer`).
4. Use factory methods when creating new aggregates.
5. Make properties private, and use methods when changing state and enforcing business rules and invariants.
6. Create a corresponding repository interface and implementation; consult [Repository](./repositories.md) for details.

### Entity Implementation

1. Use public sealed classes for Entities and inherit from `Entity<TId>`.
2. Create a strongly typed ID for entities (in the same class); always consult [Strongly Typed IDs](./strongly-typed-ids.md) for details.
3. Use factory methods when creating new entities.
4. Use private setters to control state changes.
5. Make properties private, and use methods when changing state and enforcing business rules and invariants.
6. Store entities as JSON columns on the Aggregate in the database.

### Value Object Implementation

1. Use records for Value Objects to ensure immutability.
2. Value objects do not have an ID.
3. Store value objects as JSON columns on the Aggregate in the database.

## Example 1 - Simple Aggregate

```csharp
public sealed class User : AggregateRoot<UserId>, ITenantScopedEntity
{
    private string _email = string.Empty;

    private User(TenantId tenantId, string email, UserRole role)
        : base(UserId.NewId())
    {
        Email = email;
        TenantId = tenantId;
        Role = role;
        Avatar = new Avatar();
    }

    public string Email
    {
        get => _email;
        private set => _email = value.Trim().ToLowerInvariant();
    }

    public UserRole Role { get; private set; }

    public Avatar Avatar { get; private set; }

    public TenantId TenantId { get; }

    public static User Create(TenantId tenantId, string email, UserRole role)
    {
        return new User(tenantId, email, role);
    }

    public void UpdateEmail(string email)
    {
        Email = email;
    }

    public void ChangeUserRole(UserRole userRole)
    {
        Role = userRole;
    }

    public void UpdateAvatar(string avatarUrl, bool isGravatar)
    {
        Avatar = new Avatar(avatarUrl, Avatar.Version + 1, isGravatar);
    }

    public void RemoveAvatar()
    {
        Avatar = new Avatar(Version: Avatar.Version);
    }
}

public sealed record Avatar(string? Url = null, int Version = 0, bool IsGravatar = false);

[PublicAPI]
[IdPrefix("usr")]
[JsonConverter(typeof(StronglyTypedIdJsonConverter<string, UserId>))]
public sealed record UserId(string Value) : StronglyTypedUlid<UserId>(Value)
{
    public override string ToString()
    {
        return Value;
    }
}
```

## Example 2 - Complex Order Aggregate with OrderLine Entity and Address Value Object stored as JSON

```csharp
// Orders.cs
public sealed class Order : AggregateRoot<OrderId>
{
    private readonly List<OrderLine> _orderLines = new();
    
    private Order(CustomerId customerId, Address? address)
        : base(OrderId.NewId())
    {
        CustomerId = customerId;
        Address = address;
    }
    
    public CustomerId CustomerId { get; }
    
    public Address? Address { get; private set; }
    
    public IReadOnlyCollection<OrderLine> OrderLines => _orderLines.AsReadOnly();
    
    public decimal TotalAmount => _orderLines.Sum(line => line.TotalPrice);
    
    public static Order Create(CustomerId customerId)
    {
        return new Order(customerId, null);
    }
    
    public void AddOrderLine(ProductId productId, string productName, int quantity, decimal unitPrice)
    {
        var orderLine = OrderLine.Create(productId, productName, quantity, unitPrice);
        _orderLines.Add(orderLine);
    }

    public void SetAddress(Address address)
    {
        Address = address;
    }
}

[PublicAPI]
[IdPrefix("ord")]
[JsonConverter(typeof(StronglyTypedIdJsonConverter<string, OrderId>))]
public sealed record OrderId(string Value) : StronglyTypedUlid<OrderId>(Value);

public record Address(string Street, string City, string State, string ZipCode, string Country);

// OrderLine.cs
public sealed class OrderLine : Entity<OrderLineId>
{
    private OrderLine(OrderLineId id, ProductId productId, string productName, int quantity, decimal unitPrice)
        : base(id)
    {
        ProductId = productId;
        ProductName = productName;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
    
    public ProductId ProductId { get; }
    
    public string ProductName { get; }
    
    public int Quantity { get; }
    
    public decimal UnitPrice { get; }
    
    public decimal TotalPrice => UnitPrice * Quantity;
    
    internal static OrderLine Create(ProductId productId, string productName, int quantity, decimal unitPrice)
    {
        if (quantity <= 0) throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (unitPrice <= 0) throw new ArgumentException("Unit price must be positive", nameof(unitPrice));

        return new OrderLine(OrderLineId.NewId(), productId, productName, quantity, unitPrice);
    }
}

[PublicAPI]
[IdPrefix("ordln")]
[JsonConverter(typeof(StronglyTypedIdJsonConverter<string, OrderLineId>))]
public sealed record OrderLineId(string Value) : StronglyTypedUlid<OrderLineId>(Value);

// OrderConfiguration.cs
public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.MapStronglyTypedUlid<Order, OrderId>(o => o.Id);
        builder.MapStronglyTypedUlid<Order, CustomerId>(o => o.CustomerId);
        builder.OwnsOne(o => o.Address, b => b.ToJson());

        builder.OwnsMany(o => o.OrderLines, ob =>
        {
            ob.MapStronglyTypedUlid<OrderLine, OrderLineId>(ol => ol.Id);
            ob.MapStronglyTypedUlid<OrderLine, ProductId>(ol => ol.ProductId);
        });
    }
}
``` -->