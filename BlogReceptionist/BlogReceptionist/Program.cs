using BlogReceptionist.Services;
using BlogReceptionist.Controllers;
using static System.Net.Mime.MediaTypeNames;
using System;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System.Text;

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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

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
