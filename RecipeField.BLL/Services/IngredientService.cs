using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RecipeField.BLL.Dto;
using RecipeField.BLL.Exceptions;
using RecipeField.BLL.ServiceInterfaces;
using RecipeField.DAL;
using RecipeField.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Services
{
    public class IngredientService: IIngredientService
    {
        private readonly RecipeFieldDbContext db;
        private readonly IMapper mapper;
        public IngredientService(RecipeFieldDbContext _db, IMapper _mapper)
        {
            db = _db;
            mapper = _mapper;
        }
        
        public async Task<IngredientDto> GetByIdAsync(int id)
        {
            var ingredient = await db.Ingredients.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (ingredient == null)
                throw new DbEntityNotFoundException();
            return mapper.Map<IngredientDto>(ingredient);
        }
        
        public async Task<IngredientDto> CreateAsync(IngredientDto ingredientDto)
        {
            if (IsValid(ingredientDto))
            {
                var ingredient = mapper.Map<Ingredient>(ingredientDto);
                var result = await db.Ingredients.AddAsync(ingredient);
                await db.SaveChangesAsync();
                return mapper.Map<IngredientDto>(result.Entity);
            }
            throw new NotValidParametersException();
        }
        
        public async Task<IngredientDto> ModifyAsync(IngredientDto ingredientDto)
        {
            if (IsValid(ingredientDto))
            {
                var ingredient = await db.Ingredients.Where(i => i.Id == ingredientDto.Id).FirstOrDefaultAsync();
                if (ingredient == null)
                    throw new DbEntityNotFoundException();
                ingredient.Name = ingredientDto.Name;
                await db.SaveChangesAsync();
                return mapper.Map<IngredientDto>(ingredient);
            }
            throw new NotValidParametersException();
        }

        public async Task DeleteAsync(int id)
        {
            var ingredient = await db.Ingredients.Where(i => i.Id == id).FirstOrDefaultAsync();
            if (ingredient == null)
                throw new DbEntityNotFoundException();
            var recipeIngredients = await db.Recipeingredients.Where(ri => ri.IngredientId == id).ToListAsync();
            if (recipeIngredients.Count != 0)
            {
                using (var transaction = db.Database.BeginTransaction())
                {

                    foreach (var recipeIngredient in recipeIngredients)
                    {
                        db.Recipeingredients.Remove(recipeIngredient);
                    }

                    db.Ingredients.Remove(ingredient);
                    await db.SaveChangesAsync();
                    await transaction.CommitAsync();
                }
            }
            else
            {
                db.Ingredients.Remove(ingredient);
                await db.SaveChangesAsync();
            }

        }

        public async Task<List<IngredientDto>> GetAll()
        {
            var ingredients = await db.Ingredients.ToListAsync();
            return mapper.Map<List<IngredientDto>>(ingredients);
        }

        public bool IsValid(IngredientDto ingredient)
        {
            if (string.IsNullOrEmpty(ingredient.Name))
                return false;
            return true;
        }
    }
}
