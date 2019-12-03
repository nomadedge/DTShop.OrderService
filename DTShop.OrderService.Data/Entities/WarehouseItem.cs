using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTShop.OrderService.Data.Entities
{
    public class WarehouseItem
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemId { get; set; }
        [ForeignKey("ItemId")]
        public Item Item { get; set; }
        public int Amount { get; set; }
    }
}
