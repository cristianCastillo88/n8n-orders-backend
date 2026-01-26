using WebApiPizzeria.DTOs;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Interfaces;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IOrderItemService _orderItemService;

    public OrderService(
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        IOrderItemService orderItemService)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _orderItemService = orderItemService;
    }

    private async Task<Models.Order> EnsureActiveOrderAsync(string userId, int orderId)
    {
        // Si se especifica un orderId > 0, intentar encontrarlo primero
        if (orderId > 0)
        {
            var specificOrder = await _orderRepository.GetLastOrderByUserId(userId);
            if (specificOrder != null && specificOrder.Id == orderId)
            {
                return specificOrder;
            }
        }

        // Buscar orden activa o crear nueva
        // Esto se ejecuta cuando orderId es 0 o cuando no se encuentra la orden específica
        var activeOrder = await _orderRepository.GetLastOrderByUserId(userId);
        if (activeOrder == null)
        {
            activeOrder = await _orderRepository.Save(userId);
        }
        return activeOrder;
    }

    private decimal CalculateTotal(List<OrderItemDetailDto> items)
    {
        decimal total = 0;
        foreach (var item in items)
        {
            total += item.Price * item.Quantity;
        }
        return total;
    }

    public async Task<BaseResponseDto<OrderManageResponseDto>> ManageOrder(OrderManageRequestDto dto, CancellationToken cancellationToken)
    {
        try
        {
            // Validar campos requeridos según action
            if (string.IsNullOrWhiteSpace(dto.UserId))
            {
                return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
            }

            Models.Order order;
            OrderItemDto orderItemDto;

            switch (dto.Action)
            {
                case OrderAction.ADD_PRODUCTS:
                    if (dto.Products == null || !dto.Products.Any())
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    // UserId ya viene como string desde WhatsApp, lo pasamos directamente
                    var saveDto = new OrderItemPostDto
                    {
                        OrderId = dto.OrderId,
                        UserId = dto.UserId,
                        Products = dto.Products
                    };

                    var saveResult = await _orderItemService.SaveRange(saveDto, cancellationToken);
                    if (!saveResult.Success)
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    // SaveRange ya actualiza saveDto.OrderId, usamos ese para la respuesta
                    order = await EnsureActiveOrderAsync(dto.UserId, saveDto.OrderId);
                    orderItemDto = saveResult.Response;
                    break;

                case OrderAction.REMOVE_PRODUCTS:
                    if (dto.ProductIds == null || !dto.ProductIds.Any())
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    order = await EnsureActiveOrderAsync(dto.UserId, dto.OrderId);
                    if (order == null)
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    var deleteDto = new DeleteOrderItemRequestDto
                    {
                        OrderId = order.Id,
                        UserId = dto.UserId,
                        ProductIds = dto.ProductIds
                    };

                    // DeleteOrderItems ya valida orden no vacía internamente
                    var deleteResult = await _orderItemService.DeleteOrderItems(deleteDto, cancellationToken);
                    if (!deleteResult.Success)
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    orderItemDto = deleteResult.Response;
                    break;

                case OrderAction.UPDATE_QUANTITY:
                    if (dto.Updates == null || !dto.Updates.Any())
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    order = await EnsureActiveOrderAsync(dto.UserId, dto.OrderId);
                    if (order == null)
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    // Actualizar cada item (cada uno maneja su propia transacción)
                    BaseResponseDto<OrderItemDto>? lastUpdateResult = null;
                    foreach (var update in dto.Updates)
                    {
                        var updateDto = new UpdateOrderItemRequestDto
                        {
                            OrderId = order.Id,
                            UserId = dto.UserId,
                            ProductId = update.ProductId,
                            Quantity = update.Quantity
                        };

                        lastUpdateResult = await _orderItemService.UpdateOrderItem(updateDto, cancellationToken);
                        if (!lastUpdateResult.Success)
                        {
                            return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                        }
                    }

                    // Usar el resultado del último update (ya tiene el total actualizado)
                    orderItemDto = lastUpdateResult!.Response;
                    break;

                case OrderAction.GET_ORDER:
                    order = await EnsureActiveOrderAsync(dto.UserId, dto.OrderId);
                    if (order == null)
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    orderItemDto = await _orderItemRepository.GetByOrderId(order.Id);
                    orderItemDto.Total = CalculateTotal(orderItemDto.Items);
                    break;

                case OrderAction.CLOSE_ORDER:
                    order = await EnsureActiveOrderAsync(dto.UserId, dto.OrderId);
                    if (order == null)
                    {
                        return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
                    }

                    // Por ahora solo retornamos la orden (cambiar estado se hará después)
                    orderItemDto = await _orderItemRepository.GetByOrderId(order.Id);
                    orderItemDto.Total = CalculateTotal(orderItemDto.Items);
                    break;

                default:
                    return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
            }

            var response = new OrderManageResponseDto
            {
                Action = dto.Action,
                OrderId = order.Id,
                Items = orderItemDto.Items,
                Total = orderItemDto.Total
            };

            return new BaseResponseDto<OrderManageResponseDto>(true, response);
        }
        catch (Exception)
        {
            return new BaseResponseDto<OrderManageResponseDto>(false, new OrderManageResponseDto());
        }
    }
}

