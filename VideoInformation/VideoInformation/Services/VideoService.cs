using Microsoft.EntityFrameworkCore;
using VideoInformation.DBContexts;
using VideoInformation.Models;

namespace VideoInformation.Services;

public class VideoService(VideoContext videoContext)
{
    public List<Video> GetAll()
    {
        List<Video> result = videoContext.Video
            .AsNoTracking()
            // .Include(v => v.Comments) // if split query
            .ToList();

        List<long> videoIDs = result.Select(ventity => ventity.id).ToList();
        // this doesn't give IN clause in PGSQL, weird
        List<VideoComment> comms = videoContext.VideoComment
            .AsNoTracking()
            .Where(comment => videoIDs.Contains(comment.VideoID))
            .ToList();

        foreach (VideoComment comment in comms)
        {
            result.Find(v => v.id == comment.VideoID)?.Comments.Add(comment);
        }
        return result;
    }

    public Video? GetVideo(long id)
    {
        Video? found = videoContext.Video
            .AsNoTracking()
            .Include(v => v.Comments)
            .Where(v => v.id == id)
            .AsSingleQuery()
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
