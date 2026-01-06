using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class DayType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<ProductAvailabilityDay> ProductAvailabilityDays { get; set; } = new List<ProductAvailabilityDay>();
}
