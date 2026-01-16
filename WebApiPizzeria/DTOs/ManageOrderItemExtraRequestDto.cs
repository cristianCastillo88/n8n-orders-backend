namespace WebApiPizzeria.DTOs
{
    public class ManageOrderItemExtraRequestDto
    {
        public int OrderId { get; set; }
        public int OrderItemId { get; set; }
        public int ExtraTypeId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public decimal Quantity { get; set; } = 1;
    }
}
