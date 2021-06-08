using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.DAL.Entities
{
    public class Recipeingredient
    {
        public int Id { get; set; }

        
        public int RecipeId { get; set; }
        public Recipe Recipe { get; set; }

       
        public int IngredientId { get; set; }
        public Ingredient Ingredient { get; set; }
        public string Quantity { get; set; }
    }
}
