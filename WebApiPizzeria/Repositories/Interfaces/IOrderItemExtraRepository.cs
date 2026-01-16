namespace WebApiPizzeria.Repositories.Interfaces
{
    public interface IOrderItemExtraRepository
    {
        Task<bool> ExtraExists(int orderItemId, int extraTypeId);
        Task AddExtra(int orderItemId, int extraTypeId, decimal price, decimal quantity);
        Task RemoveExtra(int orderItemId, int extraTypeId);
        Task<Models.OrderItem?> GetOrderItemWithProduct(int orderItemId, int orderId);
        Task<bool> ExtraTypeExists(int extraTypeId);
        Task<decimal> GetExtraTypePrice(int extraTypeId);
        Task<decimal> GetOrderExtrasTotal(int orderId);
    }
}
