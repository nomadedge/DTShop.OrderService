using DTShop.OrderService.Core.Enums;
using DTShop.OrderService.Data.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DTShop.OrderService.Data.Repositories
{
    public interface IOrderRepository
    {
        Task<Order> AddItemToOrder(int orderId, int warehouseItemId, int amount, string userName);
        IEnumerable<Order> GetAllOrders();
        Order GetOrderById(int orderId);
        Task<bool> SaveChangesAsync();
        Order SetOrderStatus(int orderId, OrderStatus newOrderStatus);
    }
}