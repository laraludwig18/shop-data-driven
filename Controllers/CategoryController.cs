using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;

namespace Shop.Controllers
{
    [ApiController]
    [Route("v1/categories")]
    public class CategoryController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [ResponseCache(VaryByHeader = "User-Agent", Location = ResponseCacheLocation.Any, Duration = 30)]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<List<Category>>> GetCategories([FromServices] DataContext context)
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return Ok(categories);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Category>> GetCategory(
            [FromRoute] int id,
            [FromServices] DataContext context)
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);

            return Ok(category);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Categories.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { Message = "Cannot create category." });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "employee")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Category>> UpdateCategory(
            [FromRoute] int id,
            [FromBody] Category model,
            [FromServices] DataContext context)
        {
            if (id != model.Id)
                return NotFound(new { Message = "Category not found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<Category>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { Message = "Cannot update category." });
            }
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "employee")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Category>> DeleteCategory(
            [FromRoute] int id,
            [FromServices] DataContext context)
        {
            try
            {
                var category = await context.Categories.FirstOrDefaultAsync(x => x.Id == id);
                if (category == null)
                    return NotFound(new { Message = "Category not found." });

                context.Categories.Remove(category);
                await context.SaveChangesAsync();

                return Ok(category);
            }
            catch
            {
                return BadRequest(new { Message = "Cannot update category." });
            }
        }
    }
}