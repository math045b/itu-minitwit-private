using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
[IgnoreAntiforgeryToken]
public class LoginModel (MiniTwitDbContext db, IPasswordHasher<User> passwordHasher) : PageModel
{
    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Password { get; set; }
    public string ErrorMessage { get; set; }

    public IActionResult OnPost()
    {
        var user = db.Users
            .FirstOrDefault(u => u.Username == Username);

        if (user == null)
        {
            ErrorMessage = "Invalid username";
            return Page();
        }
        
        var result = passwordHasher.VerifyHashedPassword(user, user.PwHash, Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "Invalid password";
            return Page();
        }

        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { "You were logged in" });
        
        HttpContext.Session.SetString("User", Username);
        return LocalRedirect("~/public");
            
    }
}