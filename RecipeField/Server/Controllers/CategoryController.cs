using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RecipeField.BLL.Dto;
using RecipeField.BLL.ServiceInterfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace RecipeField.Server.Controllers
{
    [Route("api/categories")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            this.categoryService = categoryService;
        }
        

        [HttpGet]
        [SwaggerOperation(
            Summary = "Kategóriák lekérdezése",
            Description = "Az összes kategória lekérdezése")]
        [SwaggerResponse(StatusCodes.Status200OK, "A kategóriák sikeresen lekérdezve")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Nem sikerült a kategóriák lekérdezése")]
        public async Task<ActionResult<List<CategoryDto>>> GetAll()
        {
            var categories = await categoryService.GetAll();
            return Ok(categories);
        }


        [HttpDelete]
        [SwaggerOperation(
            Summary = "Kategória törlése",
            Description = "A törlés előtt megnézi, hogy nincsen ehhez a kategóriához tartozó recept és hogy egyáltalán létezik-e azadatbázisban ilyen kategória")]
        [SwaggerResponse(StatusCodes.Status200OK, "A kategória sikeresen törölve")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen kategória")]
        [SwaggerResponse(StatusCodes.Status403Forbidden, "A kategóriához tartozik recept, nem lehet törölni")]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await categoryService.DeleteAsync(id);
            return Ok();
        }


        [HttpPost]
        [SwaggerOperation(
            Summary = "Kategória létrehozása",
            Description = "Megadott paraméterekkel kategóriát hoz létre, ha nem jók a paraméterek hibát jelez")]
        [SwaggerResponse(StatusCodes.Status200OK, "Kategória sikeresen létrehozva")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "A megadott paraméterekkel nem lehet kategóriát létrehozni")]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto categoryDto)
        {
            var category = await categoryService.CreateAsync(categoryDto);
            return Ok(category);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Kategória módosítása",
            Description = "A megadott kategóriát módosítja, ha nincs ilyen vagy a paraméterek rosszak, hibát jelez")]
        [SwaggerResponse(StatusCodes.Status200OK, "Kategória sikeresen módosítva")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen kategória")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "A megadott paraméterekkel nem lehet kategóriát módosítani")]
        [Route("{id}")]
        public async Task<ActionResult<CategoryDto>> Modify(int id, [FromBody] CreateCategoryDto categoryDto)
        {
            var category = await categoryService.ModifyAsync(id, categoryDto);
            return Ok(category);
        }
    }
}
