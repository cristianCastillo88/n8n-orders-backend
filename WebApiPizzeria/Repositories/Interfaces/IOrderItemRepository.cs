using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Repositories.Interfaces;

public interface IOrderItemRepository
{
    Task SaveRange(OrderItemPostDto dto);
    Task<OrderItemDto> GetByOrderId(int orderId);
}

