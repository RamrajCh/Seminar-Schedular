using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<MVCSMS>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("MVCSMS")));

// Add services to the container.

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();


builder.Services.AddDistributedMemoryCache();

// Add session
builder.Services.AddSession(
    options =>
    {
        options.IdleTimeout = TimeSpan.FromMinutes(300);
        options.Cookie.HttpOnly = true;
        options.Cookie.IsEssential = true;
    });

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

app.UseSession();

app.MapRazorPages();
app.MapDefaultControllerRoute();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=LandingPage}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "LogIn",
    pattern: "{controller=LogIn}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "SignUp",
    pattern: "{controller=SignUp}/{action=Index}/{id?}");


app.Run();
