using Microsoft.EntityFrameworkCore;
using UserServer.Models;
using UserServer.Utilities;

namespace UserServer.DBContext
{
    public class UserContext : DbContext
    {
        ILogger<UserContext> logger;
        // i don't know how inside docker activate ssl
        string connectionString = $"server={Env.DATABASEURL};port=3306;database=bloglist;uid=root;password=mysecretpassword;SslMode=Disabled";

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
