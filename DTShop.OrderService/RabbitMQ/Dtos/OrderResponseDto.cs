using DTShop.OrderService.Core.Models;

namespace DTShop.OrderService.RabbitMQ.Dtos
{
    public class OrderResponseDto
    {
        public OrderModel Order { get; set; }
        public string CardAuthorizationInfo { get; set; }
    }
}
