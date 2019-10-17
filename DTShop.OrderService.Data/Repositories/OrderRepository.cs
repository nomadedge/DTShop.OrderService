using DTShop.OrderService.Core;
using DTShop.OrderService.Core.Enums;
using DTShop.OrderService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DTShop.OrderService.Data.Repositories
{
    public class SqlOrderRepository : IOrderRepository
    {
        private readonly OrderDbContext _orderDbContext;

        public SqlOrderRepository(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public IEnumerable<Order> GetAllOrders()
        {
            var orders = _orderDbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item);
            if (orders.Count() == 0)
            {
                throw new InvalidOperationException("No orders in database.");
            }
            return orders;
        }

        public Order GetOrderById(int orderId)
        {
            var order = _orderDbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new ArgumentException("No order with such id.");
            }
            return order;
        }

        public Order SetOrderStatus(int orderId, OrderStatus newOrderStatus)
        {
            var order = GetOrderById(orderId);

            if (!OrderStateMachine.IsTransitionAllowed(order.Status, newOrderStatus))
            {
                throw new InvalidOperationException("Transition is not allowed by state machine.");
            }

            order.Status = newOrderStatus;
            if (newOrderStatus == OrderStatus.Failed || newOrderStatus == OrderStatus.Cancelled)
            {
                foreach (var orderItem in order.OrderItems)
                {
                    var warehouseItem = _orderDbContext.WarehouseItems
                        .FirstOrDefault(wi => wi.Item == orderItem.Item);
                    warehouseItem.Amount += orderItem.Amount;
                }
            }

            return order;
        }

        public async Task<Order> AddItemToOrder(int orderId, int itemId, int amount, string userName)
        {
            if (amount < 1 || amount > 99)
            {
                throw new ArgumentOutOfRangeException("Amount should be from 1 to 99");
            }

            var warehouseItem = _orderDbContext.WarehouseItems
                .Include(wi => wi.Item)
                .FirstOrDefault(wi => wi.Item.ItemId == itemId);
            if (warehouseItem == null)
            {
                throw new ArgumentException("No warehouse item with such Id.");
            }
            if (warehouseItem.Amount < amount)
            {
                throw new ArgumentException("Not enough items in warehouse");
            }

            if (orderId == 0)
            {
                var newOrder = new Order
                {
                    UserName = userName,
                    Status = OrderStatus.Collecting,
                    OrderItems = new List<OrderItem> { new OrderItem
                    {
                        Item = warehouseItem.Item,
                        Amount = amount
                    } }
                };
                _orderDbContext.Add(newOrder);
                warehouseItem.Amount -= amount;

                if (!await SaveChangesAsync())
                {
                    throw new DbUpdateException("Database failure");
                }

                return newOrder;
            }
            else
            {
                var order = GetOrderById(orderId);

                if (order.UserName != userName)
                {
                    throw new ArgumentException("UserName for existing order should be the same.");
                }

                var existingOrderItem = order.OrderItems
                    .FirstOrDefault(oi => oi.Item.ItemId == itemId);

                if (existingOrderItem == null)
                {
                    order.OrderItems.Add(new OrderItem
                    {
                        Item = warehouseItem.Item,
                        Amount = amount
                    });
                }
                else
                {
                    existingOrderItem.Amount += amount;
                }

                warehouseItem.Amount -= amount;

                if (!await SaveChangesAsync())
                {
                    throw new DbUpdateException("Database failure");
                }

                return order;
            }
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _orderDbContext.SaveChangesAsync()) > 0;
        }
    }
}
