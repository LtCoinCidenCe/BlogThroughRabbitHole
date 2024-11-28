using System.ComponentModel.DataAnnotations;

namespace UserServer.Controllers;
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
