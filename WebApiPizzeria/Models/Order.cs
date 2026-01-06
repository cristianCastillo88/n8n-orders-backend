using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class Order
{
    public int Id { get; set; }

    public int OrderNumber { get; set; }

    public DateTime CreatedAt { get; set; }

    public string UserId { get; set; } = null!;

    public decimal? Total { get; set; }

    public int? OrderStateTypeId { get; set; }

    public DateTime? LastUpdate { get; set; }

    public virtual OrderStateType? OrderStateType { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
