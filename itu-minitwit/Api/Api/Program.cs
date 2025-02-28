using Api.DataAccess;
using Api.DataAccess.Models;
using Api.DataAccess.Repositories;
using Api.Services.RepositoryInterfaces;
using Api.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

//Repositories
builder.Services.AddScoped<ILatestRepository, LatestRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

//Services
builder.Services.AddScoped<ILatestService, LatestService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IUserService, UserService>();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MinitwitDbContext>(options =>
    options.UseSqlite(connection));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

namespace Api
{
    public partial class Program
    {
    }
}