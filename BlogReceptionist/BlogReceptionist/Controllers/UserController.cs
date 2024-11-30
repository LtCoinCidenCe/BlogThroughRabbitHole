using BlogReceptionist.Models;
using BlogReceptionist.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogReceptionist.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController(
        UserService userService,
        BlogService blogService,
        RedisService redisService) : ControllerBase
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> Get(string username)
        {
            User? user = await userService.GetUser(username);
            if (user is null)
            {
                return NotFound();
            }
            List<Blog>? personsBlogs = blogService.GetbyOwner(user.ID);
            if (personsBlogs is null)
            {
                return NotFound();
            }
            user.BlogsWritten = personsBlogs;
            redisService.setUser(user);
            return Ok(user);
        }

        [HttpGet("id/{id}")]
        public async Task<IActionResult> Get(long id)
        {
            User? fromCache = redisService.getUser(id);
            if (fromCache is not null)
            {
                Console.WriteLine("Cache hit");
                return Ok(fromCache);
            }
            User? user = await userService.GetUser(id);
            if (user is null)
            {
                return NotFound();
            }
            List<Blog>? personsBlogs = blogService.GetbyOwner(user.ID);
            if (personsBlogs is null)
            {
                return NotFound();
            }
            user.BlogsWritten = personsBlogs;
            redisService.setUser(user);
            return Ok(user);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(User newUser)
        {
            if (newUser.Password is null)
                return BadRequest();
            newUser.ID = 0;
            User? result = await userService.CreateUser(newUser);
            if (result is null)
                return BadRequest();
            return Created("post", result);
        }
    }
}
