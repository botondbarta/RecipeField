using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    public class CategoryTest : IDisposable
    {
        private RecipeFieldDbContext db;
        private CategoryService service;
        private IMapper mapper;

        public CategoryTest()
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

            db.Categories.AddRange(SeedCategory());
            db.Recipes.Add(new Recipe
                {Id = 1, CategoryID = 1, Name = "Nyárson csirkecomb", Description = "Süsd meg"});
            db.SaveChanges();

            service = new CategoryService(db, mapper);
        }


        [Fact]
        public async Task DeleteCategoryFailsNoCategory()
        {
            int deleteId = 10;

            await Assert.ThrowsAsync<DbEntityNotFoundException>(() => service.DeleteAsync(deleteId));
        }
        [Fact]
        public async Task DeleteCategoryFailsNotEmptyCategory()
        {
            int deleteId = 1;

            await Assert.ThrowsAsync<NotEmptyCategoryException>(() => service.DeleteAsync(deleteId));
        }
        [Fact]
        public async Task DeleteCategory()
        {
            int deleteId = 2;
            var expected = await db.Categories.Where(e => e.Id != deleteId).ToListAsync();

            await service.DeleteAsync(deleteId);
            var result = await db.Categories.ToListAsync();

            Assert.Equal(expected.Count, result.Count);
        }
        [Fact]
        public async Task GetAllCategory()
        {
            var expected = await db.Categories.ToListAsync();

            var result = await service.GetAll();

            Assert.Equal(expected.Count, result.Count);
        }
        [Fact]
        public async Task GetCategoryById()
        {
            var result = await service.GetByIdAsync(1);

            Assert.Equal(1, result.Id);
            Assert.Equal("Hungary", result.InventionRegion);
            Assert.Equal(1400, result.InventionDate);
        }
        
        [Fact]
        public async Task CreateCategory()
        {
            CreateCategoryDto newCategoryDto = new CreateCategoryDto()
            {
                Name = "Bor alapú étel",
                InventionDate = 1300,
                InventionRegion = 32
            };

            var result = await service.CreateAsync(newCategoryDto);

            Assert.NotNull(result);
            Assert.Equal("Bor alapú étel", result.Name);
            Assert.Equal(3, result.Id);
            Assert.Equal("London", result.InventionRegion);
            Assert.Equal(1300, result.InventionDate);
        }
        [Fact]
        public async Task CreateCategoryFails()
        {
            CreateCategoryDto newCategoryDto = new CreateCategoryDto()
            {
                Name = "",
                InventionDate = 1300,
                InventionRegion = 32
            };

            await Assert.ThrowsAsync<NotValidParametersException>(() => service.CreateAsync(newCategoryDto));
        }

        [Fact]
        public async Task ModifyCategoryFailsNoEntity()
        {
            int modifyId = 10;
            CreateCategoryDto modifiedCategoryDto = new CreateCategoryDto()
            {
                Name = "Bor alapú étel",
                InventionDate = 1300,
                InventionRegion = 32
            };

            await Assert.ThrowsAsync<DbEntityNotFoundException>(() => service.ModifyAsync(modifyId, modifiedCategoryDto));
        }
        [Fact]
        public async Task ModifyCategoryFailsWrongParameters()
        {
            int modifyId = 1;
            CreateCategoryDto modifiedCategoryDto = new CreateCategoryDto()
            {
                Name = "",
                InventionDate = 1300,
                InventionRegion = 32
            };

            await Assert.ThrowsAsync<NotValidParametersException>(() => service.ModifyAsync(modifyId, modifiedCategoryDto));
        }
        [Fact]
        public async Task ModifyCategory()
        {
            int modifyId = 1;
            CreateCategoryDto modifiedCategoryDto = new CreateCategoryDto()
            {
                Name = "Bor alapú étel",
                InventionDate = 1000,
                InventionRegion = 32
            };

            var result = await service.ModifyAsync(modifyId, modifiedCategoryDto);

            Assert.Equal(1, result.Id);
            Assert.Equal("Bor alapú étel", result.Name);
            Assert.Equal("London", result.InventionRegion);
            Assert.Equal(1000, result.InventionDate);
        }
        private List<Category> SeedCategory()
        {
            var list = new List<Category>();
            var category1 = new Category {Id = 1, Name = "Pecsenye"};
            var category2 = new Category { Id = 2, Name = "Rák" };

            category1.Invention = new Invention()
            {
                Id = 1,
                CategoryID = 1,
                InventedIn = InventionRegion.Hungary,
                InventionDate = 1400
            };

            category2.Invention = new Invention()
            {
                Id = 2,
                CategoryID = 2,
                InventedIn = InventionRegion.China,
                InventionDate = 1600
            };

            list.Add(category1);
            list.Add(category2);
            return list;
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
