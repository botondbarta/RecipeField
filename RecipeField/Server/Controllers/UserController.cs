using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeField.BLL.Dto;
using RecipeField.BLL.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace RecipeField.Server.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;

        public UserController(IUserService userService)
        {
            this.userService = userService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Felhasználók lekérdezése",
            Description = "Az összes felhasználó lekérdezése")]
        [SwaggerResponse(StatusCodes.Status200OK, "Felhasználók sikeresen lekérdezve")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Nem sikerült a felhasználók lekérdezése")]
        public async Task<ActionResult<List<UserDto>>> GetAll()
        {
            var users = await userService.GetAll();
            return Ok(users);
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Felhasználó módosítása",
            Description = "A felhasználó adatait módosítja")]
        [SwaggerResponse(StatusCodes.Status200OK, "Felhasnáló adatainak módosítása sikeres volt")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen felhasználó")]
        public async Task<ActionResult<UserDto>> Modify([FromBody] UserModificationDto user)
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;
            var result = await userService.Modify(userId, user);
            return Ok(result);
        }
    }
}
