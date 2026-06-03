using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using shop.api.Data;






public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ////////////////////////////////
        ///
        builder.Services.AddDbContext<DB>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));



        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthorization();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
        builder.Services.AddControllers();
        /////////////////////////////////
        var app = builder.Build();
        //////////////////////////////////
        app.Logger.LogCritical("LogCritical ");
        app.Logger.LogError("LogError ");
        app.Logger.LogWarning("LogWarning ");
        app.Logger.LogInformation("LogInformation ");
        app.Logger.LogDebug("LogDebug ");
        app.Logger.LogTrace("LogTrace ");

        app.Use(async (context, next) =>
        {

            //check null DI
            var logfactoy = context.RequestServices.GetRequiredService<ILoggerFactory>();
            var logger = logfactoy.CreateLogger("Pipline");

            logger.LogInformation("Цепочка: {Method} {Path} ", context.Request.Method, context.Request.Path);


            await next();

            logger.LogInformation("Ответ: {StatusCode}", context.Response.StatusCode);
        });




        if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.UseStaticFiles();
        app.UseRouting();
        app.Run();
    }
}
////////////////////////////////
