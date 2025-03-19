namespace Api.DataAccess.Models;

public class Message
{
    // Navigation property
    public User? Author { get; set; }
    
    public int MessageId { get; set; }

    public int AuthorId { get; set; }

    public string Text { get; set; } = null!;

    public int? PubDate { get; set; }

    public int? Flagged { get; set; }
}
