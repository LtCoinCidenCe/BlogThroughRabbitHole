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

        public static int indexer = 0;
        public static Mutex mutex = new(false);

        public BlogController(MessageQueueService messageQueueService, ILogger<BlogController> logger)
        {
            this.messageQueueService = messageQueueService;
            this.logger = logger;
        }

        // GET: api/blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> Get()
        {
            mutex.WaitOne();
            int transID = ++indexer;
            mutex.ReleaseMutex();

            int i=0;
            for (; i < 5; i++)
            {
                await messageQueueService.GetAll(transID);
                if (messageQueueService.arrivalEvent.WaitOne(10 * 1000))
                {
                    break;
                }
            }
            if (i == 5) {
                return NotFound();
            }

            int j = 0;
            for (; j < 5; j++)
            {
                if (messageQueueService.resultQueue.ContainsKey(transID))
                    break;
                Thread.Sleep(200);
            }
            if (j == 5)
            {
                return NotFound();
            }
            List<Blog>? blogs = messageQueueService.resultQueue[transID].blogs;
            messageQueueService.resultQueue.TryRemove(transID, out var valvv);
            return blogs;
        }
    }
}
