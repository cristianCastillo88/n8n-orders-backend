namespace WebApiPizzeria.DTOs;

public class OrderItemPostDto
{
    public int OrderId { get; set; }
    public int UserId { get; set; }
    public List<ProductItemPostDto> Products { get; set; } = new();
}

