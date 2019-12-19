using DTShop.OrderService.Core.Enums;
using DTShop.OrderService.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DTShop.OrderService.Data.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddItemToOrderAsync(int orderId, int warehouseItemId, int amount, string username);
        IEnumerable<Order> GetAllOrders();
        Task<Order> GetOrderByIdAsync(int orderId);
        IEnumerable<Order> GetOrdersByUsername(string username);
        Task<Order> SetOrderStatusAsync(int orderId, OrderStatus newOrderStatus);
        Task<bool> SaveChangesAsync();
        Task<Order> PayForOrderAsync(int orderId, long paymentId, string status);
    }
}