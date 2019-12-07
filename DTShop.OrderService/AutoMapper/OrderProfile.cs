using AutoMapper;
using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;
using DTShop.OrderService.RabbitMQ.Dtos;

namespace DTShop.OrderService.AutoMapper
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Item, ItemModel>()
                .ReverseMap()
                .ForMember(i => i.ItemId, opt => opt.Ignore());

            CreateMap<Order, OrderModel>()
                .ForMember(om => om.TotalCost, opt => opt.Ignore())
                .ForMember(om => om.TotalAmount, opt => opt.Ignore())
                .ForMember(om => om.Status, opt => opt.MapFrom<OrderToOrderModelStatus>())
                .ReverseMap()
                .ForMember(o => o.Status, opt => opt.MapFrom<OrderModelToOrderStatus>());

            CreateMap<OrderItem, OrderItemModel>()
                .ReverseMap()
                .ForMember(oi => oi.OrderItemId, opt => opt.Ignore());

            CreateMap<Order, ChangeStatusDto>()
                .ForMember(csd => csd.Status, opt => opt.MapFrom<OrderToChangeStatusDtoStatus>());
        }
    }
}
