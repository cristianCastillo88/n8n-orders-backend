using System;
using System.Collections.Generic;

namespace WebApiPizzeria.Models;

public partial class ProductType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
