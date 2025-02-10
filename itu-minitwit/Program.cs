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
builder.Services.AddDbContext<MiniTwitDbContext>(options =>
    options.UseSqlite("Data Source=minitwit.db"));

builder.Services.AddRazorPages();

var app = builder.Build();  

app.UseStaticFiles();
app.UseRouting();






app.Run();

