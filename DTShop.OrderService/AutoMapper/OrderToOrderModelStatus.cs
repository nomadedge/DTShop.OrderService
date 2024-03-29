﻿using AutoMapper;
using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;

namespace DTShop.OrderService.AutoMapper
{
    public class OrderToOrderModelStatus : IValueResolver<Order, OrderModel, string>
    {
        public string Resolve(Order source, OrderModel destination, string destMember, ResolutionContext context)
        {
            if (source.Status != null)
            {
                return source.Status.Name;
            }
            return source.StatusId.ToString();
        }
    }
}
