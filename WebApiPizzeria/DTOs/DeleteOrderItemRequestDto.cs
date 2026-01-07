namespace WebApiPizzeria.DTOs
{
    public class DeleteOrderItemRequestDto
    {
        public int OrderId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public List<int> ProductIds { get; set; } = new();
    }
}
