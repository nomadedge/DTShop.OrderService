namespace DTShop.OrderService.Data.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public Item Item { get; set; }
        public int Amount { get; set; }
    }
}
