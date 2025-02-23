using Api.DataAccess.Models;
using Api.Services.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.DataAccess.Repositories;

public class LatestRepository(MinitwitDbContext dbContext) : ILatestRepository
{
    public async Task<int> GetLatest()
    {
        var latestProcessedCommandId = await dbContext.LatestProcessedSimActions.FirstOrDefaultAsync();
        if (latestProcessedCommandId == null) return -1;
        return latestProcessedCommandId.Id;
    }

    public async Task UpdateLatest(int latest)
    {
        await dbContext.LatestProcessedSimActions.ExecuteDeleteAsync();

        var latestObj = new LatestProcessedSimAction { Id = latest };

        await dbContext.LatestProcessedSimActions.AddAsync(latestObj);
        await dbContext.SaveChangesAsync();
    }
}