using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using RecipeField.BLL.Dto;
using RecipeField.BLL.Exceptions;
using RecipeField.BLL.ServiceInterfaces;
using RecipeField.DAL;
using RecipeField.DAL.Entities;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.Services
{
    public class UserService : IUserService
    {
        private readonly RecipeFieldDbContext db;
        private readonly IMapper mapper;
        private readonly UserManager<User> userManager;

        public UserService(RecipeFieldDbContext _db, IMapper _mapper, UserManager<User> _userManager)
        {
            db = _db;
            mapper = _mapper;
            userManager = _userManager;
            
        }

        public async Task<List<UserDto>> GetAll()
        {
            var users = await db.Users.ToListAsync();
            return mapper.Map<List<UserDto>>(users);
        }

        public async Task<UserDto> Modify(string userId, UserModificationDto userMod)
        {
            var user = await db.Users.Where(u => u.Id == userId).FirstOrDefaultAsync();
            if (user == null)
                throw new DbEntityNotFoundException();
            user.FirstName = userMod.FirstName;
            user.LastName = userMod.LastName;
            await db.SaveChangesAsync();
            return mapper.Map<UserDto>(user);
        }
    }
}
