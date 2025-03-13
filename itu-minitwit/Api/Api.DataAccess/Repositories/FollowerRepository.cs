using Api.Services.Exceptions;
using Api.Services.RepositoryInterfaces;
using Api.DataAccess.Models;
using Api.Services;
using Api.Services.CustomExceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog.Core;

namespace Api.DataAccess.Repositories;

public class FollowRepository(MinitwitDbContext dbContext, ILogger<FollowRepository> logger) : IFollowRepository
{
    [LogMethodParameters]
    [LogReturnValueAsync]
    public async Task Follow(string username, string follow)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        var userToFollow = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == follow);

        if (user == null || userToFollow == null)
        {
            var e =new UserDoesntExistException($"\"{username}\" not found");
            logger.LogError($"{e.Message} - throw: {e.GetType()}");
            throw e;
        }
    
        var followRelation = await dbContext.Followers.FirstOrDefaultAsync(f => f.WhoId == user!.UserId
                                                                     && f.WhomId == userToFollow!.UserId);

        if (followRelation != null)
        {
            var e = new AlreadyFollowsUserException($"\"{username}\" already follows \"{follow}\"");
            logger.LogError($"{e.Message} - throw: {e.GetType()}");
            throw e;
        }
    
        followRelation = new Follower
        {
            WhoId = user.UserId,
            WhomId = userToFollow.UserId
        };
    
        await dbContext.Followers.AddAsync(followRelation);
        await dbContext.SaveChangesAsync();
    }

    [LogMethodParameters]
    [LogReturnValueAsync]
    public async Task Unfollow(string username, string unfollow)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        var userToUnfollow = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == unfollow);

        if (user == null || userToUnfollow == null)
        {
            var e = new UserDoesntExistException($"\"{username}\" not found");
            logger.LogError($"{e.Message} - throw: {e.GetType()}");
            throw e;
        }
    
        var followRelation =
            await dbContext.Followers.FirstOrDefaultAsync(f => f.WhoId == user!.UserId
                                                    && f.WhomId == userToUnfollow!.UserId);

        if (followRelation == null)
        {
            var e = new DontFollowUserException($"\"{username}\" doesn't follow \"{unfollow}\"");
            logger.LogError($"{e.Message} - throw: {e.GetType()}");
            throw e;
        }
    
        dbContext.Followers.Remove(followRelation);
        await dbContext.SaveChangesAsync();
    }

    [LogMethodParameters]
    [LogReturnValueAsync]
    public async Task<IEnumerable<string>> GetFollows(string username, int no = 100)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null)
        {
            var e = new UserDoesntExistException("User not found");
            logger.LogError($"{e.Message} - throw: {e.GetType()}");
            throw e;
        }

        var followsList = await dbContext.Followers
            .Where(f => f.WhoId == user.UserId)
            .Join(dbContext.Users,
                f => f.WhomId,
                u => u.UserId,
                (f, u) => u.Username
            )
            .Take(no)
            .ToListAsync();
        
        return followsList;
    }
}