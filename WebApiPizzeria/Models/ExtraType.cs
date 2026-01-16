namespace WebApiPizzeria.Models
{
    public class ExtraType
    {
        public int Id { get; set; }

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public virtual ICollection<OrderItemExtras> OrderItemExtras { get; set; } = new List<OrderItemExtras>();
    }
}
