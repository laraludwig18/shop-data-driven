using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Data;
using Shop.Models;
using Shop.Services;

namespace Shop.Controllers
{
    [ApiController]
    [Route("v1/users")]
    public class UserController : Controller
    {
        [HttpGet]
        [Authorize(Roles = "manager")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<List<User>>> GetUsers([FromServices] DataContext context)
        {
            var users = await context
                .Users
                .AsNoTracking()
                .ToListAsync();

            return Ok(users);
        }

        [HttpPost]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<User>> CreateUser([FromBody] User model, [FromServices] DataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                model.Role = "employee";

                context.Users.Add(model);
                await context.SaveChangesAsync();

                model.Password = "";

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { Message = "Cannot create user." });
            }
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "manager")]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<User>> UpdateUser(
            [FromRoute] int id,
            [FromBody] User model,
            [FromServices] DataContext context)
        {
            if (id != model.Id)
                return NotFound(new { Message = "User not found." });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch
            {
                return BadRequest(new { Message = "Cannot update user." });
            }
        }

        [HttpPost("login")]
        [AllowAnonymous]
        [Consumes(MediaTypeNames.Application.Json)]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model, [FromServices] DataContext context)
        {
            var user = await context
                .Users
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Username == model.Username && x.Password == model.Password);

            if (user == null)
                return NotFound(new { Message = "User or password invalid." });

            var token = TokenService.GenerateToken(user);

            model.Password = "";
            return new
            {
                user = user,
                token = token,
            };
        }
    }
}