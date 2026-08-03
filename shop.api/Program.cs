using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using shop.api.Data;
using shop.api.Data.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO;
using System.Linq;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks;






public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ////////////////////////////////
        ///
        builder.Services.AddDbContext<DB>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

        builder.Services.AddIdentity<User, IdentityRole<Guid>>().AddEntityFrameworkStores<DB>().AddDefaultTokenProviders();

        builder.Services.AddAuthentication(options =>
        {
            //def jwt
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

        })
              .AddJwtBearer(options => {
                  //RSA
                  var pub_key = RSA.Create();
                  pub_key.ImportFromPem(File.ReadAllText("key/key.pub"));
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,
                      ValidateAudience = true,
                      ValidateIssuerSigningKey = true,
                      ValidIssuer = "HobbyShop.api",
                      ValidAudience = "HobbyShop.Web",
                      IssuerSigningKey = new RsaSecurityKey(pub_key),
                      CryptoProviderFactory = new CryptoProviderFactory() { CacheSignatureProviders = false }//ошибка handles, если нет отключить




                  }; //логи jwt
                 
              });

        builder.Services.AddHttpClient("ollama", client =>
        {
            client.Timeout = TimeSpan.FromMinutes(3);
        });




        builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // игнор зацикла
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });


        builder.Services.Configure<FormOptions>(options =>
        {
            options.MultipartBodyLengthLimit = 10 * 1024 * 1024; // 10 МБ
        });

        builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

        builder.Services.AddAuthorization();
        builder.Services.AddOpenApi();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthorization();
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Information);
        builder.Services.AddControllers();
        builder.Services.AddHttpClient();
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

        using (var scope = app.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DB>();

            await db.Database.MigrateAsync(); 

            await DbSeeder.SeedAsync(scope.ServiceProvider); 
        }


        app.MapGet("/", () =>
            {
                return "Stable API";
            });

        if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseStaticFiles();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}

