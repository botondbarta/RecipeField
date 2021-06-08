using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.ApiAuthorization.IdentityServer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Options;
using RecipeField.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.DAL
{
    public class RecipeFieldDbContext : ApiAuthorizationDbContext<User>
    {
        public RecipeFieldDbContext(DbContextOptions options, IOptions<OperationalStoreOptions> operationalStoreOptions)
                : base(options, operationalStoreOptions)
        {
        }

        public DbSet<Comment> Comments { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Recipeingredient> Recipeingredients { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Text).IsRequired().HasMaxLength(300);
            });

            modelBuilder.Entity<Ingredient>(entity => {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });
            modelBuilder.Entity<Recipeingredient>(entity =>
            {
                entity.HasOne(e => e.Recipe)
                    .WithMany(r => r.Recipeingredients)
                    .HasForeignKey(ri => ri.RecipeId)
                    .OnDelete(DeleteBehavior.NoAction);

                entity.HasOne(e => e.Ingredient)
                    .WithMany(i => i.Recipeingredients)
                    .HasForeignKey(ri => ri.IngredientId)
                    .OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.Property(e => e.Description).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasOne(e => e.User).WithMany().OnDelete(DeleteBehavior.NoAction);
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasMany(e => e.Recipes).WithOne().OnDelete(DeleteBehavior.NoAction);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            });

            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Pecsenye" },
                new Category { Id = 2, Name = "Rák" });

            modelBuilder.Entity<Category>().OwnsOne(c => c.Invention).HasData(
                new
                {
                    Id = 1,
                    CategoryID = 1,
                    InventedIn = InventionRegion.Hungary,
                    InventionDate = 1400
                },
                new
                {
                    Id = 2,
                    CategoryID = 2,
                    InventedIn = InventionRegion.China,
                    InventionDate = 1600
                }
            );

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole<string>()
                {
                    Name = "Admin",
                    NormalizedName = "ADMIN",
                    Id = "1",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                },
                new IdentityRole<string>()
                {
                    Name = "Guest",
                    NormalizedName = "GUEST",
                    Id = "2",
                    ConcurrencyStamp = Guid.NewGuid().ToString()
                }
                );

            PasswordHasher<User> ph = new PasswordHasher<User>();
            var user1 = new User()
            {
                Id = "1",
                LastName = "Barta",
                FirstName = "Botond",
                Email = "b.botond25@gmail.com",
                NormalizedEmail = "b.botond25@gmail.com".ToUpper(),
                EmailConfirmed = true,
                UserName = "b.botond25@gmail.com",
                NormalizedUserName = "b.botond25@gmail.com".ToUpper(),
            };
            var user2 = new User()
            {
                Id = "2",
                LastName = "Gipsz",
                FirstName = "Jakab",
                Email = "gipszjakab42@gmail.com",
                NormalizedEmail = "gipszjakab42@gmail.com".ToUpper(),
                EmailConfirmed = true,
                UserName = "gipszjakab42@gmail.com",
                NormalizedUserName = "gipszjakab42@gmail.com".ToUpper(),
            };
            var user3 = new User()
            {
                Id = "3",
                LastName = "Adat",
                FirstName = "Béla",
                Email = "adatbnoises66@gmail.com",
                NormalizedEmail = "adatbnoises66@gmail.com".ToUpper(),
                EmailConfirmed = true,
                UserName = "adatbnoises66@gmail.com",
                NormalizedUserName = "adatbnoises66@gmail.com".ToUpper(),
            };

            user1.PasswordHash = ph.HashPassword(user1, "Test.123");
            user2.PasswordHash = ph.HashPassword(user2, "Test.123");
            user3.PasswordHash = ph.HashPassword(user3, "Test.123");

            modelBuilder.Entity<User>().HasData(
                user1, user2, user3
            );

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                 new IdentityUserRole<string>() { UserId = "1", RoleId = "1" },
                 new IdentityUserRole<string>() { UserId = "2", RoleId = "2" },
                 new IdentityUserRole<string>() { UserId = "3", RoleId = "2" }
            );

            modelBuilder.Entity<Ingredient>().HasData(
                new Ingredient { Id = 1, Name = "Só" },
                new Ingredient { Id = 2, Name = "Bors" },
                new Ingredient { Id = 3, Name = "Káposzta" },
                new Ingredient { Id = 4, Name = "Csirkeszárny" },
                new Ingredient { Id = 5, Name = "Csirkecomb" }
                );
            modelBuilder.Entity<Recipe>().HasData(
                new Recipe { Id = 1, CategoryID = 1, Name = "Nyárson csirkecomb", Description = "Süsd meg", UserId = "1" },
                new Recipe { Id = 2, CategoryID = 1, Name = "Nyárson csirkeszárny", Description = "20perc alatt kész", UserId = "1" },
                new Recipe { Id = 3, CategoryID = 1, Name = "Nyárson rák", Description = "Rák nyamm", UserId = "1" }
                );
            modelBuilder.Entity<Comment>().HasData(
                new Comment { Id = 1, RecipeId = 1, UserId = "1", Text = "Nagyon finom" },
                new Comment { Id = 2, RecipeId = 1, UserId = "2", Text = "Olyan mint amit a nagyim csinál" },
                new Comment { Id = 3, RecipeId = 3, UserId = "3", Text = "Borzalmas, soha többet, a kutyám is kihányta" }
                );
            modelBuilder.Entity<Recipeingredient>().HasData(
                new Recipeingredient { Id = 1, RecipeId = 1, IngredientId = 1, Quantity = "2 csipet" },
                new Recipeingredient { Id = 2, RecipeId = 1, IngredientId = 5, Quantity = "5 dkg" },
                new Recipeingredient { Id = 3, RecipeId = 2, IngredientId = 4, Quantity = "2 kg" },
                new Recipeingredient { Id = 4, RecipeId = 3, IngredientId = 2, Quantity = "2 kanál" }
                );

        }

    }
}
