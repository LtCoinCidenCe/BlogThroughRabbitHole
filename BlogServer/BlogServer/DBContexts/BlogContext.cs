using BlogServer.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogServer.DBContexts
{
    internal class BlogContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Database=bloglist;Username=postgres;Password=mysecretpassword");
        }

        public DbSet<Blog> Blog => Set<Blog>();
    }
}
