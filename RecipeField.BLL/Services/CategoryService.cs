using AutoMapper;
using RecipeField.BLL.Dto;
using RecipeField.BLL.ServiceInterfaces;
using RecipeField.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RecipeField.BLL.Exceptions;
using RecipeField.DAL.Entities;

namespace RecipeField.BLL.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly RecipeFieldDbContext db;
        private readonly IMapper mapper;
        public CategoryService(RecipeFieldDbContext _db, IMapper _mapper)
        {
            db = _db;
            mapper = _mapper;
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto)
        {
            if (categoryDto.InventionDate != null &&
                categoryDto.InventionRegion != null &&
                !string.IsNullOrEmpty(categoryDto.Name))
            {
                var category = new Category();
                category.Name = categoryDto.Name;
                category.Invention = new Invention();
                category.Invention.InventionDate = categoryDto.InventionDate;
                category.Invention.InventedIn = (InventionRegion)categoryDto.InventionRegion;

                var result = await db.Categories.AddAsync(category);
                await db.SaveChangesAsync();
                return mapper.Map<CategoryDto>(result.Entity);
            }

            throw new NotValidParametersException();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await db.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null)
                throw new DbEntityNotFoundException();
            var recipe = await db.Recipes.Where(r => r.CategoryID == id).FirstOrDefaultAsync();
            if (recipe != null)
                throw new NotEmptyCategoryException();
            db.Categories.Remove(category);
            await db.SaveChangesAsync();
        }

        public async Task<List<CategoryDto>> GetAll()
        {
            var categories = await db.Categories.ToListAsync();
            return mapper.Map<List<CategoryDto>>(categories);
        }

        public async Task<CategoryDto> GetByIdAsync(int id)
        {
            var category = await db.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
            if (category == null)
                throw new DbEntityNotFoundException();
            return mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> ModifyAsync(int id, CreateCategoryDto categoryDto)
        {
            if (categoryDto.InventionDate != null &&
                categoryDto.InventionRegion != null &&
                !string.IsNullOrEmpty(categoryDto.Name))
            {
                var category = await db.Categories.Where(c => c.Id == id).FirstOrDefaultAsync();
                if (category == null)
                    throw new DbEntityNotFoundException();
                category.Name = categoryDto.Name;
                category.Invention.InventionDate = categoryDto.InventionDate;
                category.Invention.InventedIn = (InventionRegion)categoryDto.InventionRegion;

                await db.SaveChangesAsync();
                return mapper.Map<CategoryDto>(category);
            }

            throw new NotValidParametersException();
        }
    }
}
