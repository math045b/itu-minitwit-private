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
using Microsoft.EntityFrameworkCore;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true; // Make session cookie HttpOnly
    options.Cookie.IsEssential = true; // Mark session cookie as essential
});
builder.Services.AddDbContext<MiniTwitDbContext>(options =>
    options.UseSqlite("Data Source=minitwit.db"));

builder.Services.AddRazorPages();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/public");
        return;
    }
    await next();
});

app.MapRazorPages();

app.MapPost("/{whomUsername}/follow", (HttpContext context, string whomUsername, MiniTwitDbContext db) =>
{
    var whoUsername = context.Session.GetString("User");
    
    var who = db.Users.FirstOrDefault(u => u.Username == whoUsername);
    var whom = db.Users.FirstOrDefault(u => u.Username == whomUsername);
    
    if (who == null && whom == null) return Results.BadRequest("Invalid users.");
    
    var followRelation = db.Followers.FirstOrDefault(f => f.WhoId == who!.UserId && f.WhomId == whom!.UserId);
    
    if (followRelation != null) return Results.BadRequest("You already follow that user");

    followRelation = new Follower
    {
        WhoId = who!.UserId,
        WhomId = whom!.UserId
    };

    db.Followers.Add(followRelation);
    db.SaveChanges();
    
    return Results.Redirect($"/{whomUsername}");
});

app.Run();

