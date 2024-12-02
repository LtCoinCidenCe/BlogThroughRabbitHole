using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace VideoInformation.Models
{
    public class VideoComment
    {
        public long id {  get; set; }
        [JsonIgnore]
        [InverseProperty("Comments")]
        [Column(name:"VideoID")]
        [DeleteBehavior(DeleteBehavior.NoAction)]
        public Video? Video { get; set; }
        public long UserID { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;

        [Column(name: "VideoID")]
        [JsonIgnore]
        public long VideoID { get; set; }
    }
}
