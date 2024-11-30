using BlogReceptionist.Services;

namespace BlogReceptionist
{
    public class Program
    {
        // This should start multiple instances
        // limit max connections to databases' servers
        public static Semaphore limitOnProcess = new Semaphore(3, 3);
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<MessageQueueService>();
            builder.Services.AddSingleton<UserService>();
            builder.Services.AddSingleton<BlogService>();
            builder.Services.AddSingleton<LoginService>();
            builder.Services.AddSingleton<RedisService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // app.UseHttpsRedirection();

            app.UseFileServer();

            app.UseAuthorization();

            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                try
                {
                    limitOnProcess.WaitOne();
                    await next();
                }
                finally
                {
                    limitOnProcess.Release();
                }
            });

            app.UseWhen(context => context.Request.Path.StartsWithSegments("/sss"),
                config => config.Use(async (HttpContext context, Func<Task> next) =>
                {
                    Console.WriteLine("/sss here");
                    await next();
                }));

            app.MapControllers();

            app.Run();
        }
    }
}
