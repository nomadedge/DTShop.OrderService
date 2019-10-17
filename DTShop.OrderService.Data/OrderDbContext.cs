using DTShop.OrderService.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace DTShop.OrderService.Data
{
    public class OrderDbContext : DbContext
    {
        public OrderDbContext(DbContextOptions<OrderDbContext> options)
            : base(options) { }

        public DbSet<Item> Items { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<WarehouseItem> WarehouseItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var BI = new Item
            {
                ItemId = 1,
                Name = "Bioshock Infinite",
                Price = 10m
            };

            var SW = new Item
            {
                ItemId = 2,
                Name = "Shadow Warrior",
                Price = 3m
            };

            var ME = new Item
            {
                ItemId = 3,
                Name = "Mass Effect",
                Price = 6m
            };

            var WRoEF = new Item
            {
                ItemId = 4,
                Name = "What Remains of Edith Finch",
                Price = 5m
            };

            modelBuilder.Entity<Item>().HasData(BI, SW, ME, WRoEF);

            modelBuilder.Entity<WarehouseItem>().HasData(
                new
                {
                    ItemId = 1,
                    Amount = 10
                },
                new
                {
                    ItemId = 2,
                    Amount = 10
                },
                new
                {
                    ItemId = 3,
                    Amount = 10
                },
                new
                {
                    ItemId = 4,
                    Amount = 10
                });
        }
    }
}