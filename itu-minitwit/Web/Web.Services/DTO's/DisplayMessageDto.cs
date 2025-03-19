using System.Security.Cryptography;
using System.Text;

namespace Web.Services.DTO_s;

public class DisplayMessageDto
{
    public required string Username { get; set; } 
    public required string Email { get; set; }
    public required string Text { get; set; }
    public int PubDate { get; set; }

    public string GetGravatarUrl(int size = 150)
    {
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(Email.Trim().ToLower()));
        var hashString = string.Concat(hash.Select(b => b.ToString("x2")));
        return $"https://www.gravatar.com/avatar/{hashString}?d=identicon&s={size}";
    }
}