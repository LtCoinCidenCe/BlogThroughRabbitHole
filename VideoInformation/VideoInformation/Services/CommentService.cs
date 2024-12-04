using Microsoft.EntityFrameworkCore;
using VideoInformation.DBContexts;
using VideoInformation.Models;

namespace VideoInformation.Services;

public class CommentService(VideoContext videoContext)
{
    public List<VideoComment> GetCommentByUserID(long user)
    {
        List<VideoComment> comments = videoContext.VideoComment
            .Where(comm => comm.UserID == user)
            .ToList();
        return comments;
    }

    public List<VideoComment>? GetCommentOnVideoByUser(long video, long user)
    {
        // no
        //string qString = videoContext.Video
        //    .AsNoTracking()
        //    .Where(vd => vd.id == video)
        //    .Include(vd => vd.Comments)
        //    .ToQueryString();
        //Console.WriteLine(qString);
        // yes
        List<VideoComment> result = videoContext.VideoComment
            .AsNoTracking()
            .Where(comment =>
            comment.VideoID == video
            && comment.UserID == user)
            .ToList();
        return result;
    }

    public VideoComment? AddVideoComment(long videoID, VideoComment comment)
    {
        Video? vd = videoContext.Video.SingleOrDefault(vd => vd.id == videoID);
        if (vd is null)
        {
            return null;
        }
        vd.Comments.Add(comment);
        videoContext.SaveChanges();
        return comment;
    }
}
