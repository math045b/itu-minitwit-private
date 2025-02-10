using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public class RegisterModel : PageModel
{
    [BindProperty] public string Username { get; set; }
    [BindProperty] public string Email { get; set; }
    [BindProperty] public string Password { get; set; }
    [BindProperty] public string ConfirmPassword { get; set; }
    public string ErrorMessage { get; set; }

    public void OnPost()
    {
        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Passwords do not match.";
            return;
        }

        TempData["FlashMessages"] = new List<string> { "Registration successful!" };
        Response.Redirect("/Login");
    }
}