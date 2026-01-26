namespace WebApiPizzeria.DTOs;

public class OrderManageResponseDto
{
    public OrderAction Action { get; set; }
    public int OrderId { get; set; }
    public List<OrderItemDetailDto> Items { get; set; } = new();
    public decimal Total { get; set; }
}

