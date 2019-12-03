namespace DTShop.OrderService.RabbitMQ.Dtos
{
    public class SupplyItemsDto
    {
        public int ItemId { get; set; }
        public int Amount { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
