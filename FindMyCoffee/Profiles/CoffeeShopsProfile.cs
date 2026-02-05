using AutoMapper;
using FindMyCoffee.Dots;
using FindMyCoffee.Dtos;
using FindMyCoffee.Models;
using FindMyCoffee.ViewModels;

namespace FindMyCoffee.Profiles
{
    /*
     * Defines AutoMapper mappings for CoffeeShop models.
     * Helps protect internal database models by exposing only DTOs/ViewModels to the API/UI,
     * while ensuring all model conversions are defined in one central place.
     */

    public class CoffeeShopsProfile : Profile
    {
        public CoffeeShopsProfile()
        {
            CreateMap<CoffeeShopEntity, CoffeeShopReadDto>();
            CreateMap<CoffeeShopCreateDto, CoffeeShopEntity>();
            CreateMap<CoffeeShopUpdateDto, CoffeeShopEntity>();
            CreateMap<CoffeeShopEntity, CoffeeShopUpdateDto>();
            CreateMap<CoffeeShopEntity, BusinessViewModel>();
            CreateMap<BusinessViewModel, CoffeeShopEntity>();
            CreateMap<CoffeeShopViewModel, CoffeeShopEntity>();
        }
    }
}
