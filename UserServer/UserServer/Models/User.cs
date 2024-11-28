using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace UserServer.Models
{
    [Index(nameof(Username), IsUnique = true)]
    public class User
    {
        public long ID { get; set; }

        [Required]
        [MaxLength(30)]
        [RegularExpression("^[a-zA-Z0-9_-]{5,20}$")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [StringLength(64)]
        [JsonIgnore]
        public string Passhash { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; }= DateTime.Now;
    }
}
