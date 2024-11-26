using BlogReceptionist.Controllers;
using BlogReceptionist.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogReceptionist.Services
{
    public class BlogService(MessageQueueService messageQueueService)
    {
        public static int indexer = 0;

        public async Task<IEnumerable<Blog>?> Get()
        {
            int transID = Interlocked.Add(ref indexer, 1);
            messageQueueService.transIDList.Add(transID, Thread.CurrentThread);
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
                if (DateTime.Now - timeout > TimeSpan.FromMilliseconds(2000))
                {
                    await messageQueueService.GetAll(transID);
                    timeout = DateTime.Now;
                }
            }
            if (i == FAILTIMES)
            {
                return null;
            }
            List<Blog>? blogs = messageQueueService.resultQueue[transID].blogs;
            lock (messageQueueService.transIDList)
            {
                messageQueueService.transIDList.Remove(transID);
            }
            messageQueueService.resultQueue.TryRemove(transID, out _);
            return blogs;
        }

        public async Task<List<Blog>?> GetbyOwner(long owner)
        {
            int transID = Interlocked.Add(ref indexer, 1);
            messageQueueService.transIDList.Add(transID, Thread.CurrentThread);
            int i = 0;
            await messageQueueService.GetByOwner(transID, owner);
            DateTime timeout = DateTime.Now;

            const int FAILTIMES = 20;
            for (; i < FAILTIMES; i++)
            {
                messageQueueService.arrivalEvent.WaitOne(500);
                if (messageQueueService.resultQueue.ContainsKey(transID))
                {
                    break;
                }
                if (DateTime.Now - timeout > TimeSpan.FromMilliseconds(2000))
                {
                    await messageQueueService.GetByOwner(transID, owner);
                    timeout = DateTime.Now;
                }
            }
            if (i == FAILTIMES)
            {
                return null;
            }
            List<Blog>? blogs = messageQueueService.resultQueue[transID].blogs;
            lock (messageQueueService.transIDList)
            {
                messageQueueService.transIDList.Remove(transID);
            }
            messageQueueService.resultQueue.TryRemove(transID, out _);
            return blogs;
        }
    }
}
