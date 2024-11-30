using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using VideoInformation.DBContexts;
using VideoInformation.Models;

namespace VideoInformation.Services;

public class CommentService(VideoContext videoContext)
{
    public List<VideoComment> GetCommentByUserID(long user)
    {
        List<VideoComment> comments = videoContext.VideoComment.Where(comm => comm.UserID == user).ToList();
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
        Video? theVideo = videoContext.Video
            .AsNoTracking()
            .Where(vd => vd.id == video)
            .Include(vd => vd.Comments)
            .SingleOrDefault();
        if (theVideo is null)
        {
            return null;
        }
        //var result = videoContext.Entry(theVideo)
        //    .Collection(v => v.Comments)
        //    .Query()
        //    .Where(ll => ll.UserID == user)
        //    .ToList();
        return theVideo.Comments;
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
