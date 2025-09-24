using AutoMapper;
using RestaurantApp.Models.DTOs;
using RestaurantApp.Models.Entities;

namespace RestaurantApp.Utils.Mapping;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserDto>();
        CreateMap<MenuItem, MenuItemDto>();
        CreateMap<Restaurant, RestaurantShortDto>();
        CreateMap<Restaurant, RestaurantDetailsDto>()
            .ForMember(dest => dest.Menu, opt => opt.MapFrom(src => src.MenuItems));
        CreateMap<OrderItem, OrderItemDetailsDto>()
            .ForMember(dest => dest.MenuItemName, opt => opt.MapFrom(src => src.MenuItem.Name))
            .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.MenuItem.Price));
            
        CreateMap<Order, OrderDetailsDto>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
            .ForMember(dest => dest.RestaurantName, opt => opt.MapFrom(src => src.Restaurant.Name))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderItems));
    }
}