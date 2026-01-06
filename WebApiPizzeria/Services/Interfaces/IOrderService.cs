using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Services.Interfaces;

public interface IOrderService
{
    Task<AddItemResponse> AddItemToOrderAsync(AddItemRequest request);
    Task<ServiceResponse> RemoveItemFromOrderAsync(int orderId, int productId);
    Task<ServiceResponse> CancelOrderAsync(int orderId);
}
