using Microsoft.EntityFrameworkCore;
using SignalRAssignment.Models;
using SignalRAssignment.Services;
using System.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<SalesManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});


builder.Services.AddAuthentication("SaleAppSaintRai")
    .AddCookie("SaleAppSaintRai", options =>
    {
        options.LoginPath = "/Members/Login";
    });


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StaffPolicy", policy =>
        policy.RequireClaim(ClaimTypes.Role, "Staff"));
    options.AddPolicy("UserPolicy", policy =>
            policy.RequireClaim(ClaimTypes.Role, "User"));
});

builder.Services.AddScoped<UploadImagesService>();
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<SearchService>();

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

app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static RSA GetPublicKey(string publicKeyPath)
{
    using var rsa = RSA.Create();
    var publicKey = File.ReadAllText(publicKeyPath);
    rsa.ImportFromPem(publicKey.ToCharArray());
    return rsa;
}

