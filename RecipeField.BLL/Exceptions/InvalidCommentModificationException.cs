using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Exceptions
{
    public class InvalidCommentModificationException : Exception
    {
        public InvalidCommentModificationException():base("This is not your comment! :o")
        {
                
        }
    }
}
