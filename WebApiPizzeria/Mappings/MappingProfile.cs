using AutoMapper;
using WebApiPizzeria.DTOs;
using WebApiPizzeria.Models;

namespace WebApiPizzeria.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Mapeo de AddItemRequest a OrderItem
        // Solo mapea las propiedades coincidentes (OrderId, ProductId, Quantity)
        CreateMap<AddItemRequest, OrderItem>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.Order, opt => opt.Ignore())
            .ForMember(dest => dest.Product, opt => opt.Ignore());

        // Mapeo de Order a AddItemResponse
        CreateMap<Order, AddItemResponse>()
            .ForMember(dest => dest.Success, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Message, opt => opt.MapFrom(src => "Item agregado exitosamente"))
            .ForMember(dest => dest.NewTotal, opt => opt.MapFrom(src => src.Total))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => "USD"));
    }
}
