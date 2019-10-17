﻿// <auto-generated />
using System;
using DTShop.OrderService.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DTShop.OrderService.Data.Migrations
{
    [DbContext(typeof(OrderDbContext))]
    [Migration("20191014090342_InitialMigration")]
    partial class InitialMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DTShop.OrderService.Data.Entities.Item", b =>
                {
                    b.Property<int>("ItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(18,2)");

                    b.HasKey("ItemId");

                    b.ToTable("Items");

                    b.HasData(
                        new
                        {
                            ItemId = 1,
                            Name = "Bioshock Infinite",
                            Price = 10m
                        },
                        new
                        {
                            ItemId = 2,
                            Name = "Shadow Warrior",
                            Price = 3m
                        },
                        new
                        {
                            ItemId = 3,
                            Name = "Mass Effect",
                            Price = 6m
                        },
                        new
                        {
                            ItemId = 4,
                            Name = "What Remains of Edith Finch",
                            Price = 5m
                        });
                });

            modelBuilder.Entity("DTShop.OrderService.Data.Entities.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int?>("PaymentId")
                        .HasColumnType("int");

                    b.Property<int>("Status")
                        .HasColumnType("int");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("OrderId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("DTShop.OrderService.Data.Entities.OrderItem", b =>
                {
                    b.Property<int>("OrderItemId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.Property<int?>("ItemId")
                        .HasColumnType("int");

                    b.Property<int?>("OrderId")
                        .HasColumnType("int");

                    b.HasKey("OrderItemId");

                    b.HasIndex("ItemId");

                    b.HasIndex("OrderId");

                    b.ToTable("OrderItems");
                });

            modelBuilder.Entity("DTShop.OrderService.Data.Entities.WarehouseItem", b =>
                {
                    b.Property<int>("ItemId")
                        .HasColumnType("int");

                    b.Property<int>("Amount")
                        .HasColumnType("int");

                    b.HasKey("ItemId");

                    b.ToTable("WarehouseItems");

                    b.HasData(
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
                });

            modelBuilder.Entity("DTShop.OrderService.Data.Entities.OrderItem", b =>
                {
                    b.HasOne("DTShop.OrderService.Data.Entities.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId");

                    b.HasOne("DTShop.OrderService.Data.Entities.Order", null)
                        .WithMany("OrderItems")
                        .HasForeignKey("OrderId");
                });

            modelBuilder.Entity("DTShop.OrderService.Data.Entities.WarehouseItem", b =>
                {
                    b.HasOne("DTShop.OrderService.Data.Entities.Item", "Item")
                        .WithMany()
                        .HasForeignKey("ItemId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
