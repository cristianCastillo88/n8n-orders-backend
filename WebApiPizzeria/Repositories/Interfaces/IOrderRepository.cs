using WebApiPizzeria.Models;

namespace WebApiPizzeria.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Product?> GetProductByIdAsync(int productId);
    Task<Order?> GetOrderByIdAsync(int orderId);
    Task<OrderItem?> GetOrderItemAsync(int orderId, int productId);
    Task AddOrderItemAsync(OrderItem orderItem);
    void RemoveOrderItem(OrderItem orderItem);
    void DeleteOrderItem(OrderItem orderItem);
    void UpdateOrder(Order order);
    Task<bool> SaveChangesAsync();
}
