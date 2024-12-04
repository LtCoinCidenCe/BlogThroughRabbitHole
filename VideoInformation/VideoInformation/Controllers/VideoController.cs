using Microsoft.AspNetCore.Mvc;
using VideoInformation.Models;
using VideoInformation.Services;

namespace VideoInformation.Controllers;

[Route("api/[controller]")]
[ApiController]
public class VideoController(VideoService videoService, CommentService commentService) : ControllerBase
{
    // GET: api/<VideoController>
    [HttpGet]
    public IEnumerable<Video> Get()
    {
        return videoService.GetAll();
    }

    // GET api/<VideoController>/5
    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        Video? vid = videoService.GetVideo(id);
        if (vid is null)
        {
            return NotFound();
        }
        return Ok(vid);
    }

    // POST api/<VideoController>
    [HttpPost]
    public Video Post([FromBody] Video newVideo)
    {
        newVideo.id = 0;
        Video result = videoService.CreateVideo(newVideo);
        return result;
    }

    [HttpGet("{video}/comment")]
    public IActionResult GetVideoComments(long video, [FromQuery] string userID = "")
    {
        if (!long.TryParse(userID, out long user))
        {
            Video? result = videoService.GetVideo(video);
            if (result is null)
            {
                return NotFound();
            }
            return Ok(result.Comments);
        }
        List<VideoComment>? comments = commentService.GetCommentOnVideoByUser(video, user);
        if (comments is null)
        {
            return NotFound();
        }
        return Ok(comments);
    }

    [HttpPost("{id}/comment")]
    public IActionResult AddComment(long id, VideoComment videoComment)
    {
        VideoComment? result = commentService.AddVideoComment(id, videoComment);
        if (result is null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}