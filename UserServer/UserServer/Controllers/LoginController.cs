using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UserServer.Controllers
{
    //[Route("api/[controller]")]
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        [HttpPost]
        public IActionResult login(NewUser logging)
        {
            return Unauthorized();
        }
    }
}
