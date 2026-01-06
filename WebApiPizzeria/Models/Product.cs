using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class Product
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public int ProductTypeId { get; set; }

    public decimal Price { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<ProductAvailabilityDay> ProductAvailabilityDays { get; set; } = new List<ProductAvailabilityDay>();

    public virtual ProductType ProductType { get; set; } = null!;
}
