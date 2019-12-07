using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;

namespace DTShop.OrderService.RabbitMQ.Dtos
{
    public class ReserveItemsDto
    {
        public int OrderId { get; set; }
        public int ItemId { get; set; }
        public int Amount { get; set; }

        public ReserveItemsDto() { }
        public ReserveItemsDto(Order order, AddItemModel addItemModel)
        {
            OrderId = order.OrderId;
            ItemId = addItemModel.ItemId;
            Amount = addItemModel.Amount;
        }
    }
}
