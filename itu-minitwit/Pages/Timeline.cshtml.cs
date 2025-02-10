using System.Security.Cryptography;
using System.Text;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;


namespace itu_minitwit.Pages;

public class TimelineModel(MiniTwitDbContext db) : PageModel
{
    [BindProperty] public string Message { get; set; }
    public string PageTitle { get; set; } = "My Timeline";
    public bool IsUserLoggedIn => HttpContext.Session.GetString("User") != null;
    public string Username => HttpContext.Session.GetString("User") ?? "Guest";
    public List<MessageModel> Messages { get; set; } = new List<MessageModel>();
    public bool Follows { get; set; }

    public void OnGet()
    {
        var author = HttpContext.GetRouteValue("author")!.ToString()!;
        if (author.Equals("public"))
        {
            Messages = GetMessages();
        }
        else
        {
            Messages = GetUserMessages(author);
            if (IsUserLoggedIn)
            {
                Follows = DoesFollow(author);
            }
        }
    }

    public List<MessageModel> GetMessages()
    {
        var messages = db.Messages
            .Where(m => m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(30)
            .Select(m => new
            {
                m.Text,
                m.PubDate,
                m.AuthorId
            })
            .ToList();

        return messages.Select(m => new MessageModel
            {
                Text = m.Text,
                PublishedAt = DateTimeOffset.FromUnixTimeSeconds((long)m.PubDate!).DateTime,
                Username = db.Users
                    .Where(u => u.UserId == m.AuthorId)
                    .Select(u => u.Username)
                    .First(),
                EmailGravatarUrl = GetGravatarUrl(
                    db.Users
                        .Where(u => u.UserId == m.AuthorId)
                        .Select(u => u.Email)
                        .First())
            })
            .ToList();
    }

    public List<MessageModel> GetUserMessages(string author)
    {
        if (author == "Timeline")
        {
            author = Username;
        }

        var author_id = db.Users.Where(u => u.Username == author).Select(u => u.UserId).FirstOrDefault();

        var messages = db.Messages
            .Where(m => m.Flagged == 0)
            .Where(m => m.AuthorId == author_id)
            .OrderByDescending(m => m.PubDate)
            .Take(30)
            .Select(m => new
            {
                m.Text,
                m.PubDate,
                m.AuthorId
            })
            .ToList();

        return messages.Select(m => new MessageModel
            {
                Text = m.Text,
                PublishedAt = DateTimeOffset.FromUnixTimeSeconds((long)m.PubDate!).DateTime,
                Username = db.Users
                    .Where(u => u.UserId == m.AuthorId)
                    .Select(u => u.Username)
                    .First(),
                EmailGravatarUrl = GetGravatarUrl(
                    db.Users
                        .Where(u => u.UserId == m.AuthorId)
                        .Select(u => u.Email)
                        .First())
            })
            .ToList();
    }

    public bool DoesFollow(string whomUsername)
    {
        if (!IsUserLoggedIn) return false;

        var who = db.Users.FirstOrDefault(u => u.Username == Username);
        var whom = db.Users.FirstOrDefault(u => u.Username == whomUsername);

        if (who == null || whom == null) return false;
        
        var result = db.Followers.FirstOrDefault(f => f.WhoId == who.UserId && f.WhomId == whom.UserId);

        return result != null;
    }

    private string GetGravatarUrl(string email, int size = 50)
    {
        var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower()));
        var hashString = string.Concat(hash.Select(b => b.ToString("x2")));
        return $"https://www.gravatar.com/avatar/{hashString}?d=identicon&s={size}";
    }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Message))
        {
        }

        var message = new Message
        {
            AuthorId = db.Users.FirstOrDefault(c => c.Username == Username).UserId,
            Flagged = 0,
            PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
            Text = Message,
        };

        db.Messages.Add(message);
        db.SaveChanges();
        
        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { "Your message was recorded" });

        return RedirectToPage();
    }
}

public class MessageModel
{
    public string Username { get; set; }
    public string Text { get; set; }
    public string EmailGravatarUrl { get; set; }
    public DateTime PublishedAt { get; set; }
}