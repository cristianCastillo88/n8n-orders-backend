namespace WebApiPizzeria.Models
{
    public class OrderItemExtras
    {
        public int Id { get; set; }

        public int OrderItemId { get; set; }

        public int ExtraTypeId { get; set; }

        public decimal Price { get; set; }

        public decimal Quantity { get; set; }

        public virtual OrderItem OrderItem { get; set; } = null!;

        public virtual ExtraType ExtraType { get; set; } = null!;
    }
}
