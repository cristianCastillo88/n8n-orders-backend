using WebApiPizzeria.DTOs;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Interfaces;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Services.Implementations
{
    public class OrderItemExtraService : IOrderItemExtraService
    {
        private readonly IOrderItemExtraRepository _orderItemExtraRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly PostgresContext _context;

        public OrderItemExtraService(
            IOrderItemExtraRepository orderItemExtraRepository,
            IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository,
            PostgresContext context)
        {
            _orderItemExtraRepository = orderItemExtraRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _context = context;
        }

        public async Task<BaseResponseDto<OrderItemDto>> ManageOrderItemExtra(ManageOrderItemExtraRequestDto dto, CancellationToken cancellationToken)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

            try
            {
                var isValidOrder = await _orderRepository.ValidateOrderOwnershipAndState(dto.OrderId, dto.UserId);
                if (!isValidOrder)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return new BaseResponseDto<OrderItemDto>(false, new OrderItemDto());
                }

                var orderItem = await _orderItemExtraRepository.GetOrderItemWithProduct(dto.OrderItemId, dto.OrderId);
                if (orderItem == null)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return new BaseResponseDto<OrderItemDto>(false, new OrderItemDto());
                }

                if (orderItem.ParentOrderItemId == null && orderItem.ChildOrderItems != null && orderItem.ChildOrderItems.Any())
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return new BaseResponseDto<OrderItemDto>(false, new OrderItemDto());
                }

                if (orderItem.Product == null ||
                    !orderItem.Product.Description.ToLower().Contains("pizza"))
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return new BaseResponseDto<OrderItemDto>(false, new OrderItemDto());
                }

                var extraTypeExists = await _orderItemExtraRepository.ExtraTypeExists(dto.ExtraTypeId);
                if (!extraTypeExists)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    return new BaseResponseDto<OrderItemDto>(false, new OrderItemDto());
                }

                if (dto.Action.ToLower() == "agregar")
                {
                    var exists = await _orderItemExtraRepository.ExtraExists(dto.OrderItemId, dto.ExtraTypeId);
                    if (exists)
                    {
                        await transaction.RollbackAsync(cancellationToken);
                        return new BaseResponseDto<OrderItemDto>(false, new OrderItemDto());
                    }

                    var extraPrice = await _orderItemExtraRepository.GetExtraTypePrice(dto.ExtraTypeId);

                    await _orderItemExtraRepository.AddExtra(dto.OrderItemId, dto.ExtraTypeId, extraPrice, dto.Quantity);
                }
                else if (dto.Action.ToLower() == "eliminar")
                {
                    await _orderItemExtraRepository.RemoveExtra(dto.OrderItemId, dto.ExtraTypeId);
                }

                var orderItemDto = await _orderItemRepository.GetByOrderId(dto.OrderId);

                decimal productsTotal = 0;
                foreach (var item in orderItemDto.Items)
                {
                    productsTotal += item.Price * item.Quantity;
                }

                decimal extrasTotal = await _orderItemExtraRepository.GetOrderExtrasTotal(dto.OrderId);

                decimal total = productsTotal + extrasTotal;

                await _orderRepository.UpdateTotal(dto.OrderId, total);

                var responseOrderItemDto = await _orderItemRepository.GetByOrderId(dto.OrderId);
                responseOrderItemDto.Total = total;

                await transaction.CommitAsync(cancellationToken);

                return new BaseResponseDto<OrderItemDto>(true, responseOrderItemDto);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                return new BaseResponseDto<OrderItemDto>(false, new OrderItemDto());
            }
        }
    }
}
