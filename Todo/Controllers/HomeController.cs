using Microsoft.AspNetCore.Mvc;
using Todo.Data;
using Todo.Models;

namespace Todo.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(
            [FromServices] AppDbContext context
        )
        {
            var retorno = context.Todos.ToList();
            return Ok(retorno);
        }

        [HttpGet]
        public IActionResult GetById(
            [FromServices] AppDbContext context,
            [FromQuery] int id
        )
        {
            var retorno = context.Todos.FirstOrDefault(x => x.Id == id);

            if (retorno is null)
                return NotFound();

            return Ok(retorno);
        }

        [HttpPost]
        public IActionResult Post(
            [FromServices] AppDbContext context,
            [FromBody] TodoModel model
        )
        {

            var todos = context.Todos.ToList();

            model.Id = todos.Count + 1;
            model.CreatedAt = DateTime.Now;
            model.Done = false;
            context.Todos.Add(model);
            context.SaveChanges();
            return Ok("Sucesso");
        }

        [HttpPut]
        public IActionResult Put(
            [FromServices] AppDbContext context,
            [FromBody] TodoUpdateModel model
        )
        {
            var todo = context.Todos.FirstOrDefault(x => x.Id == model.Id);

            todo.Done = model.Done;
            context.Update(todo);
            context.SaveChanges();

            return Ok(todo);
        }
    }
}