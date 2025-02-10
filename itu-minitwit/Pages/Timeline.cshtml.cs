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
            .Select(m => new MessageModel
            {
                Username = db.Users
                    .Where(u => u.UserId == m.AuthorId)
                    .Select(u => u.Username)
                    .FirstOrDefault(),
                Text = m.Text,
                //PublishedAt = m.PubDate, // Handle nulls in PubDate
                EmailGravatarUrl = "some-url-based-on-email" // Add logic for Gravatar URL if needed
            })
            .ToList();

        Messages = messages;
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
    public string EmailGravatarUrl { get; set; }
    public DateTime PublishedAt { get; set; }
}