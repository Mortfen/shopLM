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
builder.Services.AddControllers();


var app = builder.Build();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();


app.MapRazorPages();
app.MapControllers();
app.Run();
//test
//maria.kuznetsova@test.com
//Пароль: Maria2024!
//2134@gmail.com
//Mortgfe2026!
// 21334@gmail.com
//Mortgfe2023!