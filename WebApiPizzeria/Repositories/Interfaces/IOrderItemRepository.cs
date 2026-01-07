using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Repositories.Interfaces;

public interface IOrderItemRepository
{
    Task SaveRange(OrderItemPostDto dto);
    Task<OrderItemDto> GetByOrderId(int orderId);
    Task DeleteOrderItems(int orderId, List<int> productIds);
    Task<bool> UpdateOrderItem(int orderId, int productId, decimal quantity);
}

