using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Dto
{
    public class CreateCategoryDto
    {
        public string Name { get; set; }
        public int InventionRegion { get; set; }
        public int InventionDate { get; set; }
    }
}
