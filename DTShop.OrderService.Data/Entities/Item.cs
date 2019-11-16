using System.ComponentModel.DataAnnotations.Schema;

namespace DTShop.OrderService.Data.Entities
{
    public class Item
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
