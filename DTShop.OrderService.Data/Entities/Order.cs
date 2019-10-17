using DTShop.OrderService.Core.Enums;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DTShop.OrderService.Data.Entities
{
    public class Order
    {
        public int OrderId { get; set; }
        [Required]
        public string UserName { get; set; }
        public int? PaymentId { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
    }
}
