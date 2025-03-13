using Api.DataAccess.Models;
using Api.Services;
using Api.Services.Dto_s.MessageDTO_s;
using Api.Services.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Api.DataAccess.Repositories;

public class MessageRepository(MinitwitDbContext dbContext, ILogger<MessageRepository> logger) : IMessageRepository
{
    [LogMethodParameters]
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

    [LogMethodParameters]
    public Task<List<DisplayMessageDTO>> ReadFilteredMessages(string username, int pagesize = 100)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        if (user == null)
        {
            var e = new KeyNotFoundException($"User \"{username}\" found");
            logger.LogError($"{e.Message} - throw: {e.GetType()}");
            throw e;
        }
        
        return dbContext.Messages
            .Where(m => m.AuthorId == user.UserId && m.Flagged == 0)
            .OrderByDescending(m => m.PubDate)
            .Take(pagesize)
            .Select(m => new DisplayMessageDTO
            {
                Username = username,
                Text = m.Text,
                PubDate = m.PubDate,
            })
            .ToListAsync();
        
    }
    
    [LogMethodParameters]
    [LogReturnValue]
    public Task<bool> PostMessage(string username, string content)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        
        if (user == null)
        {
            var e = new KeyNotFoundException($"User \"{username}\" not found");
            logger.LogError($"{e.Message} - throw: {e.GetType()}");
            throw e;
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