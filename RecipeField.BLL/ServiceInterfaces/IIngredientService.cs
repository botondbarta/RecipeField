using RecipeField.BLL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.ServiceInterfaces
{
    public interface IIngredientService
    {
        Task<List<IngredientDto>> GetAll();
        Task<IngredientDto> GetByIdAsync(int id);
        Task<IngredientDto> CreateAsync(IngredientDto ingredientDto);
        Task DeleteAsync(int id);
        Task<IngredientDto> ModifyAsync(IngredientDto ingredientDto);
    }
}
