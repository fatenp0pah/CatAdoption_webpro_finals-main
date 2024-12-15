using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql;
using CatAdoption.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configure services (equivalent to Startup.ConfigureServices)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 33)) // MySQL version
    ));

// Add authentication with cookie middleware
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Users/Login";   // Redirect to login page if unauthenticated
        options.LogoutPath = "/Users/Logout"; // Redirect after logout
        options.AccessDeniedPath = "/Users/Login"; // Redirect to login if access is denied
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // Session timeout
    });

// Add MVC services to support controllers with views
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Enable routing for controllers and views
app.UseRouting();

// Add authentication and authorization middlewares
app.UseAuthentication();
app.UseAuthorization();

// Define the default route for the application
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Login}/{id?}");



app.MapControllerRoute(
    name: "admin",
    pattern: "{controller=Admin}/{action=Dashboard}/{id?}");
app.Run();
