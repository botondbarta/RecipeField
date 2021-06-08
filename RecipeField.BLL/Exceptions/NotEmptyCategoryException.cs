using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Exceptions
{
    public class NotEmptyCategoryException:Exception
    {
        public NotEmptyCategoryException():base("Cannot be deleted as there is atleast one recipe in this category :c")
        {
            
        }
    }
}
