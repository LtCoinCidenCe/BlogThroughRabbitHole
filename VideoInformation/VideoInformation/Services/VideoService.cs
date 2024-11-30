using Microsoft.EntityFrameworkCore;
using VideoInformation.DBContexts;
using VideoInformation.Models;

namespace VideoInformation.Services;

public class VideoService(VideoContext videoContext)
{
    public List<Video> GetAll()
    {
        List<Video> result = videoContext.Video.AsNoTracking().ToList();
        return result;
    }

    public Video? GetVideo(long id)
    {
        Video? found = videoContext.Video
            .AsNoTracking()
            .Where(v => v.id == id)
            .Include(v => v.Comments)
            .SingleOrDefault();
        return found;
    }

    public Video CreateVideo(Video video)
    {
        videoContext.Video.Add(video);
        videoContext.SaveChanges();
        return video;
    }
}
