using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using ForEveryAdventure.Models;
using ForEveryAdventure.Controllers;
using ForEveryAdventure.Services;
using Microsoft.OpenApi.Models;

namespace ForEveryAdventure
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ForEveryAdventure API",
                    Version = "v1",
                    Description = "API for ForEveryAdventure",
                    Contact = new OpenApiContact
                    {
                        Name = "Tony Calice",
                        Email = "tony@foreveryidea.com"
                    }
                });
            });
            builder.Services.AddSingleton<IAssetTagStore, AssetTagStore>();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.MapTripPlanEndpoints();
            
            app.Run();
        }
    }
}