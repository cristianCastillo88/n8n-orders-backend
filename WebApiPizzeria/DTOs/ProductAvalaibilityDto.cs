namespace WebApiPizzeria.DTOs;

public class ProductAvalaibilityDto
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public string ProductTypeName { get; set; } = null!;
    public decimal Price { get; set; }
}

