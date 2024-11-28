using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserServer.Models;
using UserServer.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace UserServer.Controllers;


//[Route("api/[Controller]")]
[Route("api/user")]
[ApiController]
public class UserController(UserService userService) : ControllerBase
{
    [HttpGet]
    public IEnumerable<User> Get()
    {
        return userService.GetAll();
    }

    // GET api/<UserController>/5
    [HttpGet("id/{id}")]
    public ActionResult<User> Get(long id)
    {
        User? found = userService.Get(id);
        if (found is null)
        {
            return NotFound();
        }
        return Ok(found);
    }

    [HttpGet("{username}")]
    public ActionResult<User> Get(string username)
    {
        User? found = userService.Get(username);
        if (found is null)
        {
            return NotFound();
        }
        return Ok(found);
    }

    // POST api/<UserController>
    [HttpPost]
    public IActionResult Post([FromBody] NewUser user)
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
