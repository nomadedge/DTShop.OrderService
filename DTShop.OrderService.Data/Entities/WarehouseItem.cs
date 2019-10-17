using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTShop.OrderService.Data.Entities
{
    public class WarehouseItem
    {
        [Key]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }
        public int Amount { get; set; }
    }
}
