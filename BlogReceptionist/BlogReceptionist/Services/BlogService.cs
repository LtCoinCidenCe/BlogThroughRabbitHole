using BlogReceptionist.Controllers;
using BlogReceptionist.Models;
using Microsoft.AspNetCore.Mvc;

namespace BlogReceptionist.Services
{
    public class BlogService(MessageQueueService messageQueueService)
    {
        public IEnumerable<Blog>? GetAll()
        {
            return messageQueueService.Get();
        }

        public List<Blog>? GetbyOwner(long owner)
        {
            return messageQueueService.Get(owner);
        }
    }
}
