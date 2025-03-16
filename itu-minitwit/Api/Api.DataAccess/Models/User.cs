namespace Api.DataAccess.Models;

public class User
{
    // Navigation property
    public ICollection<Message>? Messages { get; set; }
    
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PwHash { get; set; } = null!;
}
