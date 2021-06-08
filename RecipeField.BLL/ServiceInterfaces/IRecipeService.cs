using RecipeField.BLL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.ServiceInterfaces
{
    public interface IRecipeService
    {
        Task<RecipeDetailsDto> GetByIdAsync(int id);
        Task<RecipeDto> CreateAsync(string userId, RecipeCreationDto recipeCreationDto);
        Task DeleteAsync(int id, string userId);
        Task<RecipeDto> ModifyAsync(int id, string userId, RecipeModificationDto recipeDto);
        Task<List<RecipeDto>> GetAllAsync();
        Task<List<RecipeDto>> GetByUserIdAsync(string id);
        Task<List<CommentDto>> GetCommentsByRecipeIdAsync(int id);
        Task AddCommentForRecipeIdAsync(int id, string userId, CommentCreationDto comment);
        Task ModifyCommentAsync(int recipeId, int commentId, string userId, CommentCreationDto comment);
        Task<List<RecipeIngredientDto>> GetIngredientsAsync(int recipeId);
        Task AddIngredientAsync(int recipeId, NewIngredientForRecipeDto ingredientDto, string userId);
        Task RemoveIngredientAsync(int recipeId, int ingredientId, string userId);
    }
}
