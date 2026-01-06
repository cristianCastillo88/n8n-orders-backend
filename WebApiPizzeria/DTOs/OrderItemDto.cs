namespace WebApiPizzeria.DTOs;

public class OrderItemDto
{
    public List<OrderItemDetailDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}

