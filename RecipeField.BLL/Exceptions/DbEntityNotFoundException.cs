using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Exceptions
{
    public class DbEntityNotFoundException : Exception
    {
        public DbEntityNotFoundException() : base("Entity not found in the database.")
        {

        }
    }
}
