using Microsoft.EntityFrameworkCore;
using VideoInformation.Models;
using VideoInformation.Utilities;

namespace VideoInformation.DBContexts;

public class VideoContext : DbContext
{
    ILogger<VideoContext> logger;
    public VideoContext(DbContextOptions<VideoContext> options,
        ILogger<VideoContext> DILogger) : base(options)
    {
        logger = DILogger;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql($"Host={Env.DATABASEURL};Database=bloglist;Username=postgres;Password=mysecretpassword"
        , npgsqlOptionsAction: option => option.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // generate good utc time on database
        // time order implemented by this is not guaranteed with the same order as id
        // in postgres
        // id is unique no problem
        modelBuilder
            .Entity<VideoComment>()
            .Property(e => e.Authenticated)
            .HasDefaultValueSql("now()");
    }

    public DbSet<Video> Video => Set<Video>();
    public DbSet<VideoComment> VideoComment => Set<VideoComment>();
}
