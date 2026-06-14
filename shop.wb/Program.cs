using shop.wb.Services;

var builder = WebApplication.CreateBuilder(args);
//Serves reg
builder.Services.AddRazorPages();
builder.Services.AddScoped<API>();
builder.Services.AddHttpClient("Api", user =>
{ user.BaseAddress = new Uri("https://localhost:7168"); }
);
builder.Services.AddSession(x =>
{ x.IdleTimeout = TimeSpan.FromMinutes(45);
    x.Cookie.HttpOnly = true;  }
);
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseSession(); 
app.MapRazorPages();

app.Run();

