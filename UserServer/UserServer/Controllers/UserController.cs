using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using UserServer.DBContext;
using UserServer.Models;
using UserServer.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(UserService userService) : ControllerBase
    {
        // GET: api/<UserController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public ActionResult<User> Get(long id)
        {
            User? found = userService.Get(id);
            if (found is null)
            {
                return NotFound();
            }
            return Ok(found);
        }

        // POST api/<UserController>
        [HttpPost]
        public IActionResult Post([FromBody]NewUser user)
        {
            User inDB = new User() { ID = 0, Username = user.Username, Passhash = user.Password };
            try
            {
                User created = userService.Create(inDB);
                return CreatedAtAction("Post", created);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException is not null && ex.InnerException.Message.StartsWith("Duplicate entry"))
                {
                    return BadRequest("Duplicate Username");
                }
                return BadRequest();
            }
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }

    public class NewUser
    {
        [Required]
        [MaxLength(30)]
        [RegularExpression("^[a-z0-9_-]{5,20}$")]
        public string? Username { get; set; }

        [Required]
        [MinLength(7)]
        [MaxLength(60)]
        public string Password { get; set; } = string.Empty;
    }
}
