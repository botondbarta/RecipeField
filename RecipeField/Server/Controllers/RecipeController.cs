using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeField.BLL.Dto;
using RecipeField.BLL.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace RecipeField.Server.Controllers
{
    [Route("api/recipes")]
    [ApiController]
    public class RecipeController : ControllerBase
    {
        private readonly IRecipeService recipeService;
        public RecipeController(IRecipeService _recipeService)
        {
            recipeService = _recipeService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Receptek lekérdezése",
            Description = "Összes recept lekérdezése")]
        [SwaggerResponse(StatusCodes.Status200OK, "A receptek lekérdezése sikeres volt")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Nem sikerült a lekérdezés")]
        public async Task<ActionResult<List<RecipeDto>>> GetAll()
        {
            var recipes = await recipeService.GetAllAsync();
            return Ok(recipes);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Recept lekérdezése",
            Description = "Adott recept lekérdezése leírással, hozzávalókkal együtt")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recept lekérdezése sikeres volt")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nem található ilyen recept")]
        [Route("{id}")]
        public async Task<ActionResult<RecipeDetailsDto>> GetById(int id)
        {
            var recipe = await recipeService.GetByIdAsync(id);
            return Ok(recipe);
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Hozzászólások lekérdezése",
            Description = "Adott recepthez tartozó összes hozzászólás lekérdezése")]
        [SwaggerResponse(StatusCodes.Status200OK, "Hozzászólások lekérdezése sikeres volt")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Nem sikerült a hozzászólások lekérdezése")]
        [Route("{id}/comments")]
        public async Task<ActionResult<List<CommentDto>>> GetCommentsByRecipeId(int id)
        {
            var comments = await recipeService.GetCommentsByRecipeIdAsync(id);
            return Ok(comments);
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Hozzászólás készítése",
            Description = "Adott recepthez hozzászólás készítése")]
        [SwaggerResponse(StatusCodes.Status200OK, "Hozzászólás sikeresen létrehozva")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Nem sikerült a hozzászólás létrehozása")]
        [Route("{id}/comments")]
        public async Task<ActionResult> PostCommentForRecipe(int id, [FromBody] CommentCreationDto commentDto)
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;

            await recipeService.AddCommentForRecipeIdAsync(id, userId, commentDto);
            return Ok();
        }

        //[Authorize(Policy = "LoggedInUser")]
        [HttpGet("user")]
        [SwaggerOperation(
            Summary = "Felhasználó receptjei",
            Description = "Felhasználó összes receptjének lekérdezése")]
        [SwaggerResponse(StatusCodes.Status200OK, "Receptek lekérdezése sikeres volt")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Nem sikerült a recepteket lekérni")]
        public async Task<ActionResult<List<RecipeDto>>> GetAllForUser()
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;

            var recipes = await recipeService.GetByUserIdAsync(userId);
            return Ok(recipes);
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Recept létrehozása",
            Description = "Recept létrehozása a megadott paraméterekkel")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recept sikeresen létrehozva")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nem található a kategória amihez hozzákellene adni")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Az adott paraméterekkel nem lehet receptet létrehozni")]
        public async Task<ActionResult<RecipeDto>> Create([FromBody] RecipeCreationDto recipeCreationDto)
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;

            var recipe = await recipeService.CreateAsync(userId, recipeCreationDto);
            return Ok(recipe);
        }

        [Authorize]
        [HttpPut]
        [SwaggerOperation(
            Summary = "Recept módosítása",
            Description = "Megadott recept módosítása megadott paraméterekkel")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recept sikeresen módosítva")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nem található recept/kategória")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Nem a felhasználó receptje")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Nem megfelelőek a paraméterek")]
        [Route("{id}")]
        public async Task<ActionResult<RecipeDto>> Modify(int id, [FromBody] RecipeModificationDto recipeModificationDto)
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;

            var recipe = await recipeService.ModifyAsync(id, userId, recipeModificationDto);
            return Ok(recipe);
        }

        [Authorize]
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Recept törlése",
            Description = "Megadott recept törlése")]
        [SwaggerResponse(StatusCodes.Status200OK, "Recept sikeresen törölve")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen recept")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Nem a felhasználó receptje")]
        [Route("{id}")]
        public async Task<ActionResult<RecipeDto>> Delete(int id)
        {
            var userId = this.HttpContext.User.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;

            await recipeService.DeleteAsync(id, userId);
            return Ok();
        }

        [Authorize]
        [HttpPut]
        [SwaggerOperation(
            Summary = "Hozzászólás módosítása",
            Description = "Megadott hozzászólás módosítása")]
        [SwaggerResponse(StatusCodes.Status200OK, "Hozzászólás sikeresen módosítva")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen hozzászólás")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Nem a felhasználó hozzászólása")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Nem megfelelőek a paraméterek")]
        [Route("{id}/comments/{commentid}")]
        public async Task<ActionResult> ModifyCommentForRecipe(int id, int commentid, [FromBody] CommentCreationDto commentDto)
        {
            var userId = GetUserIdFromHttpContext(this.HttpContext);
            await recipeService.ModifyCommentAsync(id, commentid, userId, commentDto);
            return Ok();
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Hozzávalók lekérdezése",
            Description = "Adott recepthez tartozó összes hozzávaló lekérdezése")]
        [SwaggerResponse(StatusCodes.Status200OK, "Hozzávalók sikeresen lekérdezve")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Nem sikerült a hozzávalókat lekérni")]
        [Route("{id}/ingredients")]
        public async Task<ActionResult<List<RecipeIngredientDto>>> GetIngredients(int id)
        {
            var ingredients = await recipeService.GetIngredientsAsync(id);
            return Ok(ingredients);
        }

        [Authorize]
        [HttpPost]
        [SwaggerOperation(
            Summary = "Hozzávaló hozzáadása",
            Description = "Adott recepthez új hozzávaló hozzáadása")]
        [SwaggerResponse(StatusCodes.Status200OK, "Hozzávaló sikeresen hozzáadva")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen hozzávaló/recept")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Nem a felhasználó receptje")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Nem megfelelőek a paraméterek")]
        [Route("{id}/ingredients")]
        public async Task<ActionResult> AddIngredients(int id, [FromBody] NewIngredientForRecipeDto ingredientDto)
        {
            string userId = GetUserIdFromHttpContext(this.HttpContext);

            await recipeService.AddIngredientAsync(id, ingredientDto, userId);
            return Ok();
        }

        [Authorize]
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Hozzávaló eltávolítása",
            Description = "Megadott receptből a megadott hozzávaló eltávolítása")]
        [SwaggerResponse(StatusCodes.Status200OK, "Hozzávaló sikeresen törölve")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "Nem a felhasználó receptje")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen hozzávaló/recept")]
        [Route("{id}/ingredients/{ingredientId}")]
        public async Task<ActionResult> RemoveIngredient(int id, int ingredientId)
        {
            string userId = GetUserIdFromHttpContext(this.HttpContext);

            await recipeService.RemoveIngredientAsync(id, ingredientId, userId);
            return Ok();
        }
        private string GetUserIdFromHttpContext(HttpContext http)
        {
            return http.User.Claims.First(x => x.Type.Equals(ClaimTypes.NameIdentifier)).Value;
        }
    }
}
