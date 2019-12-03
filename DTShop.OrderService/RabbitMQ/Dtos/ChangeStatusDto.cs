namespace DTShop.OrderService.RabbitMQ.Dtos
{
    public class ChangeStatusDto
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
    }
}
