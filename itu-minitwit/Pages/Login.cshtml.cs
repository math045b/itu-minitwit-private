using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;

public class LoginModel : PageModel
{
    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Password { get; set; }
    public string ErrorMessage { get; set; }

    public IActionResult OnPost()
    {
        if (Username == "test" && Password == "password") 
        {
            HttpContext.Session.SetString("User", Username);
            return RedirectToPage("/Timeline");
        }
        else
        {
            ErrorMessage = "Invalid credentials.";
            return Page();
        }
    }
}