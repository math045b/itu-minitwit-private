using itu_minitwit.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;

public class RegisterModel(MiniTwitDbContext db, IPasswordHasher<User> passwordHasher) : PageModel
{
    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Password { get; set; }
    [BindProperty] public string ConfirmPassword { get; set; }
    public string ErrorMessage { get; set; }

    public IActionResult OnPost()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return Page();
        }

        User user = new User
        {
            Username = Username,
            Email = Email,
            
        };
        user.PwHash = passwordHasher.HashPassword(user, Password);
        
        db.Users.Add(user);
        db.SaveChanges();
        
        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { "You were successfully registered and can login now" });
        
        return RedirectToPage("Login");
    }
}