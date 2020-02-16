using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using shop.Data;
using shop.Models;
using shop.Services;

namespace shop.Controllers
{
    [Route("users")]
    public class UserController : ControllerBase
    {
        [HttpGet]
        [Route("")]
        [Authorize(Roles = "manager")]
        public async Task<ActionResult<List<User>>> Get([FromServices]DataContext context)
        {
            var users = await context.Users.AsNoTracking().ToListAsync();

            return Ok(users);
        }

        
        [HttpPost]
        [Route("")]
        [AllowAnonymous]
        //[Authorize(Roles = "manager")]
        public async Task<ActionResult<User>> Post([FromBody] User model, [FromServices]DataContext context)
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

                return BadRequest(new { message = "Não foi possível criar a categoria." });
            }
        }

        [HttpPut]
        [Route("{id:int}")]
        public async Task<ActionResult<User>> Put(int id, [FromBody] User model, [FromServices]DataContext context)
        {
            if (model.Id != id)
                return NotFound(new { message = "Usuário não encontrado" });

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                context.Entry<User>(model).State = EntityState.Modified;
                await context.SaveChangesAsync();

                return Ok(model);
            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Este registro já foi atualizado." });
            }
            catch (Exception)
            {
                return BadRequest(new { message = "Não foi possível editar o usuário." });
            }
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<ActionResult<dynamic>> Authenticate([FromBody] User model, [FromServices]DataContext context)
        {
            var user = await context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.UserName == model.UserName && x.Password == model.Password);

            if(user != null)
            return NotFound(new { message = "Usuário ou senha inválidos"});

            var token = TokenService.GenerateToken(user);
            user.Password = "";
            return new{
                user = user,
                token = token
            };
        }
        
    }
}
