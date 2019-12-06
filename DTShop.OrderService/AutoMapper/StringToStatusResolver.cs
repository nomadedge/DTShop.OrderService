using AutoMapper;
using DTShop.OrderService.Core.Enums;
using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;
using System;

namespace DTShop.OrderService.AutoMapper
{
    public class StringToStatusResolver : IValueResolver<OrderModel, Order, Status>
    {
        public Status Resolve(OrderModel source, Order destination, Status destMember, ResolutionContext context)
        {
            Enum.TryParse(source.Status, out OrderStatus orderStatusEnum);
            return new Status { StatusId = orderStatusEnum, Name = orderStatusEnum.ToString() };
        }
    }
}
