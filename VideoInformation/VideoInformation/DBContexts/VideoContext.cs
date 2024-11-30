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
        optionsBuilder.UseNpgsql($"Host={Env.DATABASEURL};Database=bloglist;Username=postgres;Password=mysecretpassword");
    }

    public DbSet<Video> Video => Set<Video>();
    public DbSet<VideoComment> VideoComment => Set<VideoComment>();
}
