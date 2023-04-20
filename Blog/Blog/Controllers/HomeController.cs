// using Blog.Attributes;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        [HttpGet]
        //[ApiKey] -> Atributo criado para demonstração
        public IActionResult HealthCheck()
        {
            return Ok();
        }
    }
}
