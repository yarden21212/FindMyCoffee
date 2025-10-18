using AutoMapper;
using FindMyCoffee.Dots;
using FindMyCoffee.Dtos;
using FindMyCoffee.Models;

namespace FindMyCoffee.Profiles
{
    public class CoffeeShopsProfile : Profile
    {
        public CoffeeShopsProfile()
        {
            CreateMap<CoffeeShopEntity, CoffeeShopReadDto>();
            CreateMap<CoffeeShopCreateDto, CoffeeShopEntity>();
            CreateMap<CoffeeShopUpdateDto, CoffeeShopEntity>();
            CreateMap<CoffeeShopEntity, CoffeeShopUpdateDto>();
        }
    }
}
