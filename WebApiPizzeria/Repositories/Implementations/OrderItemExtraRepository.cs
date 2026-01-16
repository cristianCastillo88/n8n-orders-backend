using Microsoft.EntityFrameworkCore;
using WebApiPizzeria.Models;
using WebApiPizzeria.Repositories.Interfaces;

namespace WebApiPizzeria.Repositories.Implementations
{
    public class OrderItemExtraRepository : IOrderItemExtraRepository
    {
        private readonly PostgresContext _context;

        public OrderItemExtraRepository(PostgresContext context)
        {
            _context = context;
        }

        public async Task<bool> ExtraExists(int orderItemId, int extraTypeId)
        {
            return await _context.OrderItemExtras
                .AnyAsync(oie => oie.OrderItemId == orderItemId && oie.ExtraTypeId == extraTypeId);
        }

        public async Task AddExtra(int orderItemId, int extraTypeId, decimal price, decimal quantity)
        {
            var orderItemExtra = new OrderItemExtras
            {
                OrderItemId = orderItemId,
                ExtraTypeId = extraTypeId,
                Price = price,
                Quantity = quantity
            };

            await _context.OrderItemExtras.AddAsync(orderItemExtra);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveExtra(int orderItemId, int extraTypeId)
        {
            var orderItemExtra = await _context.OrderItemExtras
                .FirstOrDefaultAsync(oie => oie.OrderItemId == orderItemId && oie.ExtraTypeId == extraTypeId);

            if (orderItemExtra != null)
            {
                _context.OrderItemExtras.Remove(orderItemExtra);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<OrderItem?> GetOrderItemWithProduct(int orderItemId, int orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.ChildOrderItems)
                .FirstOrDefaultAsync(oi => oi.Id == orderItemId && oi.OrderId == orderId);
        }

        public async Task<bool> ExtraTypeExists(int extraTypeId)
        {
            return await _context.ExtraTypes.AnyAsync(et => et.Id == extraTypeId);
        }

        public async Task<decimal> GetExtraTypePrice(int extraTypeId)
        {
            var extraType = await _context.ExtraTypes.FirstOrDefaultAsync(et => et.Id == extraTypeId);
            return extraType?.Price ?? 0;
        }

        public async Task<decimal> GetOrderExtrasTotal(int orderId)
        {
            var extrasTotal = await _context.OrderItemExtras
                .Include(oie => oie.OrderItem)
                .Where(oie => oie.OrderItem.OrderId == orderId)
                .SumAsync(oie => oie.Price * oie.Quantity);

            return extrasTotal;
        }
    }
}
