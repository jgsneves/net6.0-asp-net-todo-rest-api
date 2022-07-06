using MeuTodo.Data;
using MeuTodo.Models;
using MeuTodo.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MeuTodo.Controllers
{
    [ApiController]
    [Route("v1")]
    public class TodoController : ControllerBase
    {
        [HttpGet]
        [Route("todos")]
        public async Task<IActionResult> Get([FromServices] AppDbContext context)
        {
            var todos = await context.Todos.AsNoTracking().ToListAsync();
            return Ok(todos);   
        }

        [HttpGet]
        [Route("todos/{id}")]
        public async Task<IActionResult> GetById(
            [FromServices] AppDbContext context,
            [FromRoute] int id
        )
        {
            var todo = await context.Todos.AsNoTracking().FirstOrDefaultAsync(todo => todo.Id == id);
            return todo == null ? NotFound() : Ok(todo);
        }

        [HttpPost("todos")]
        public async Task<IActionResult> Create(
            [FromServices] AppDbContext context,
            [FromBody] CreateTodoViewModel model
        )
        {
            if (!ModelState.IsValid) {
                return BadRequest();
            }

            var todo = new Todo
            {
                Date = DateTime.Now,
                Done = false,
                Title = model.Title
            };

            try
            {
                await context.Todos.AddAsync(todo);
                await context.SaveChangesAsync();
                return Created($"v1/todos/{todo.Id}", todo);
            }
            catch (Exception excep)
            {
                return BadRequest(excep);
            }
        }

        [HttpPut("todos/{id}")]
        public async Task<IActionResult> ChangeTodoDone(
            [FromServices] AppDbContext context,
            [FromRoute] int id
        )
        {
            var todo = await context.Todos.FirstOrDefaultAsync(todo => todo.Id == id);
            if (todo == null) {
                return NotFound();
            }

            try
            {
                todo.Done = true;
                context.Todos.Update(todo);
                await context.SaveChangesAsync();
                return Ok(todo);
            }
            catch (Exception excep) 
            {
                return BadRequest(excep);
            }
        }

        [HttpDelete("todos/{id}")]
        public async Task<IActionResult> Delete(
            [FromServices] AppDbContext context,
            [FromRoute] int id
        )
        {
            var todo = await context.Todos.FirstOrDefaultAsync(todo => todo.Id == id);

            if (todo == null) {
                return BadRequest();
            }

            try
            {
                context.Todos.Remove(todo);
                await context.SaveChangesAsync();
                return Ok("Todo removido do sistema!");
            }
            catch (System.Exception excep)
            {
                return BadRequest(excep);
            }
        }
    }
}