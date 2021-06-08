using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Exceptions
{
    public class NotValidParametersException : Exception
    {
        public NotValidParametersException() : base("Some parameters are not valid for this entity")
        {

        }
    }
}
