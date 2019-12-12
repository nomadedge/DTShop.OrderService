using AutoMapper;
using DTShop.OrderService.Core.Enums;
using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;
using System;

namespace DTShop.OrderService.AutoMapper
{
    public class OrderModelToOrderStatusId : IValueResolver<OrderModel, Order, OrderStatus>
    {
        public OrderStatus Resolve(OrderModel source, Order destination, OrderStatus destMember, ResolutionContext context)
        {
            Enum.TryParse(source.Status, out OrderStatus orderStatusEnum);
            return orderStatusEnum;
        }
    }
}
