using Microsoft.AspNetCore.Mvc;
using OWASP.DAL;

namespace OWASP.Controllers
{
    [Route("api/Test")]
    [Consumes("application/json")]
    public class TestController : Controller
    {
        public TestController(ApplicationDbContext context)
        {

        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Success");
        }

        [HttpPost]
        public IActionResult Post([FromBody] string save)
        {
            return Ok(save);
        }
    }
}
