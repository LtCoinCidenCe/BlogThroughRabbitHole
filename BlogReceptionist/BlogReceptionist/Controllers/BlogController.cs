using BlogReceptionist.Models;
using BlogReceptionist.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogReceptionist.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        MessageQueueService messageQueueService;
        ILogger<BlogController> logger;

        public BlogController(MessageQueueService messageQueueService, ILogger<BlogController> logger)
        {
            this.messageQueueService = messageQueueService;
            this.logger = logger;
        }

        // GET: api/blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> Get()
        {
            int i;
            for (i = 0; i < 1; i++)
            {
                await messageQueueService.GetAll();
                if (messageQueueService.arrivalEvent.WaitOne(10 * 1000))
                {
                    break;
                }
            }
            if (i == 5) {
                return NotFound();
            }
            messageQueueService.resultQueue.TryDequeue(out List<Blog>? theResult);
            return theResult;
        }
    }
}
