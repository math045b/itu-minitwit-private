using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace itu_minitwit.Pages;

public class TimelineModel(MiniTwitDbContext db) : PageModel
{
    public string PageTitle { get; set; } = "My Timeline";
    public bool IsUserLoggedIn => HttpContext.Session.GetString("User") != null;
    public string Username => HttpContext.Session.GetString("User") ?? "Guest";
    public bool IsViewingOwnTimeline => true; // Adjust logic as needed
    public List<MessageModel> Messages { get; set; } = new List<MessageModel>();

    public void OnGet()
    {
        if (HttpContext.GetRouteValue("author")!.Equals("public"))
        {
            Messages = GetMessages();
        }
        else
        {
            Messages = GetUserMessages(HttpContext.GetRouteValue("author")!.ToString()!);
        }
        
    }

    public List<MessageModel> GetMessages()
    {
        return db.Messages
            .Where(m => m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(30)
            .Select(m => new MessageModel
            {
                Text = m.Text,
                PublishedAt = DateTimeOffset.FromUnixTimeSeconds((long)  m.PubDate!).DateTime,
                Username = db.Users
                    .Where(u => u.UserId == m.AuthorId)
                    .Select(u => u.Username)
                    .First()
            })
            .ToList();
    }

    public List<MessageModel> GetUserMessages(string author)
    {
        var author_id = db.Users.Where(u => u.Username == author).Select(u => u.UserId).FirstOrDefault();
        return db.Messages
            .Where(m => m.Flagged == 0)
            .Where(m => m.AuthorId == author_id)
            .OrderByDescending(m => m.PubDate)
            .Take(30)
            .Select(m => new MessageModel
            {
                Text = m.Text,
                PublishedAt = DateTimeOffset.FromUnixTimeSeconds((long)  m.PubDate!).DateTime,
                Username = db.Users
                    .Where(u => u.UserId == m.AuthorId)
                    .Select(u => u.Username)
                    .First()
            })
            .ToList();
    }

    public IActionResult OnPost(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            TempData["FlashMessages"] = new List<string> { "Message posted successfully!" };
        }
        return RedirectToPage();
    }
}

public class MessageModel
{
    public string Username { get; set; }
    public string Text { get; set; }
    //public string EmailGravatarUrl { get; set; }
    public DateTime PublishedAt { get; set; }
}