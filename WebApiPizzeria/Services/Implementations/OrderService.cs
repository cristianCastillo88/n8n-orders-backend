using AutoMapper;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Interfaces;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IMapper _mapper;

    public OrderService(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<AddItemResponse> AddItemToOrderAsync(AddItemRequest request)
    {

        var response = new BaseResponseDto<List<AddItemResponse>>(false, null);
        response.Success = true;
        response.Response = new List<AddItemResponse> {
            new AddItemResponse
            {
                Success = true,
                Message = "Item agregado exitosamente",
                NewTotal = 0,
                Currency = "USD"
            }
        };
        // 1. Validar que el producto existe
        var product = await _orderRepository.GetProductByIdAsync(request.ProductId);
        if (product == null)
        {
            return new AddItemResponse
            {
                Success = false,
                Message = "Producto no encontrado",
                NewTotal = null,
                Currency = "USD"
            };
        }

        // 2. Validar que la orden existe
        var order = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        if (order == null)
        {
            return new AddItemResponse
            {
                Success = false,
                Message = "Orden no encontrada",
                NewTotal = null,
                Currency = "USD"
            };
        }

        // 3. Buscar si el item ya existe en la orden
        var existingItem = await _orderRepository.GetOrderItemAsync(request.OrderId, request.ProductId);

        if (existingItem != null)
        {
            // Caso A: El item existe - actualizar cantidad manualmente
            existingItem.Quantity += request.Quantity;

            // Si la cantidad es <= 0, eliminar el item
            if (existingItem.Quantity <= 0)
            {
                _orderRepository.RemoveOrderItem(existingItem);
            }
        }
        else
        {
            // Caso B: El item no existe - crear uno nuevo usando AutoMapper
            var newOrderItem = _mapper.Map<OrderItem>(request);

            await _orderRepository.AddOrderItemAsync(newOrderItem);
        }

        // 4. Guardar cambios
        var saved = await _orderRepository.SaveChangesAsync();
        if (!saved)
        {
            return new AddItemResponse
            {
                Success = false,
                Message = "Error al guardar los cambios",
                NewTotal = null,
                Currency = "USD"
            };
        }

        // 5. Recuperar la orden actualizada y recalcular el total
        var updatedOrder = await _orderRepository.GetOrderByIdAsync(request.OrderId);
        if (updatedOrder != null)
        {
            // Calcular el total usando la relación con Product
            updatedOrder.Total = updatedOrder.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price);
            updatedOrder.LastUpdate = DateTime.UtcNow;
            await _orderRepository.SaveChangesAsync();
        }

        // 6. Mapear a respuesta usando AutoMapper
        var response = _mapper.Map<AddItemResponse>(updatedOrder);

        return response;
    }

    public async Task<ServiceResponse> RemoveItemFromOrderAsync(int orderId, int productId)
    {
        // 1. Validar que la orden existe
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return new ServiceResponse
            {
                Success = false,
                Message = "Orden no encontrada",
                NewTotal = null,
                Currency = "USD"
            };
        }

        // 2. Buscar el item en la orden
        var orderItem = await _orderRepository.GetOrderItemAsync(orderId, productId);
        if (orderItem == null)
        {
            return new ServiceResponse
            {
                Success = false,
                Message = "Item no encontrado en la orden",
                NewTotal = order.Total,
                Currency = "USD"
            };
        }

        // 3. Eliminar el item físicamente
        _orderRepository.DeleteOrderItem(orderItem);

        // 4. Guardar cambios
        var saved = await _orderRepository.SaveChangesAsync();
        if (!saved)
        {
            return new ServiceResponse
            {
                Success = false,
                Message = "Error al guardar los cambios",
                NewTotal = null,
                Currency = "USD"
            };
        }

        // 5. Recalcular el total de la orden
        var updatedOrder = await _orderRepository.GetOrderByIdAsync(orderId);
        if (updatedOrder != null)
        {
            updatedOrder.Total = updatedOrder.OrderItems.Any() 
                ? updatedOrder.OrderItems.Sum(oi => oi.Quantity * oi.Product.Price)
                : 0;
            updatedOrder.LastUpdate = DateTime.UtcNow;
            _orderRepository.UpdateOrder(updatedOrder);
            await _orderRepository.SaveChangesAsync();
        }

        return new ServiceResponse
        {
            Success = true,
            Message = "Item eliminado exitosamente",
            NewTotal = updatedOrder?.Total,
            Currency = "USD"
        };
    }

    public async Task<ServiceResponse> CancelOrderAsync(int orderId)
    {
        // 1. Obtener la orden por ID
        var order = await _orderRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return new ServiceResponse
            {
                Success = false,
                Message = "Orden no encontrada",
                NewTotal = null,
                Currency = "USD"
            };
        }

        // 2. Validar que la orden esté en estado ABIERTO (ID = 1)
        if (order.OrderStateTypeId != 1)
        {
            var currentState = order.OrderStateTypeId switch
            {
                4 => "cancelada",
                _ => "en un estado que no permite cancelación"
            };

            return new ServiceResponse
            {
                Success = false,
                Message = $"No se puede cancelar la orden. La orden ya está {currentState}",
                NewTotal = order.Total,
                Currency = "USD"
            };
        }

        // 3. Cambiar el estado a CANCELADO (ID = 4)
        order.OrderStateTypeId = 4;
        order.LastUpdate = DateTime.UtcNow;
        _orderRepository.UpdateOrder(order);

        // 4. Guardar cambios
        var saved = await _orderRepository.SaveChangesAsync();
        if (!saved)
        {
            return new ServiceResponse
            {
                Success = false,
                Message = "Error al guardar los cambios",
                NewTotal = null,
                Currency = "USD"
            };
        }

        return new ServiceResponse
        {
            Success = true,
            Message = "Orden cancelada exitosamente",
            NewTotal = order.Total,
            Currency = "USD"
        };
    }
}
