using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DTShop.OrderService.Data.Entities
{
    public class Item
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ItemId { get; set; }
        [Required]
        public string Name { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
    }
}
