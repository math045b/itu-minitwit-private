using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Scriban;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using itu_minitwit;
using Microsoft.EntityFrameworkCore;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
builder.Services.AddScoped<LatestService>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true; // Make session cookie HttpOnly
    options.Cookie.IsEssential = true; // Mark session cookie as essential
});

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MiniTwitDbContext>(options =>
    options.UseSqlite(connection));

builder.Services.AddRazorPages();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MiniTwitDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Session.GetString("User") != null)
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/Timeline");
            return;
        }
        await next();
    }
    else
    {
        if (context.Request.Path == "/")
        {
            context.Response.Redirect("/public");
            return;
        }
        await next();  
    }
    
});

app.MapRazorPages();
app.MapControllers();

app.Run();

