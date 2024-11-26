using BlogReceptionist.Models;
using BlogReceptionist.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogReceptionist.Controllers
{
    [Route("api/blog")]
    [Route("api/blogs")]
    [ApiController]
    public class BlogController(BlogService blogService): ControllerBase
    {
        // GET: api/blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> Get()
        {
            IEnumerable<Blog>? result = await blogService.Get();
            if (result is null)
            {
                return Unauthorized();
            }
            return Ok(result);
        }
    }
}
