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

        private Status EnumToStatus(OrderStatus orderStatusEnum)
        {
            //return new Status { StatusId = orderStatusEnum, Name = orderStatusEnum.ToString() };
            return _orderDbContext.Statuses.FirstOrDefault(os => os.StatusId == orderStatusEnum);
        }

        public IEnumerable<Order> GetAllOrders()
        {
            var orders = _orderDbContext.Orders
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item);
            return orders;
        }

        public async Task<Order> GetOrderByIdAsync(int orderId)
        {
            var order = await _orderDbContext.Orders
                .Include(o => o.Status)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Item)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new ArgumentException("No order with such id.");
            }
            return order;
        }

        public async Task<Order> SetOrderStatusAsync(int orderId, OrderStatus newOrderStatus)
        {
            var order = new Order();

            using (var transaction = _orderDbContext.Database.BeginTransaction())
            {
                try
                {
                    order = await GetOrderByIdAsync(orderId);

                    if (!OrderStateMachine.IsTransitionAllowed(order.Status.StatusId, newOrderStatus))
                    {
                        throw new InvalidOperationException("Transition is not allowed by state machine.");
                    }

                    order.Status = EnumToStatus(newOrderStatus);
                    if (newOrderStatus == OrderStatus.Failed || newOrderStatus == OrderStatus.Cancelled)
                    {
                        foreach (var orderItem in order.OrderItems)
                        {
                            var warehouseItem = await _orderDbContext.WarehouseItems
                                .FirstOrDefaultAsync(wi => wi.Item == orderItem.Item);
                            warehouseItem.Amount += orderItem.Amount;
                        }
                    }

                    if (!await SaveChangesAsync())
                    {
                        throw new DbUpdateException("Database failure");
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return order;
        }

        public async Task<Order> AddItemToOrderAsync(int orderId, int itemId, int amount, string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException("Username cannot be null.");
            }

            var order = new Order();

            using (var transaction = _orderDbContext.Database.BeginTransaction())
            {
                try
                {
                    if (amount < 1 || amount > 99)
                    {
                        throw new ArgumentOutOfRangeException("Amount should be from 1 to 99");
                    }

                    var warehouseItem = await _orderDbContext.WarehouseItems
                        .Include(wi => wi.Item)
                        .FirstOrDefaultAsync(wi => wi.Item.ItemId == itemId);
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
                        order = new Order
                        {
                            Username = username,
                            Status = EnumToStatus(OrderStatus.Collecting),
                            OrderItems = new List<OrderItem> { new OrderItem
                    {
                        Item = warehouseItem.Item,
                        Amount = amount
                    } }
                        };
                        await _orderDbContext.AddAsync(order);
                        warehouseItem.Amount -= amount;
                    }
                    else
                    {
                        order = await GetOrderByIdAsync(orderId);

                        if (order.Status.StatusId != OrderStatus.Collecting)
                        {
                            throw new ArgumentException("Order status should be \"Collecting\".");
                        }

                        if (order.Username != username)
                        {
                            throw new ArgumentException("Username for existing order should be the same.");
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
                    }

                    if (!await SaveChangesAsync())
                    {
                        throw new DbUpdateException("Database failure");
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return order;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _orderDbContext.SaveChangesAsync()) > 0;
        }

        public async Task<Order> PayForOrderAsync(int orderId, long paymentId, string status)
        {
            var order = new Order();
            using (var transaction = _orderDbContext.Database.BeginTransaction())
            {
                try
                {
                    order = await GetOrderByIdAsync(orderId);

                    if (order.Status.StatusId != OrderStatus.Collecting)
                    {
                        throw new ArgumentException("Order status should be \"Collecting\"");
                    }

                    order.PaymentId = paymentId;
                    OrderStatus orderStatus;
                    switch (status)
                    {
                        case "Paid":
                            orderStatus = OrderStatus.Paid;
                            break;
                        case "Failed":
                            orderStatus = OrderStatus.Failed;
                            break;
                        default:
                            throw new ArgumentException("Status is not valid.");
                    }
                    order.Status = EnumToStatus(orderStatus);

                    if (orderStatus == OrderStatus.Failed)
                    {
                        foreach (var orderItem in order.OrderItems)
                        {
                            var warehouseItem = await _orderDbContext.WarehouseItems
                                .FirstOrDefaultAsync(wi => wi.Item == orderItem.Item);
                            warehouseItem.Amount += orderItem.Amount;
                        }
                    }

                    if (!await SaveChangesAsync())
                    {
                        throw new DbUpdateException("Database failure");
                    }

                    transaction.Commit();
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    throw e;
                }
            }

            return order;
        }
    }
}