using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace VideoInformation.Models
{
    public class VideoComment
    {
        public long id {  get; set; }
        [JsonIgnore]
        [InverseProperty("Comments")]
        public long Videoid { get; set; }
        public long UserID { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
