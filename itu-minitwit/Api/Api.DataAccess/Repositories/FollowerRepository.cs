using Api.Services.Exceptions;
using Api.Services.RepositoryInterfaces;
using Api.DataAccess.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess.Repositories;

public class FollowRepository(MinitwitDbContext dbContext) : IFollowRepository
{
    public async Task Follow(string username, string follow)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        var userToFollow = dbContext.Users.FirstOrDefault(u => u.Username == follow);
    
        if (user == null || userToFollow == null) throw new UserDoesntExistException("User not found");
    
        var followRelation = dbContext.Followers.FirstOrDefault(f => f.WhoId == user!.UserId
                                                                     && f.WhomId == userToFollow!.UserId);
    
        if (followRelation != null) throw new AlreadyFollowsUserException("You already follow that user");
    
        followRelation = new Follower
        {
            WhoId = user.UserId,
            WhomId = userToFollow.UserId
        };
    
        dbContext.Followers.Add(followRelation);
        await dbContext.SaveChangesAsync();
    }

    public async Task Unfollow(string username, string unfollow)
    {
        var user = dbContext.Users.FirstOrDefault(u => u.Username == username);
        var userToUnfollow = dbContext.Users.FirstOrDefault(u => u.Username == unfollow);
    
        if (user == null || userToUnfollow == null) throw new UserDoesntExistException("User not found");
    
        var followRelation =
            dbContext.Followers.FirstOrDefault(f => f.WhoId == user!.UserId
                                                    && f.WhomId == userToUnfollow!.UserId);
    
        if (followRelation == null) throw new AlreadyFollowsUserException("You already unfollow that user");
    
        dbContext.Followers.Remove(followRelation);
        await dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<string>> GetFollows(string username, int no = 100)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null) throw new UserDoesntExistException("User not found");

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