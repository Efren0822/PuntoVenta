using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PuntoVenta.Models;
using MySql.Data.EntityFrameworkCore.Extensions;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("SqlServerConnection");

//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");




builder.Services.AddControllersWithViews();



builder.Services.AddDbContext<ApplicationDbContext>(options =>
options.UseSqlServer(connectionString, sqlServerOptionsAction: sqlOptions =>
{
    sqlOptions.EnableRetryOnFailure();
}));

/*
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(connectionString,
    ServerVersion.Parse("8.0.28")));
*/


var app = builder.Build();

// Configurar el pipeline de solicitud HTTP.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Usuarios}/{action=Login}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Productos}/{id?}");
});

app.Run();
