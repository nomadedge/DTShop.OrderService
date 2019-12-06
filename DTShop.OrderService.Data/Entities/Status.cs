using DTShop.OrderService.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace DTShop.OrderService.Data.Entities
{
    public class Status
    {
        public OrderStatus StatusId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
