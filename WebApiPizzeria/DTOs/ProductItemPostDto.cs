namespace WebApiPizzeria.DTOs;

public class ProductItemPostDto
{
    public int Id { get; set; }
    public decimal Quantity { get; set; }
    public List<ProductItemPostDto>? SubProducts { get; set; }
}

