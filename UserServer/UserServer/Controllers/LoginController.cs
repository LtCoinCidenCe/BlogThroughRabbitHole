using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using UserServer.Models;
using UserServer.Services;
using UserServer.Utilities;

namespace UserServer.Controllers;

//[Route("api/[controller]")]
[Route("api/login")]
[ApiController]
public class LoginController(UserService userService) : ControllerBase
{
    [HttpPost]
    public IActionResult login(NewUser logging)
    {
        User? found = userService.Get(logging.Username);
        if (found is null)
        {
            return BadRequest();
        }
        if (found.Passhash != userService.getHash(logging.Password))
        {
            return BadRequest();
        }
        var claims = new List<Claim> {
                new Claim("username", found.Username),
                new Claim("id", found.ID.ToString()),
            };
        var jwtToken = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: new SigningCredentials(
                new SymmetricSecurityKey(Env.APPSECRETBYTES),
                SecurityAlgorithms.HmacSha256Signature
                )
            );
        return Ok(new JwtSecurityTokenHandler().WriteToken(jwtToken));
    }
}
