using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class ProductAvailabilityDay
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int DayTypeId { get; set; }

    public virtual DayType DayType { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
