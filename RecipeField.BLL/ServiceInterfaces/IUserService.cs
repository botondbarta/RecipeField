using RecipeField.BLL.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeField.BLL.ServiceInterfaces
{
    public interface IUserService
    {
        public Task<UserDto> Modify(string userId, UserModificationDto userMod);
        public Task<List<UserDto>> GetAll();
    }
}
