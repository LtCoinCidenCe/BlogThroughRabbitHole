using Google.Protobuf.WellKnownTypes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
    [Authorize]
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
    [Authorize]
    public IActionResult Put(long id, [FromBody] string value)
    {
        if (!TryGetTokenUserID(out var fromToken))
        {
            return Unauthorized();
        }
        if (id != fromToken)
        {
            return Unauthorized();
        }
        userService.UpdatePassword(id, value);
        return Ok();
    }

    // DELETE api/<UserController>/5
    [HttpDelete("{id}")]
    [Authorize]
    public IActionResult Delete(long id)
    {
        if (!TryGetTokenUserID(out var fromToken))
        {
            return Unauthorized();
        }
        if (id != fromToken)
        {
            return Unauthorized();
        }
        userService.Remove(id);
        return NoContent();
    }

    private bool TryGetTokenUserID(out long ID)
    {
        var identity = HttpContext.User.Identity as ClaimsIdentity;
        if (identity is null)
        {
            ID = 0;
            return false;
        }
        IEnumerable<Claim> claims = identity.Claims;
        string? idstr = claims.FirstOrDefault(pair => pair.Type == "id")?.Value;
        if (idstr is null)
        {
            ID = 0;
            return false;
        }
        if (!long.TryParse(idstr, out ID))
        {
            return false;
        }
        return true;
    }
}
