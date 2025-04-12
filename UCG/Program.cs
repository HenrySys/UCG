using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using UCG.Models;
using UCG.Services;
using Rotativa.AspNetCore; // Importar Rotativa
using FluentValidation.AspNetCore;
using System.Reflection;

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
// Registrar todos los validadores automáticamente
builder.Services.AddControllersWithViews().AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssembly(Assembly.GetExecutingAssembly());
});



var app = builder.Build();


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.UseAuthentication();

RotativaConfiguration.Setup("C:\\Users\\Israel\\OneDrive\\Documentos\\GitHub\\UCG\\UCG\\wwwroot\\");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
