namespace WebApiPizzeria.DTOs;

public class OrderItemPostDto
{
    public int OrderId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<ProductItemPostDto> Products { get; set; } = new();
}

