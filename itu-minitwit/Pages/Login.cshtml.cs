using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

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
            ErrorMessage = "Invalid credentials.";
            return Page();
        }
        
        var result = passwordHasher.VerifyHashedPassword(user, user.PwHash, Password);
        if (result == PasswordVerificationResult.Failed)
        {
            ErrorMessage = "Invalid credentials.";
            return Page();
        }
        
        HttpContext.Session.SetString("User", Username);
        return RedirectToPage("/Timeline");
            
    }
}