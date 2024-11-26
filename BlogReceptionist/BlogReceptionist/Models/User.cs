using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlogReceptionist.Models
{
    public class User
    {
        public long ID { get; set; }

        public string Username { get; set; } = string.Empty;

        [JsonIgnore]
        public string? Password { get; set; }

        public IEnumerable<Blog>? BlogsWritten { get; set; }
    }
}
