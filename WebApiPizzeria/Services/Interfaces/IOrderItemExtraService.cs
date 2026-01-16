using WebApiPizzeria.DTOs;

namespace WebApiPizzeria.Services.Interfaces
{
    public interface IOrderItemExtraService
    {
        Task<BaseResponseDto<OrderItemDto>> ManageOrderItemExtra(ManageOrderItemExtraRequestDto dto, CancellationToken cancellationToken);
    }
}
