using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Dto
{
    public class RecipeDetailsDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public CategoryDto Category { get; set; }
        public List<IngredientForRecipeDto> Ingredients { get; set; } = new List<IngredientForRecipeDto>();
    }
}
