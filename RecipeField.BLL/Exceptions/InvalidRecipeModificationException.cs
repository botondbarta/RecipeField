using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Exceptions
{
    public class InvalidRecipeModificationException : Exception
    {
        public InvalidRecipeModificationException() : base("This is not your recipe. :o")
        {

        }
    }
}
