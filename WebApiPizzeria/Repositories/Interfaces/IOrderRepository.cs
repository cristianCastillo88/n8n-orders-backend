using WebApiPizzeria.Models;

namespace WebApiPizzeria.Repositories.Interfaces;

public interface IOrderRepository
{
    Task<Order?> GetLastOrderByUserId(string userId);
    Task<Order> Save(string userId);
    Task UpdateTotal(int orderId, decimal total);
}

