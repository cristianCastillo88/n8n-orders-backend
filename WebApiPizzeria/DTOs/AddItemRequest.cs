namespace WebApiPizzeria.DTOs;

public class AddItemRequest
{
    public int OrderId { get; set; }
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
    public string UserId { get; set; } = null!;
}
