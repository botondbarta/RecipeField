using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecipeField.BLL.Dto;

namespace RecipeField.BLL.ServiceInterfaces
{
    public interface ICategoryService
    {


        Task<List<CategoryDto>> GetAll();
        Task<CategoryDto> GetByIdAsync(int id);
        Task<CategoryDto> CreateAsync(CreateCategoryDto categoryDto);
        Task DeleteAsync(int id);
        Task<CategoryDto> ModifyAsync(int id, CreateCategoryDto categoryDto);
    }
}
