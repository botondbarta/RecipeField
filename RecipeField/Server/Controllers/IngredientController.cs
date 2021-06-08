using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RecipeField.BLL.Dto;
using RecipeField.BLL.ServiceInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Swashbuckle.AspNetCore.Annotations;

namespace RecipeField.Server.Controllers
{
    [Route("api/ingredients")]
    [ApiController]
    public class IngredientController : ControllerBase
    {
        private readonly IIngredientService ingredientService;

        public IngredientController(IIngredientService ingredientService)
        {
            this.ingredientService = ingredientService;
        }

        [HttpGet]
        [SwaggerOperation(
            Summary = "Hozzávalók lekérdezése",
            Description = "Az összes hozzávaló lekérdezése")]
        [SwaggerResponse(StatusCodes.Status200OK, "A hozzávalók sikeresen lekérdezve")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Nem sikerült a hozzávalók lekérdezése")]
        public async Task<ActionResult<List<IngredientDto>>> GetAll()
        {
            var ingredients = await ingredientService.GetAll();
            return Ok(ingredients);
        }
        
        [HttpDelete]
        [SwaggerOperation(
            Summary = "Hozzávaló törlése",
            Description = "Törli a hozzávalót és törli az összes receptből ezt")]
        [SwaggerResponse(StatusCodes.Status200OK, "A hozzávaló sikeresen törölve")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen hozzávaló")]
        [Route("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            await ingredientService.DeleteAsync(id);
            return Ok();
        }

        [HttpPost]
        [SwaggerOperation(
            Summary = "Hozzávaló létrehozása",
            Description = "A megadott paraméterekkel létrehoz egy hozzávalót, ha nem megfelelőek a paraméterek hibát jelez")]
        [SwaggerResponse(StatusCodes.Status200OK, "A hozzávaló sikeresen létrehozva")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "A paraméterekkel nem lehet hozzávalót létrehozni")]
        public async Task<ActionResult<IngredientDto>> Create([FromBody] IngredientDto ingredientDto)
        {
            var ingredient = await ingredientService.CreateAsync(ingredientDto);
            return Ok(ingredient);
        }

        [HttpPut]
        [SwaggerOperation(
            Summary = "Hozzávaló módosítása",
            Description = "A megadott hozzávalót módosítja, ha nincs ilyen vagy a paraméterek rosszak, hibát jelez")]
        [SwaggerResponse(StatusCodes.Status200OK, "Hozzávaló sikeresen módosítva")]
        [SwaggerResponse(StatusCodes.Status404NotFound, "Nincs ilyen hozzávaló")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "A megadott paraméterekkel nem lehet hozzávalót módosítani")]
        [Route("{id}")]
        public async Task<ActionResult<IngredientDto>> Modify(int id, [FromBody] IngredientDto ingredientDto)
        {
            ingredientDto.Id = id;
            var ingredient = await ingredientService.ModifyAsync(ingredientDto);
            return Ok(ingredient);
        }
    }
}
