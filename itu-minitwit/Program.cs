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

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();


builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout duration
    options.Cookie.HttpOnly = true; // Make session cookie HttpOnly
    options.Cookie.IsEssential = true; // Mark session cookie as essential
});
builder.Services.AddDbContext<MiniTwitDbContext>(options =>
    options.UseSqlite("Data Source=minitwit.db"));

builder.Services.AddRazorPages();

<<<<<<< Updated upstream
var app = builder.Build();  
=======
var app = builder.Build();
>>>>>>> Stashed changes

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();



app.MapRazorPages();


app.Run();

