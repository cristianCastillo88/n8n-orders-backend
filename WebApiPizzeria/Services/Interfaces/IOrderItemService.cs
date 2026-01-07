using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Services.Interfaces;

public interface IOrderItemService
{
    Task<BaseResponseDto<OrderItemDto>> SaveRange(OrderItemPostDto dto, CancellationToken cancellationToken);
    Task<BaseResponseDto<OrderItemDto>> DeleteOrderItems(DeleteOrderItemRequestDto dto, CancellationToken cancellationToken);
    Task<BaseResponseDto<OrderItemDto>> UpdateOrderItem(UpdateOrderItemRequestDto dto, CancellationToken cancellationToken);
}

