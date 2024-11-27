using Microsoft.EntityFrameworkCore;
using UserServer.Models;

namespace UserServer.DBContext
{
    public class UserContext : DbContext
    {
        ILogger<UserContext> logger;
        string connectionString = $"server={Environment.GetEnvironmentVariable("DATABASEURL")};port=3306;database=bloglist;uid=root;password=mysecretpassword";

        public UserContext(
            DbContextOptions<UserContext> options,
            ILogger<UserContext> logger) : base(options)
        {
            this.logger = logger;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(connectionString);
        }

        public DbSet<User> Users => Set<User>();
    }
}
