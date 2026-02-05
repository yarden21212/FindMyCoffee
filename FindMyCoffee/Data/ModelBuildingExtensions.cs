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
            // Tells EF Core to store enum values as strings in the database
            var genderToString = new EnumToStringConverter<Gender>();
            var roleToString = new EnumToStringConverter<Role>();

            // Optional google api use: No two CoffeeShops can have the same PlaceId (as long they arrive from google api)
            modelBuilder.Entity<CoffeeShopEntity>()
            .HasIndex(e => e.PlaceId)
            .HasFilter("\"PlaceId\" IS NOT NULL");  // allows duplicates if null

            // Tells EF Core to store Gender as string in the database
            modelBuilder.Entity<UserEntity>()
                .Property(entity => entity.Gender)
                .HasConversion(genderToString);

            // Tells EF Core to store Role as string in the database
            modelBuilder.Entity<UserEntity>()
                .Property(entity => entity.Role)
                .HasConversion(roleToString);

        }
    }
}
