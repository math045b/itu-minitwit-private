using itu_minitwit.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;

namespace itu_minitwit.Controllers;

[Route("/")]
public class FollowController(MiniTwitDbContext db) : Controller
{
    [IgnoreAntiforgeryToken]
    [HttpGet("/{whomUsername}/follow")]
    public IActionResult FollowUser(string whomUsername)
    {
        var whoUsername = HttpContext.Session.GetString("User");
    
        var who = db.Users.FirstOrDefault(u => u.Username == whoUsername);
        var whom = db.Users.FirstOrDefault(u => u.Username == whomUsername);
    
        if (who == null && whom == null) return BadRequest("Invalid users.");
    
        var followRelation = db.Followers.FirstOrDefault(f => f.WhoId == who!.UserId && f.WhomId == whom!.UserId);
    
        if (followRelation != null) return BadRequest("You already follow that user");

        followRelation = new Follower
        {
            WhoId = who!.UserId,
            WhomId = whom!.UserId
        };

        db.Followers.Add(followRelation);
        db.SaveChanges();
        
        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { $"You are now following \"{whomUsername}\"" });
    
        return Redirect($"/{whomUsername}");
    }

    [HttpGet("/{whomUsername}/unfollow")]
    public IActionResult UnfollowUser(string whomUsername)
    {
        var whoUsername = HttpContext.Session.GetString("User");
    
        var who = db.Users.FirstOrDefault(u => u.Username == whoUsername);
        var whom = db.Users.FirstOrDefault(u => u.Username == whomUsername);
    
        if (who == null && whom == null) return BadRequest("Invalid users.");
    
        var followRelation = db.Followers.FirstOrDefault(f => f.WhoId == who!.UserId && f.WhomId == whom!.UserId);
    
        if (followRelation == null) return BadRequest("You dont follow that user");

        db.Followers.Remove(followRelation);
        db.SaveChanges();
        
        TempData["FlashMessages"] = JsonConvert.SerializeObject(new List<string> { $"You are no longer following \"{whomUsername}\"" });
    
        return Redirect($"/{whomUsername}");
    }
}