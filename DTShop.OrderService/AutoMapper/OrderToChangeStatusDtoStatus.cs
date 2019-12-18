using AutoMapper;
using DTShop.OrderService.Data.Entities;
using DTShop.OrderService.RabbitMQ.Dtos;

namespace DTShop.OrderService.AutoMapper
{
    public class OrderToChangeStatusDtoStatus : IValueResolver<Order, ChangeStatusDto, string>
    {
        public string Resolve(Order source, ChangeStatusDto destination, string destMember, ResolutionContext context)
        {
            return source.StatusId.ToString();
        }
    }
}
