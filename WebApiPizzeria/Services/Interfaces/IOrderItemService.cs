using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Services.Interfaces;

public interface IOrderItemService
{
    Task<BaseResponseDto<OrderItemDto>> SaveRange(OrderItemPostDto dto, CancellationToken cancellationToken);
}

