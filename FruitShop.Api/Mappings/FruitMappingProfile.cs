using AutoMapper;
using FruitShop.Api.Models;
using FruitShop.Api.DTOs;
using FruitShop.Api.Commands;

namespace FruitShop.Api.Mappings;

/// <summary>
/// AutoMapper profile for entity-to-DTO mapping.
/// </summary>
public class FruitMappingProfile : Profile
{
    public FruitMappingProfile()
    {
        CreateMap<Fruit, FruitDto>()
            .ForMember(dest => dest.Unit, opt => opt.MapFrom(src => 
                src.PricingStrategy != null ? src.PricingStrategy.GetUnit() : string.Empty));
        
        CreateMap<CreateFruitCommand, Fruit>();
        CreateMap<UpdateFruitCommand, Fruit>();
    }
}
