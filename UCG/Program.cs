using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using UCG.Models;
using UCG.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<UcgdbContext>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("ucgdbConnection"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("ucgdbConnection")) // Detecta la versión automáticamente
    );
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Acceso/Login";
        options.SlidingExpiration = true;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.AccessDeniedPath = "/Acceso/Denegado";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("AsocAdmin"));
    options.AddPolicy("UnionCantonalPolicy", policy => policy.RequireRole("UCAGAdmin"));
});

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<HashingService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
