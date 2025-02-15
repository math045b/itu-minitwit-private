using System.Security.Cryptography;
using System.Text;
using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;


namespace itu_minitwit.Pages;
[IgnoreAntiforgeryToken]
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
        return db.Messages
            .Where(m => m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(30)
            .Join(db.Users,
                m => m.AuthorId,
                a => a.UserId,
                (m,a) => new MessageModel
                {
                    Text = m.Text, 
                    PublishedAt = DateTimeOffset.FromUnixTimeSeconds((long)m.PubDate!).DateTime,
                    Username = a.Username,
                    EmailGravatarUrl = GetGravatarUrl(a.Email, 50),
                }
            )
            .ToList();
    }

    public List<MessageModel> GetUserMessages(string author)
{
    // Get the ID of the user
    int author_id = db.Users
        .Where(u => u.Username == author)
        .Select(u => u.UserId)
        .FirstOrDefault();

    if (author == "Timeline")
    {
        author = Username; 
        author_id = db.Users
            .Where(u => u.Username == author)
            .Select(u => u.UserId)
            .FirstOrDefault();

        // Get the list of followed user IDs
        var followedUserIds = db.Followers
            .Where(f => f.WhoId == author_id)
            .Select(f => f.WhomId)
            .ToList();

        // Get messages from the logged-in user and the followed users
        var messages = db.Messages
            .Where(m => m.Flagged == 0 && 
                        (m.AuthorId == author_id || followedUserIds.Contains(m.AuthorId)))
            .OrderByDescending(m => m.PubDate)
            .Take(30)
            .ToList(); 

        return MapMessages(messages);
    }
    else
    {
        // Show ONLY the specified author's messages
        var messages = db.Messages
            .Where(m => m.Flagged == 0 && m.AuthorId == author_id)
            .OrderByDescending(m => m.PubDate)
            .Take(30)
            .ToList();

        return MapMessages(messages);
    }
}
private List<MessageModel> MapMessages(List<Message> messages)
{
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

    private static string GetGravatarUrl(string email, int size = 50)
    {
        var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower()));
        var hashString = string.Concat(hash.Select(b => b.ToString("x2")));
        return $"https://www.gravatar.com/avatar/{hashString}?d=identicon&s={size}";
    }
}

public class MessageModel
{
    public string Username { get; set; }
    public string Text { get; set; }
    public string EmailGravatarUrl { get; set; }
    public DateTime PublishedAt { get; set; }
}