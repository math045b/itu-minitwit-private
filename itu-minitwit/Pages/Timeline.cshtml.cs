using System.Security.Cryptography;
using System.Text;
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

        Messages = messages
            .Select(m => new MessageModel
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

    
    public IActionResult OnPost(string text)
    {
        if (!string.IsNullOrWhiteSpace(text))
        {
            TempData["FlashMessages"] = new List<string> { "Message posted successfully!" };
        }
        return RedirectToPage();
    }
    
    
    private string GetGravatarUrl(string email, int size = 50)
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