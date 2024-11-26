using BlogServer.DBContexts;
using BlogServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogServer.Services
{
    internal class BlogService(BlogContext blogContext)
    {
        public List<Blog> GetAll()
        {
            return blogContext.Blog.AsNoTracking().ToList();
        }

        public List<Blog> GetBlogsByOwner(long owner)
        {
            return blogContext.Blog.AsNoTracking().Where(b => b.OwnerID == owner).ToList();
        }
    }
}
