namespace WebApiPizzeria.DTOs
{
    public class UpdateOrderItemRequestDto
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
    }
}
