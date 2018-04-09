using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OWASP.Configuration;
using OWASP.DAL;
using OWASP.DAL.Security;
using System.Data.SqlClient;
using System.Linq;

namespace OWASP.Controllers
{
    [Route("api/User")]
    [Consumes("application/json")]
    [Authorize]
    public class UserController : Controller
    {
        private ApplicationDbContext _context;
        private SecurityService _security;
        private ApplicationOptions _options;

        public UserController(ApplicationDbContext context, SecurityService security,
            ApplicationOptions options)
        {
            _context = context;
            _security = security;
            _options = options;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_context.Users.ToList());
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetById([FromRoute] string id)
        {
            var dbEntry = _context.Users.SingleOrDefault(u => u.UserId == id);
            if (dbEntry == null) return NotFound();

            return Ok(dbEntry);
        }

        [HttpGet]
        [Route("insecure")]
        public IActionResult GetByIdInsecure([FromRoute] string id)
        {
            using(SqlConnection connection = new SqlConnection(_options.ConnectionStrings["DefaultConnection"]))
            {
                connection.Open();
                SqlCommand command = new SqlCommand($"SELECT [Username], [EmailAddress] FROM dbo.Users", connection);

                using(var reader = command.ExecuteReader())
                {
                    //var results = new List<string>();
                    //while(reader.Read())
                    //{
                    //    reader.GetValues(row.ToArray());
                    //    results.Add(string.Join(" ", row.Select(f => f.ToString())));
                    //}

                    return Ok();
                }
            }
        }

        [HttpPost]
        public IActionResult Post([FromBody] User user)
        {
            // Password rules
            if (_context.WeakPasswords.Any(w => w.Password == user.Password))
                return BadRequest(new { Error = "Failed. Insecure password." });

            _context.Add(user);
            _context.SaveChanges();
            return CreatedAtAction(nameof(Get), new { Id = user.UserId }, user);
        }
    }
}
