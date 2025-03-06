using Api.DataAccess.Models;
using Api.Services.Dto_s.MessageDTO_s;
using Api.Services.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess.Repositories;

public class MessageRepository(MinitwitDbContext dbContext) : IMessageRepository
{
    public Task<List<DisplayMessageDTO>> ReadMessages()
    {
        return dbContext.Messages 
            .Join(dbContext.Users,
                m => m.AuthorId,
                a => a.UserId,
                (m,a) => new DisplayMessageDTO {Username = a.Username, Text = m.Text, PubDate = m.PubDate}
            )
            .OrderByDescending(displayMessage => displayMessage.PubDate)
            .Take(100)
            .ToListAsync();
    }

    public Task<List<DisplayMessageDTO>> ReadFilteredMessages(string username, int pagesize = 100)
    {
        var pageSize = pagesize;
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            throw new KeyNotFoundException($"User not found - Username: \"{username}\"");
        }
        
        return dbContext.Messages
            .Where(m => m.AuthorId == user.UserId && m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(pageSize)
            .Select(m => new DisplayMessageDTO
            {
                Username = username,
                Text = m.Text,
                PubDate = m.PubDate,
            })
            .ToListAsync();
        
    }

    public Task<bool> PostMessage(string username, string content)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null)
        {
            throw new KeyNotFoundException("User not found");
        }
        
        var message = new Message
        {
            AuthorId = user.UserId,
            Text = content,
            Flagged = 0,
            PubDate = (int)DateTimeOffset.Now.ToUnixTimeSeconds(),
        };
        
        dbContext.Messages.Add(message);
        dbContext.SaveChangesAsync();
        return Task.FromResult(true);
    }
}