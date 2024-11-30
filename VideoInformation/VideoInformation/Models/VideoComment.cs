namespace VideoInformation.Models
{
    public class VideoComment
    {
        public long id {  get; set; }
        public Video? Video { get; set; }
        public long UserID { get; set; }
        public string Comment { get; set; } = string.Empty;
        public DateTime Created { get; set; } = DateTime.UtcNow;
    }
}
