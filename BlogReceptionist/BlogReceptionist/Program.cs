using BlogReceptionist.Services;

namespace BlogReceptionist
{
    public class Program
    {
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseFileServer();

            app.UseAuthorization();

            app.Use(async (HttpContext context, Func<Task> next) =>
            {
                await next();
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
