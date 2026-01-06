using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class OrderStateType
{
    public int Id { get; set; }

    public string Description { get; set; } = null!;

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
}
