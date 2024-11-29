using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UserServer.Services;

namespace UserServer.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PingController : ControllerBase
{
    private readonly UserService _userService;
    public PingController(UserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public string Get()
    {
        _userService.ToString();
        return "pong";
    }
}
