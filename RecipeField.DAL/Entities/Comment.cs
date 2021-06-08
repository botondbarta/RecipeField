using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.DAL.Entities
{
    public class Comment
    {
        public int Id { get; set; }
        public string Text { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int RecipeId{ get; set; }
        public Recipe Recipe { get; set; }
    }
}
