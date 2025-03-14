using System.Security.Cryptography;
using System.Text;

namespace Web.Services.DTO_s;

public class DisplayMessageDto(string message, string username, string email,  DateTime pubDate)
{
    public string Message { get; } = message;
    public string Username { get; } = username;
    public string GravatarUrl { get; } = GetGravatarUrl(email);
    public DateTime PubDate { get; } = pubDate;
    
    private static string GetGravatarUrl(string email, int size = 150)
    {
        var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower()));
        var hashString = string.Concat(hash.Select(b => b.ToString("x2")));
        return $"https://www.gravatar.com/avatar/{hashString}?d=identicon&s={size}";
    }
}