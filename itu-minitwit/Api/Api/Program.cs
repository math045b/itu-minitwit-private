using Api.DataAccess;
using Api.DataAccess.Models;
using Api.DataAccess.Repositories;
using Api.Services.RepositoryInterfaces;
using Api.Services.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Filters;

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
builder.Services.AddScoped<IMessageRepository, MessageRepository>();


//Services
builder.Services.AddScoped<ILatestService, LatestService>();
builder.Services.AddScoped<IFollowService, FollowService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IMessageService, MessageService>();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<MinitwitDbContext>(options =>
    options.UseSqlite(connection));

// Configure Serilog
var logFolder = builder.Configuration["LogLocation:LogFolder"];
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()  // Log to console
    //log everything to a file
    .WriteTo.File($"{logFolder}/general/api_log-.txt", rollingInterval: RollingInterval.Day)
    //log errors to a separate file as well
    .WriteTo.File($"{logFolder}logs/errors/api_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error)  
    //filter to log things from the follow related namespaces
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                     (e.Properties["SourceContext"].ToString().Contains("Api.Controllers.FollowerController") ||
                                      e.Properties["SourceContext"].ToString().Contains("Api.Services.Services.FollowService") ||
                                      e.Properties["SourceContext"].ToString().Contains("Api.DataAccess.Repositories.FollowRepository")))
        .WriteTo.File($"{logFolder}/follow/general/follow_log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.File($"{logFolder}/follow/errors/follow_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error))
    //filter to log things from the message related namespaces
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                     (e.Properties["SourceContext"].ToString().Contains("Api.Controllers.MessageController") ||
                                      e.Properties["SourceContext"].ToString().Contains("Api.Services.Services.MessageService") ||
                                      e.Properties["SourceContext"].ToString().Contains("Api.DataAccess.Repositories.MessageRepository")))
        .WriteTo.File($"{logFolder}/message/general/message_log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.File($"{logFolder}/message/errors/message_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error))
    //filter to log things from the user related namespaces
    .WriteTo.Logger(lc => lc
        .Filter.ByIncludingOnly(e => e.Properties.ContainsKey("SourceContext") &&
                                     (e.Properties["SourceContext"].ToString().Contains("Api.Controllers.RegisterController") ||
                                      e.Properties["SourceContext"].ToString().Contains("Api.Services.Services.UserService") ||
                                      e.Properties["SourceContext"].ToString().Contains("Api.DataAccess.Repositories.UserRepository")))
        .WriteTo.File($"{logFolder}/user/general/user_log-.txt", rollingInterval: RollingInterval.Day)
        .WriteTo.File($"{logFolder}/user/errors/user_log-.txt", rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: Serilog.Events.LogEventLevel.Error))
    .CreateLogger();

// Add Serilog to .NET logging system
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();
builder.Host.UseSerilog();


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