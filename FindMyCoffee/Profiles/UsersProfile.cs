using FindMyCoffee.Models;
using AutoMapper;
using FindMyCoffee.Dtos;
using FindMyCoffee.Domain;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using FindMyCoffee.ViewModels;

namespace FindMyCoffee.Profiles
{
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
