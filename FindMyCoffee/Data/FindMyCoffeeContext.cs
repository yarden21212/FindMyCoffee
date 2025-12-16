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

        public DbSet<UserCoffeeShopsEntity> UserCoffeeShops { get; set; }

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

            ////Creation of many-to-many relationship:
            //modelBuilder.Entity<UserEntity>()   
            //    .HasMany(x => x.CoffeeShops)    //  Specifies that a User can have many Coffee shops.
            //    .WithMany(x => x.Users)         //  Specifies that a Coffee shop can have many Users.       
            //    .UsingEntity(j => j.ToTable("UserCoffeeShops")); //  Explicitly names the join table. (If I don’t specify this, EF Core will)

            modelBuilder.Entity<UserCoffeeShopsEntity>(entity =>
            {
                // Composite primary key (UserId + CoffeeShopId)
                entity.HasKey(ucs => new { ucs.UserId, ucs.CoffeeShopId });

                // One User has many UserCoffeeShops
                entity.HasOne(ucs => ucs.User)
                      .WithMany(u => u.UserCoffeeShops)
                      .HasForeignKey(ucs => ucs.UserId)
                      .OnDelete(DeleteBehavior.Cascade);

                // One CoffeeShop has many UserCoffeeShops
                entity.HasOne(ucs => ucs.CoffeeShop)
                      .WithMany(cs => cs.UserCoffeeShops)
                      .HasForeignKey(ucs => ucs.CoffeeShopId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }

        /*
         * This is the async version of saving to the database, overrides EF's default SaveChangesAsync.
         * The CancellationToken is just an optional stop button, most of the time, it's not in use,
         * but ASP.NET can use it automatically to stop the save if the request is no longer needed (for example when a user closes the browser in the middle of the processes).
         */
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            applyTimestamps();
            return base.SaveChangesAsync(cancellationToken);
        }

        /*
         * This overrides EF's default SaveChanges.
         */
        public override int SaveChanges()
        {
            applyTimestamps();
            return base.SaveChanges();
        }

        /* 
         * A helper function that: 
         * Checks EF Core’s ChangeTracker (what entities are new, updated, deleted)
         * If entity is Added -> set CreatedAt and ModifiedAt
         * If entity is Modified -> set ModifiedAt
         */
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