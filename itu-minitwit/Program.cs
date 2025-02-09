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
var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

// Home route (Public Timeline)
app.MapGet("/", async (HttpContext context, MiniTwitDbContext db) =>
{
    var messages = db.Messages
                 .Where(m => m.Flagged == 0)
                 .OrderByDescending(m => m.PubDate)
                 .Take(30)
                 .Select(m => new
                 {
                     m.MessageId,
                     m.Text,
                     m.PubDate,
                     Username = db.Users
                                  .Where(u => u.UserId == m.AuthorId)
                                  .Select(u => u.Username)
                                  .FirstOrDefault()
                 })
                 .ToList();

    var templateText = await File.ReadAllTextAsync("Templates/timeline.html");
    var template = Template.Parse(templateText);
    string result = template.Render(new { messages });
    await context.Response.WriteAsync(result);
});

// User Timeline
app.MapGet("/{username}", async (HttpContext context, string username, MiniTwitDbContext db) =>
{
    var profileUser = db.Users.FirstOrDefault(u => u.Username == username);
    if (profileUser == null)
    {
        context.Response.StatusCode = 404;
        await context.Response.WriteAsync("User not found");
        return;
    }
    var messages = db.Messages
                     .Where(m => m.AuthorId == profileUser.UserId)
                     .OrderByDescending(m => m.PubDate)
                     .Take(30)
                     .ToList();
    var templateText = await File.ReadAllTextAsync("Templates/timeline.html");
    var template = Template.Parse(templateText);
    string result = template.Render(new { messages, username });
    await context.Response.WriteAsync(result);
});

// Add Message (POST)
app.MapPost("/add_message", async (HttpContext context, MiniTwitDbContext db) =>
{
    var form = await context.Request.ReadFormAsync();
    string text = form["text"];
    var userId = 1; // Replace with authenticated user's ID
    db.Messages.Add(new Message { AuthorId = userId, Text = text, PubDate = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), Flagged = 0 });
    db.SaveChanges();
    context.Response.Redirect("/");
});

// User Registration
app.MapGet("/register", async (HttpContext context) =>
{
    var templateText = await File.ReadAllTextAsync("Templates/register.html");
    var template = Template.Parse(templateText);
    string result = template.Render(new { error = "" });
    await context.Response.WriteAsync(result);
});

app.MapPost("/register", async (HttpContext context, MiniTwitDbContext db) =>
{
    var form = await context.Request.ReadFormAsync();
    string username = form["username"];
    string password = form["password"];
    string email = form["email"];
    if (db.Users.Any(u => u.Username == username))
    {
        context.Response.Redirect("/register?error=Username taken");
        return;
    }
    db.Users.Add(new User { Username = username, Email = email, PwHash = password });
    db.SaveChanges();
    context.Response.Redirect("/login");
});

// Login Route
app.MapGet("/login", async (HttpContext context) =>
{
    var templateText = await File.ReadAllTextAsync("Templates/login.html");
    var template = Template.Parse(templateText);
    string result = template.Render(new { error = "" });
    await context.Response.WriteAsync(result);
});

app.Run();

// Database Context
/*public class MiniTwitDbContext : DbContext
{
    public DbSet<User> user { get; set; }
    public DbSet<Message> message { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<User>(entity =>
        {
            //define auto increment key
            entity.HasKey(c => c.user_id);
            entity.Property(c => c.user_id)
                .ValueGeneratedOnAdd();

            entity.Property(a => a.username)
                .IsRequired();
            entity.Property(a => a.email)
                .IsRequired();

            entity.HasMany(u => u.follows)
                .WithMany(u => u.followers);
        });

        modelBuilder.Entity<Message>(entity =>
        {
            //define auto increment key
            entity.HasKey(c => c.message_id);
            entity.Property(c => c.message_id)
                .ValueGeneratedOnAdd();

            //define required fields
            entity.Property(c => c.text)
                .IsRequired()
                .HasMaxLength(160);
            entity.Property(c => c.pub_date)
                .IsRequired();

            //define foreign relation
            entity.HasOne(c => c.author)
                .WithMany(a => a.messages)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite("Data Source=minitwit.db");

    public List<Message> GetMessages() => message.ToList();
    public List<Message> GetUserMessages(string username) => message.Where(m => m.author_id == username).ToList();
    public void AddMessage(string author, string text)
    {
        message.Add(new Message { author_id = author, text = text });
        SaveChanges();
    }
    public void RegisterUser(string username, string password)
    {
        user.Add(new User { username = username, pw_hash = password });
        SaveChanges();
    }
}

public class User
{
    public int user_id { get; set; }
    public string username { get; set; }
    public string email { get; set; }
    public string pw_hash { get; set; }
    public List<Message> messages { get; set; } = [];

    public List<User> follows { get; set; }

    public List<User> followers { get; set; }
}

public class Message
{
    public int message_id { get; set; }
    public string author_id { get; set; }
    public string text { get; set; }
    public int pub_date { get; set; }
    public int flagged { get; set; }
    public User author { get; set; }
}*/
