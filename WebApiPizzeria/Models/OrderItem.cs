using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class OrderItem
{
    public int Id { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }
    public int? ParentOrderItemId { get; set; }

    public virtual Order Order { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
    public virtual OrderItem? ParentOrderItem { get; set; }
    public virtual ICollection<OrderItem> ChildOrderItems { get; set; } = new List<OrderItem>();
    public virtual ICollection<OrderItemExtras> OrderItemExtras { get; set; } = new List<OrderItemExtras>();
}
