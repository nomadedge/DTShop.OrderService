using System.ComponentModel.DataAnnotations;

namespace DTShop.OrderService.Data.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        [Required]
        public Item Item { get; set; }
        public int Amount { get; set; }
    }
}
