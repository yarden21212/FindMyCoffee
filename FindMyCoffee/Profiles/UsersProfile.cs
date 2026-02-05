using FindMyCoffee.Models;
using AutoMapper;
using FindMyCoffee.Dtos;
using FindMyCoffee.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FindMyCoffee.ViewModels;

namespace FindMyCoffee.Profiles
{

    /*
     * Defines AutoMapper mappings for User models.
     * Helps protect internal database models by exposing only DTOs/ViewModels to the API/UI,
     * while ensuring all model conversions are defined in one central place.
     */
    public class UsersProfile : Profile
    {
        public UsersProfile()
        {
            CreateMap<UserEntity, UserReadDto>();
            CreateMap<RegisterViewModel, UserRequestLogic>();
            CreateMap<UserRequestLogic, UserRegistrationDto>();
            CreateMap<UserRegistrationDto, UserEntity>();



        }
    }
}
