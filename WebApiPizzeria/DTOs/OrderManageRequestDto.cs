namespace WebApiPizzeria.DTOs;

public class OrderManageRequestDto
{
    public OrderAction Action { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int OrderId { get; set; }
    public List<ProductItemPostDto>? Products { get; set; }
    public List<int>? ProductIds { get; set; }
    public List<UpdateItemDto>? Updates { get; set; }
}

public class UpdateItemDto
{
    public int ProductId { get; set; }
    public decimal Quantity { get; set; }
}

