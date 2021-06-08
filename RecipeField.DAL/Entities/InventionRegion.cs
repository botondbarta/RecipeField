using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.DAL.Entities
{
    [Flags]
    public enum InventionRegion
    {
        China = 1,
        Russia = 2,
        Hungary = 4,
        USA = 8,
        Canada = 16,
        London = 32
    }
}
