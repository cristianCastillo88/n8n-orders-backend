namespace WebApiPizzeria.Models
{
    public class ProductConfiguration
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; } = null!;
        public int ChildProductId { get; set; } 
        public virtual Product ChildProduct { get; set; } = null!;
        public int Quantity { get; set; }
    }
}
