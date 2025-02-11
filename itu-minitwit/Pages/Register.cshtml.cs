using itu_minitwit.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
[IgnoreAntiforgeryToken]
public class RegisterModel(MiniTwitDbContext db, IPasswordHasher<User> passwordHasher) : PageModel
{
    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Password { get; set; }
    [BindProperty] public string Password2 { get; set; }
    public string ErrorMessage { get; set; }

    public IActionResult OnPost()
    {
        if (string.IsNullOrWhiteSpace(Username))
        {
            ErrorMessage = "You have to enter a username";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "You have to enter a password";
            return Page();
        }
        if (Password != Password2)
        {
            ErrorMessage = "The two passwords do not match";
            return Page();
        }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@") || !Email.Contains("."))
        {
            ErrorMessage = "You have to enter a valid email address";
            return Page();
        }

        User user = new User
        {
            Username = Username,
            Email = Email,
            
        };
        user.PwHash = passwordHasher.HashPassword(user, Password);

        try
        {
            db.Users.Add(user);
            db.SaveChanges();
        }
        catch (Microsoft.EntityFrameworkCore.DbUpdateException)
        {
            ErrorMessage = "The username is already taken";
            return Page();
        }
        
        
        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { "You were successfully registered and can login now" });
        
        return RedirectToPage("Login");
    }
}