using DTShop.OrderService.Core.Enums;
using DTShop.OrderService.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

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
        public DbSet<Status> Statuses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var statuses = new List<Status>();
            foreach (OrderStatus orderStatus in Enum.GetValues(typeof(OrderStatus)))
            {
                statuses.Add(new Status
                {
                    StatusId = orderStatus,
                    Name = orderStatus.ToString()
                });
            }
            
            modelBuilder.Entity<Status>().HasData(statuses);
        }
    }
}