using BlogReceptionist.Models;
using BlogReceptionist.Services;
using Microsoft.AspNetCore.Mvc;

namespace BlogReceptionist.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController(LoginService loginService) : ControllerBase
    {
        [HttpPost]
        public IActionResult login([FromBody]User logging)
        {
            if(!loginService.TryLogin(logging, out string token))
                return BadRequest();
            return Ok(token);
        }
    }
}
