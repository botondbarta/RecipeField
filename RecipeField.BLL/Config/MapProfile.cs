using AutoMapper;
using RecipeField.BLL.Dto;
using RecipeField.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Config
{
    public class MapProfile : Profile
    {
        public MapProfile()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<Category, CategoryDto>()
                .ForMember(s => s.InventionDate, m => m.MapFrom(t => t.Invention.InventionDate))
                .ForMember(s => s.InventionRegion, m => m.MapFrom(t => t.Invention.InventedIn.ToString()))
                .ForMember(s => s.Name, m => m.MapFrom(t => t.Name))
                .ForMember(s => s.Id, m => m.MapFrom(t => t.Id)).ReverseMap();
            CreateMap<Comment, CommentDto>().ReverseMap();
            CreateMap<Ingredient, IngredientDto>().ReverseMap();
            CreateMap<Recipe, RecipeDto>().ReverseMap();
        }
    }
}
