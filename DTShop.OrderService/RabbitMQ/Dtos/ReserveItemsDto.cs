namespace DTShop.OrderService.RabbitMQ.Dtos
{
    public class ReserveItemsDto
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }
    }
}
