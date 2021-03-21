using System.Collections.Generic;
using System.Linq;
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
    [Route("v1/products")]
    public class ProductController : ControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<List<Product>>> GetProducts([FromServices] DataContext context)
        {
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .ToListAsync();

            return Ok(products);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Product>> GetProduct(
           [FromRoute] int id,
           [FromServices] DataContext context)
        {
            var product = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            return Ok(product);
        }

        [HttpGet("categories/{categoryId:int}")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<List<Product>>> GetByCategory(
           [FromRoute] int categoryId,
           [FromServices] DataContext context)
        {
            var products = await context
                .Products
                .Include(x => x.Category)
                .AsNoTracking()
                .Where(x => x.Category.Id == categoryId)
                .ToListAsync();

            return Ok(products);
        }

        [HttpPost]
        [Authorize(Roles = "employee")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<Category>> CreateProduct([FromBody] Product model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Products.Add(model);
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { Message = "Cannot create product." });
            }
        }
    }
}