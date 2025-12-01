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

            //No two CoffeeShops can have the same PlaceId (as long they arrive from google api)
            modelBuilder.Entity<CoffeeShopEntity>()
            .HasIndex(e => e.PlaceId)
            .HasFilter("\"PlaceId\" IS NOT NULL");  // allows duplicates if null

            modelBuilder.Entity<UserEntity>()
                .Property(entity => entity.Gender)
                .HasConversion(genderToString);

            modelBuilder.Entity<UserEntity>()
                .Property(entity => entity.Role)
                .HasConversion(roleToString);

        }
    }
}
