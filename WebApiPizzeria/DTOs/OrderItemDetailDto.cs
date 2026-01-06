namespace WebApiPizzeria.DTOs;

public class OrderItemDetailDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
}

