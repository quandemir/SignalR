using Microsoft.EntityFrameworkCore;
using SignalR.Api.Hubs;
using SignalR.Api.Models;

namespace SignalR.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //dbcontext
            builder.Services.AddDbContext<AppDbContext>(options =>
            {
                options.UseSqlServer(builder.Configuration["ConStr"]);
            });

            // Add services to the container.

            builder.Services.AddCors(opt =>
            {
                opt.AddPolicy("CorsPolicy", opts =>
                {
                    opts.WithOrigins("https://localhost:7092").AllowAnyHeader().AllowAnyMethod().AllowCredentials();
                });
            });

            builder.Services.AddControllers();

            //
            builder.Services.AddSignalR();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();


            app.MapControllers();

            //Clientlar huba baðlanmak için http://localhost:4400/MyHub
            app.MapHub<MyHub>("/MyHub");

            app.Run();
        }
    }
}