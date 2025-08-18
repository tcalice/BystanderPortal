using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Mvc;
using System;
using ForEveryAdventure.Models;
using ForEveryAdventure.Controllers;
using ForEveryAdventure.Services;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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
            
            // Add authentication services
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "https://accounts.google.com";
                    options.Audience = "232358876020-kv5mlakfb8ts9jn4oa6b7u9tfgfgap1b.apps.googleusercontent.com";
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("TripPlanning", policy =>
                policy.RequireClaim("scp", "Trip.Plan"));
            });

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Add authentication before authorization
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.MapTripPlanEndpoints();
            
            app.Run();
        }
    }
}