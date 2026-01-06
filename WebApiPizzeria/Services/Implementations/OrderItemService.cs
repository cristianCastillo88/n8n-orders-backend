using Microsoft.EntityFrameworkCore.Storage;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Interfaces;
using WebApiPizzeria.Services.Interfaces;

namespace WebApiPizzeria.Services.Implementations;

public class OrderItemService : IOrderItemService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly PostgresContext _context;

    public OrderItemService(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, PostgresContext context)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _context = context;
    }

    public async Task<BaseResponseDto<OrderItemDto>> SaveRange(OrderItemPostDto dto, CancellationToken cancellationToken)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            var userIdString = dto.UserId.ToString();
            var order = await _orderRepository.GetLastOrderByUserId(userIdString);

            if (order == null)
            {
                order = await _orderRepository.Save(userIdString);
            }

            dto.OrderId = order.Id;
            await _orderItemRepository.SaveRange(dto);

            var orderItemDto = await _orderItemRepository.GetByOrderId(order.Id);
            
            decimal total = 0;
            foreach (var item in orderItemDto.Items)
            {
                total += item.Price * item.Quantity;
            }

            await _orderRepository.UpdateTotal(order.Id, total);

            var responseOrderItemDto = await _orderItemRepository.GetByOrderId(order.Id);
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

