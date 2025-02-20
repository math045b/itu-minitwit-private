using itu_minitwit.Data;
using Microsoft.EntityFrameworkCore;

namespace itu_minitwit;

public class LatestService(MiniTwitDbContext dbContext)
{
    public async Task<int> GetLatest()
    {
        var latestProcessedCommandId = await dbContext.LatestProcessedSimActions.FirstOrDefaultAsync();
        if (latestProcessedCommandId == null) return -1;
        return latestProcessedCommandId.Id;
    }

    public async Task UpdateLatest(int? latest)
    {
        var newLatest = latest ?? -1; 
        if (newLatest != -1)
        {
            await dbContext.LatestProcessedSimActions.ExecuteDeleteAsync();

            var latestObj = new LatestProcessedSimAction { Id = newLatest };

            await dbContext.LatestProcessedSimActions.AddAsync(latestObj);
            await dbContext.SaveChangesAsync();
        }
    }
}