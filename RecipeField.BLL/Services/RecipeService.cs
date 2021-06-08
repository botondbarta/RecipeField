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
    public class RecipeService: IRecipeService
    {
        private readonly RecipeFieldDbContext db;
        private readonly IMapper mapper;
        public RecipeService(RecipeFieldDbContext _db, IMapper _mapper)
        {
            db = _db;
            mapper = _mapper;
        }
        public async Task<RecipeDto> CreateAsync(string userId, RecipeCreationDto recipeCreationDto)
        {
            if (!string.IsNullOrEmpty(recipeCreationDto.Name) &&
                !string.IsNullOrEmpty(recipeCreationDto.Description) &&
                !string.IsNullOrEmpty(recipeCreationDto.CategoryName))
            {
                var category = await db.Categories.Where(c => c.Name == recipeCreationDto.CategoryName).FirstOrDefaultAsync();
                if (category == null)
                    throw new DbEntityNotFoundException();
                var recipe = new Recipe();
                recipe.Name = recipeCreationDto.Name;
                recipe.Description = recipeCreationDto.Description;
                recipe.CategoryID = category.Id;
                recipe.UserId = userId;
                

                var result = await db.Recipes.AddAsync(recipe);

                await db.SaveChangesAsync();
                return mapper.Map<RecipeDto>(result.Entity);
            }
            throw new NotValidParametersException();
        }
        public async Task DeleteAsync(int id, string userId)
        {
            var recipe = await db.Recipes.Where(r => r.Id == id).FirstOrDefaultAsync();
            if (recipe == null)
                throw new DbEntityNotFoundException();
            if (recipe.UserId != userId)
                throw new InvalidRecipeModificationException();
            db.Recipes.Remove(recipe);
            await db.SaveChangesAsync();
        }
        public async Task<RecipeDetailsDto> GetByIdAsync(int id)
        {
            var recipe = await db.Recipes.Where(r => r.Id == id)
                .Include(r => r.Category)
                .Include(r => r.Recipeingredients)
                .ThenInclude(e => e.Ingredient)
                .FirstOrDefaultAsync();
            if (recipe == null)
                throw new DbEntityNotFoundException();
            var result = new RecipeDetailsDto();
            result.Category = new CategoryDto();
            result.Category.Id = recipe.Category.Id;
            result.Category.Name = recipe.Category.Name;
            result.Category.InventionDate = recipe.Category.Invention.InventionDate;
            result.Category.InventionRegion = recipe.Category.Invention.InventedIn.ToString();
            result.Name = recipe.Name;
            result.Id = recipe.Id;
            result.Description = recipe.Description;
            foreach (var v in recipe.Recipeingredients)
                result.Ingredients.Add(new IngredientForRecipeDto { Id = v.Id, Name = v.Ingredient.Name, Quantity = v.Quantity });

            return result;
        }
        public async Task<RecipeDto> ModifyAsync(int recipeId, string userId, RecipeModificationDto recipeDto)
        {
            if (!string.IsNullOrEmpty(recipeDto.Description) && 
                !string.IsNullOrEmpty(recipeDto.Name) &&
                !string.IsNullOrEmpty(recipeDto.CategoryName))
            {
                var category = await db.Categories.Where(c => c.Name == recipeDto.CategoryName).FirstOrDefaultAsync();
                if (category == null)
                    throw new DbEntityNotFoundException();

                var recipe = await db.Recipes.Where(r => r.Id == recipeId).FirstOrDefaultAsync();
                if (recipe == null)
                    throw new DbEntityNotFoundException();

                if (recipe.UserId != userId)
                    throw new InvalidRecipeModificationException();

                recipe.Name = recipeDto.Name;
                recipe.Description = recipeDto.Description;
                recipe.CategoryID = category.Id;

                await db.SaveChangesAsync();
                return mapper.Map<RecipeDto>(recipe);
            }
            throw new NotValidParametersException();
        }
        public async Task<List<RecipeDto>> GetAllAsync()
        {
            var recipes = await db.Recipes.Include(r => r.Category).ToListAsync();
            return mapper.Map<List<RecipeDto>>(recipes);
        }
        public async Task<List<RecipeDto>> GetByUserIdAsync(string id)
        {
            var recipes = await db.Recipes.Include(r => r.Category).Where(r => r.UserId == id).ToListAsync();
            return mapper.Map<List<RecipeDto>>(recipes);
        }
        public async Task<List<CommentDto>> GetCommentsByRecipeIdAsync(int id)
        {
            var comments = await db.Comments.Where(c => c.RecipeId == id).Include(c => c.User).ToListAsync();
            return mapper.Map<List<CommentDto>>(comments);
        }
        public async Task AddCommentForRecipeIdAsync(int id, string userId, CommentCreationDto commentDto)
        {
            Comment comment = new Comment();
            comment.Text = commentDto.Text;
            comment.RecipeId = id;
            comment.UserId = userId;
            await db.Comments.AddAsync(comment);
            await db.SaveChangesAsync();
        }
        public async Task ModifyCommentAsync(int recipeId, int commentId, string userId, CommentCreationDto commentDto)
        {
            if (!string.IsNullOrEmpty(commentDto.Text))
            {
                var comment = await db.Comments.Where(c => c.Id == commentId).FirstOrDefaultAsync();
                if (comment == null)
                    throw new DbEntityNotFoundException();
                if (comment.UserId != userId)
                    throw new InvalidCommentModificationException();
                if (comment.RecipeId != recipeId)
                    throw new NotValidParametersException();
                comment.Text = commentDto.Text;
                await db.SaveChangesAsync();
            }
            else
                throw new NotValidParametersException();
        }
        public async Task<List<RecipeIngredientDto>> GetIngredientsAsync(int recipeId)
        {
            var recipeingrediets = await db.Recipeingredients
                                                                    .Include(ri => ri.Ingredient)
                                                                    .Where(ri => ri.RecipeId == recipeId)
                                                                    .ToListAsync();
            var ingredients = new List<RecipeIngredientDto>();
            foreach (var ri in recipeingrediets)
            {
                ingredients.Add(new RecipeIngredientDto
                {
                    Id = ri.Ingredient.Id,
                    Name = ri.Ingredient.Name,
                    Quantity = ri.Quantity
                });
            }

            return ingredients;
        }
        public async Task AddIngredientAsync(int recipeId, NewIngredientForRecipeDto ingredientDto, string userId)
        {
            var ingredient = await db.Ingredients.Where(i => i.Id == ingredientDto.IngredientId).FirstOrDefaultAsync();
            var recipe = await db.Recipes.Where(r => r.Id == recipeId).FirstOrDefaultAsync();
            if (recipe == null || ingredient == null)
                throw new DbEntityNotFoundException();
            if (recipe.UserId != userId)
                throw new InvalidRecipeModificationException();
            if (string.IsNullOrEmpty(ingredientDto.Quantity))
                throw new NotValidParametersException();

            Recipeingredient recipeingredient = new Recipeingredient();
            recipeingredient.Quantity = ingredientDto.Quantity;
            recipeingredient.RecipeId = recipeId;
            recipeingredient.IngredientId = ingredientDto.IngredientId;
            var result = await db.Recipeingredients.AddAsync(recipeingredient);
            await db.SaveChangesAsync();
        }
        public async Task RemoveIngredientAsync(int recipeId, int ingredientId, string userId)
        {
            var recipeIngredient = await db.Recipeingredients
                .Include(ri => ri.Recipe)
                .Where(ri => ri.IngredientId == ingredientId && ri.RecipeId == recipeId)
                .FirstOrDefaultAsync();
            if (recipeIngredient == null)
                throw new DbEntityNotFoundException();
            if (recipeIngredient.Recipe.UserId != userId)
                throw new InvalidRecipeModificationException();
            db.Recipeingredients.Remove(recipeIngredient);
            await db.SaveChangesAsync();
        }
    }
}
