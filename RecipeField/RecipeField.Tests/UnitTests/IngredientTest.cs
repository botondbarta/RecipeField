using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RecipeField.BLL.Config;
using RecipeField.BLL.Dto;
using RecipeField.BLL.Exceptions;
using RecipeField.BLL.Services;
using RecipeField.DAL;
using RecipeField.DAL.Entities;
using Xunit;

namespace RecipeField.Tests.UnitTests
{
    public class IngredientTest : IDisposable
    {
        private RecipeFieldDbContext db;
        private IngredientService service;
        private IMapper mapper;

        public IngredientTest()
        {
            var mockMapper = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new MapProfile());
            });
            mapper = mockMapper.CreateMapper();

            var options = new DbContextOptionsBuilder<RecipeFieldDbContext>()
                .UseInMemoryDatabase("TestDatabaseComment")
                .Options;

            var someOptions = Options.Create(new OperationalStoreOptions());

            db = new RecipeFieldDbContext(options, someOptions);

            db.Ingredients.AddRange(SeedData());
            db.SaveChanges();

            service = new IngredientService(db, mapper);
        }
       
        
        [Fact]
        public async Task CreateIngredient()
        {
            IngredientDto newIngredientDto = new IngredientDto()
            {
                Name = "Saláta"
            };

            var result = await service.CreateAsync(newIngredientDto);

            Assert.NotNull(result);
            Assert.Equal("Saláta", result.Name);
            Assert.Equal(6, result.Id);
        }

        [Fact]
        public async Task CreateIngredientFails()
        {
            IngredientDto newIngredientDto = new IngredientDto()
            {
                Name = ""
            };

            await Assert.ThrowsAsync<NotValidParametersException>(() => service.CreateAsync(newIngredientDto));
        }

        [Fact]
        public async Task GetIngredientById()
        {
            var result = await service.GetByIdAsync(1);

            Assert.Equal(1, result.Id);
            Assert.Equal("Só", result.Name);
        }

        [Fact]
        public async Task GetIngredientByIdFails()
        {
            await Assert.ThrowsAsync<DbEntityNotFoundException>(() => service.GetByIdAsync(10));
        }

        [Fact]
        public async Task ModifyIngredientFailsNoEntity()
        {
            IngredientDto newIngredientDto = new IngredientDto()
            {
                Id = 10,
                Name = "Keszeg"
            };

            await Assert.ThrowsAsync<DbEntityNotFoundException>(() => service.ModifyAsync(newIngredientDto));
        }
        [Fact]
        public async Task ModifyIngredientFailsWrongParameters()
        {
            IngredientDto newIngredientDto = new IngredientDto()
            {
                Id = 1,
                Name = ""
            };

            await Assert.ThrowsAsync<NotValidParametersException>(() => service.ModifyAsync(newIngredientDto));
        }
        [Fact]
        public async Task ModifyIngredient()
        {
            IngredientDto newIngredientDto = new IngredientDto()
            {
                Id = 1,
                Name = "Keszeg"
            };

            var result = await service.ModifyAsync(newIngredientDto);

            Assert.Equal(1, result.Id);
            Assert.Equal("Keszeg", result.Name);
        }

        [Fact]
        public async Task DeleteIngredientFails()
        {
            int deleteId = 10;

            await Assert.ThrowsAsync<DbEntityNotFoundException>(() => service.DeleteAsync(deleteId));
        }
        [Fact]
        public async Task DeleteIngredient()
        {
            int deleteId = 1;
            var expected = await db.Ingredients.Where(e => e.Id != deleteId).ToListAsync();

            await service.DeleteAsync(deleteId);
            var result = await db.Ingredients.ToListAsync();

            Assert.Equal(expected.Count, result.Count);
        }
        [Fact]
        public async Task GetAllIngredient()
        {
            var expected = await db.Ingredients.ToListAsync();

            var result = await service.GetAll();

            Assert.Equal(expected.Count, result.Count);
        }

        private List<Ingredient> SeedData()
        {
            return new List<Ingredient>(){
                new Ingredient { Id = 1, Name = "Só" },
                new Ingredient { Id = 2, Name = "Bors" },
                new Ingredient { Id = 3, Name = "Káposzta" },
                new Ingredient { Id = 4, Name = "Csirkeszárny" },
                new Ingredient { Id = 5, Name = "Csirkecomb" }
            };
        }

        public void Dispose()
        {
            db.Database.EnsureDeleted();
            db.Dispose();
            service = null;
            mapper = null;
        }
    }
}
