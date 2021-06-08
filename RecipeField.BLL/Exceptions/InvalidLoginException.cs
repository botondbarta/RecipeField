using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Exceptions
{
    public class InvalidLoginException : Exception
    {
        public InvalidLoginException()
        {
        }

        public InvalidLoginException(string message) : base(message)
        {
        }

        public InvalidLoginException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidLoginException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
