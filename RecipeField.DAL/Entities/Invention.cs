using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.DAL.Entities
{
    [Owned]
    public class Invention
    {
        public int Id { get; set; }
        public int CategoryID { get; set; }
        public InventionRegion InventedIn { get; set; }
        public int InventionDate { get; set; }
    }
}
