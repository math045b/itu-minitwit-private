using System.Security.Cryptography;
using System.Text;

namespace Web.Services.DTO_s;

public class DisplayMessageDto
{
    public string Text { get; set; }
    public string Username { get; set; } 
    public string GravatarUrl { get; set; }
    public int PubDate { get; set; }

    public DisplayMessageDto()
    {
        
    }
    

    //{"Username":"test123","Text":"this is a test","PubDate":1741947254,"$type":"DisplayMessageDTO"}
    
    
    
    private static string GetGravatarUrl(string email, int size = 150)
    {
        var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(email.Trim().ToLower()));
        var hashString = string.Concat(hash.Select(b => b.ToString("x2")));
        return $"https://www.gravatar.com/avatar/{hashString}?d=identicon&s={size}";
    }
}