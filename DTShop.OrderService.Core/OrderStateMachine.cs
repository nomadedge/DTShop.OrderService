using DTShop.OrderService.Core.Enums;
using System;
using System.Collections.Generic;

namespace DTShop.OrderService.Core
{
    public static class OrderStateMachine
    {
        private static List<Tuple<OrderStatus, OrderStatus>> _transitions =
            new List<Tuple<OrderStatus, OrderStatus>>()
            {
                new Tuple<OrderStatus, OrderStatus>(OrderStatus.Collecting, OrderStatus.Paid),
                new Tuple<OrderStatus, OrderStatus>(OrderStatus.Collecting, OrderStatus.Failed),
                new Tuple<OrderStatus, OrderStatus>(OrderStatus.Paid, OrderStatus.Shipping),
                new Tuple<OrderStatus, OrderStatus>(OrderStatus.Paid, OrderStatus.Cancelled),
                new Tuple<OrderStatus, OrderStatus>(OrderStatus.Shipping, OrderStatus.Complete)
            };

        public static bool IsTransitionAllowed(OrderStatus statusFrom, OrderStatus statusTo)
        {
            return _transitions.Contains(new Tuple<OrderStatus, OrderStatus>(statusFrom, statusTo));
        }
    }
}
