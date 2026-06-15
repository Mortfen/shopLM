using Microsoft.AspNetCore.Authentication.Cookies;
using shop.wb.Services;

var builder = WebApplication.CreateBuilder(args);
//Serves reg
builder.Services.AddRazorPages();
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<API>();
builder.Services.AddHttpClient("API", user =>
{ user.BaseAddress = new Uri("https://localhost:7168"); }
);
builder.Services.AddSession(x =>
{ x.IdleTimeout = TimeSpan.FromHours(2);
    x.Cookie.HttpOnly = true;  }
);
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(2); // как JWT
    });



var app = builder.Build();

app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
app.UseRouting();
app.MapRazorPages();
app.UseHttpsRedirection();
app.Run();

