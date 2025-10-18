using FindMyCoffee.Domain.Enums;
using FindMyCoffee.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FindMyCoffee.Data
{
    public static class ModelBuildingExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var genderToString = new EnumToStringConverter<Gender>();
            var roleToString = new EnumToStringConverter<Role>();

            //No two CoffeeShops can have the same PlaceId
            modelBuilder.Entity<CoffeeShopEntity>()
                .HasIndex(x => x.PlaceId)//in databases (and therefore EF Core), "index"
                                         //is a special lookup table that the database creates behind the scenes to make searching faster
                .IsUnique();

            modelBuilder.Entity<UserEntity>()
                .Property(entity => entity.Gender)
                .HasConversion(genderToString);

            modelBuilder.Entity<UserEntity>()
                .Property(entity => entity.Role)
                .HasConversion(roleToString);

        }
    }
}
