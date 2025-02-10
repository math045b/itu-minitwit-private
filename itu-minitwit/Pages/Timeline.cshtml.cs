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
        Messages = new List<MessageModel>
        {
            new MessageModel { Username = "john_doe", Text = "Hello World!", EmailGravatarUrl = "https://www.gravatar.com/avatar/...", PublishedAt = DateTime.Now }
        };
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