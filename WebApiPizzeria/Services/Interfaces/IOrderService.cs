using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Services.Interfaces;

public interface IOrderService
{
    Task<BaseResponseDto<OrderManageResponseDto>> ManageOrder(OrderManageRequestDto dto, CancellationToken cancellationToken);
}

