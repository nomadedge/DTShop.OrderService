using DTShop.OrderService.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTShop.OrderService.Data.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        [Required]
        public string Username { get; set; }
        public long? PaymentId { get; set; }
        public OrderStatus StatusId { get; set; }
        public Status Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
