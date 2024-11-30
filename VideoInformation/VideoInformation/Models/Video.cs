using System.ComponentModel.DataAnnotations;

namespace VideoInformation.Models;
public class Video
{
    public long id { get; set; }
    public long AuthorID { get; set; }
    [Required]
    [MaxLength(120)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(510)]
    public string Description { get; set; } = string.Empty;
    public DateTime UploadTime { get; set; } = DateTime.UtcNow;
    public List<VideoComment> Comments { get; set; } = new List<VideoComment>();
}
