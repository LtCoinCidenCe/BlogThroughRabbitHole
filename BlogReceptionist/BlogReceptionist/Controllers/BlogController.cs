using BlogReceptionist.Models;
using BlogReceptionist.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogReceptionist.Controllers
{
    [Route("api/blog")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        public static Dictionary<int,Thread> transIDList = new();
        MessageQueueService messageQueueService;
        ILogger<BlogController> logger;

        public static int indexer = 0;

        public BlogController(MessageQueueService messageQueueService, ILogger<BlogController> logger)
        {
            this.messageQueueService = messageQueueService;
            this.logger = logger;
        }

        // GET: api/blog
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Blog>>> Get()
        {
            int transID = Interlocked.Add(ref indexer, 1);

            transIDList.Add(transID, Thread.CurrentThread);
            int i = 0;

            await messageQueueService.GetAll(transID);
            DateTime timeout = DateTime.Now;

            const int FAILTIMES = 20;
            for (; i < FAILTIMES; i++)
            {
                messageQueueService.arrivalEvent.WaitOne(500);
                if (messageQueueService.resultQueue.ContainsKey(transID))
                {
                    break;
                }
                if(DateTime.Now - timeout > TimeSpan.FromMilliseconds(2000))
                {
                    await messageQueueService.GetAll(transID);
                    timeout = DateTime.Now;
                }
            }
            if (i == FAILTIMES) {
                return NotFound();
            }
            List<Blog>? blogs = messageQueueService.resultQueue[transID].blogs;
            lock (transIDList)
            {
                transIDList.Remove(transID);
            }

            messageQueueService.resultQueue.TryRemove(transID, out _);
            return blogs;
        }
    }
}
