using FindMyCoffee.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Reflection;

namespace FindMyCoffee.Data
{
    //public class FindMyCoffeeContext(DbContextOptions<FindMyCoffeeContext> opt) : base(opt)
    public class FindMyCoffeeContext : DbContext
    {
        public FindMyCoffeeContext(DbContextOptions<FindMyCoffeeContext> options)
            : base(options) {}

        //This DBset tells EF Core "make me a CoffeeShop table".
        public DbSet<CoffeeShopEntity> CoffeeShops { get; set; }

        public DbSet<UserEntity> Users { get; set; }

        /*
         * Automatically called by EF Core when building the model (blueprint) of your database.
         * “Here’s exactly how I want my database to look and behave. -> I don't have to use it, but it adds constraints and so on...
         * By default, EF looks at your classes (CoffeeShop, etc.) and creates tables from them.
         * But sometimes you need more control → that’s when you use OnModelCreating.
         * 
         */
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Seed(); //A side method to add the constraints

            //Creation of many-to-many relationship:
            modelBuilder.Entity<UserEntity>()       // We tell the modelBuilder that the "UserEntity" entity
                .HasMany(x => x.CoffeeShops)  // has many LinkedCoffeeShops
                .WithMany(x => x.Users)
                .UsingEntity(j => j.ToTable("UserCoffeeShops")); //CoffeeShopsLinked should be the "join table" or in other words: the table that connects the users and coffeeshops.;
        }

        internal object GetShopById(int id)
        {
            throw new NotImplementedException();
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            applyTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        public override int SaveChanges()
        {
            applyTimestamps();
            return base.SaveChanges();
        }

        private void applyTimestamps() 
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries()
                         .Where(e => e.State == EntityState.Added))
            {
                if (entry.Properties.Any(p => p.Metadata.Name == nameof(UserEntity.CreatedAtUtc)))
                    entry.Property(nameof(UserEntity.CreatedAtUtc)).CurrentValue = now;

                if (entry.Properties.Any(p => p.Metadata.Name == nameof(UserEntity.ModifiedAtUtc)))
                    entry.Property(nameof(UserEntity.ModifiedAtUtc)).CurrentValue = now;
            }

            foreach (var entry in ChangeTracker.Entries()
                         .Where(e => e.State == EntityState.Modified))
            {
                if (entry.Properties.Any(p => p.Metadata.Name == nameof(UserEntity.ModifiedAtUtc)))
                    entry.Property(nameof(UserEntity.ModifiedAtUtc)).CurrentValue = now;
            }


        }
    }
}