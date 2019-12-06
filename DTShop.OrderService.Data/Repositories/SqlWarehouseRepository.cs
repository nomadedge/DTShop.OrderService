using DTShop.OrderService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace DTShop.OrderService.Data.Repositories
{
    public class SqlWarehouseRepository : IWarehouseRepository
    {
        private readonly OrderDbContext _orderDbContext;

        public SqlWarehouseRepository(OrderDbContext orderDbContext)
        {
            _orderDbContext = orderDbContext;
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _orderDbContext.SaveChangesAsync()) > 0;
        }

        public async Task SupplyItemsAsync(int itemId, int amount, string name, decimal price)
        {
            using (var transaction = _orderDbContext.Database.BeginTransaction())
            {
                try
                {
                    var warehouseItem = await _orderDbContext.WarehouseItems
                        .FirstOrDefaultAsync(wi => wi.ItemId == itemId);
                    if (warehouseItem == null)
                    {
                        if (string.IsNullOrEmpty(name) || price == 0m)
                        {
                            throw new ArgumentException("Provide name and price for new item.");
                        }
                        _orderDbContext.Add(new Item
                        {
                            ItemId = itemId,
                            Name = name,
                            Price = price
                        });
                        _orderDbContext.Add(new WarehouseItem
                        {
                            ItemId = itemId,
                            Amount = amount
                        });
                    }
                    else
                    {
                        if (amount > warehouseItem.Amount)
                        {
                            warehouseItem.Amount = amount;
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
        }
    }
}
