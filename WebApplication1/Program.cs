using Microsoft.EntityFrameworkCore;
using AspNetCoreIdentity.Web.Models;
using AspNetCoreIdentity.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlCon"));
});

// Identity
builder.Services.AddIdentityExtension();

// Cookie options
builder.Services.ConfigureApplicationCookie(options =>
{
    var cookieBuilder = new CookieBuilder();
    cookieBuilder.Name = "identityCookie";

    options.LoginPath = new PathString("/Home/SignIn");
    options.LogoutPath = new PathString("/Member/Logout");

    options.Cookie = cookieBuilder;

    options.ExpireTimeSpan = TimeSpan.FromDays(5);
    options.SlidingExpiration = true;
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

app.UseAuthentication();
app.UseAuthorization();

// area için gerekli config
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
