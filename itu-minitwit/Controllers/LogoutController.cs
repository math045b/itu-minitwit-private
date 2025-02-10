using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace itu_minitwit.Controllers;

[Route("[controller]")]
public class LogoutController(MiniTwitDbContext db) : Controller
{
    [HttpGet("/logout")]
    public IActionResult Logout()
    {
        HttpContext.Session.Clear();
        
        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { "You were logged out" });
    
        return Redirect("/");
    }
}