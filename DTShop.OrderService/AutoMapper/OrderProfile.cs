using AutoMapper;
using DTShop.OrderService.Core.Models;
using DTShop.OrderService.Data.Entities;

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
                .ForMember(om => om.Status, opt => opt.MapFrom<StatusToStringResolver>())
                .ReverseMap()
                .ForMember(om => om.Status, opt => opt.MapFrom<StringToStatusResolver>());

            CreateMap<OrderItem, OrderItemModel>()
                .ReverseMap()
                .ForMember(oi => oi.OrderItemId, opt => opt.Ignore());
        }
    }
}
