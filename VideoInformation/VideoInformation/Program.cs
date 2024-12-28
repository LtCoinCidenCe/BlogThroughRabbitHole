using VideoInformation.DBContexts;
using VideoInformation.Services;
using VideoInformation.Utilities;

namespace VideoInformation;

public class Program
{
    public static Mutex mutex = new(false);
    public static void Main(string[] args)
    {
        // spin up
        Env.DATABASEURL.ToString();

        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<VideoContext>();
        builder.Services.AddScoped<VideoService>();
        builder.Services.AddScoped<CommentService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();

        app.UseAuthorization();

        app.Use((context, next) =>
        {
            try
            {
                mutex.WaitOne();
                return next(context);
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        });

        app.MapControllers();

        app.Run();
    }
}
