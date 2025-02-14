namespace itu_minitwit;

public class LatestService
{
    private const string FilePath = "./latest_processed_sim_action_id.txt";

    public async Task<int> GetLatest()
    {
        var latestProcessedCommandId = -1;

        if (!File.Exists(FilePath)) return -1;
        
        try
        {
            var content = await File.ReadAllTextAsync(FilePath);
            latestProcessedCommandId = int.Parse(content);
        }
        catch
        {
            latestProcessedCommandId = -1;
        }

        return latestProcessedCommandId;
    }

    public async Task UpdateLatest(int latest)
    {
        if (latest != -1)
        { 
            await File.WriteAllTextAsync(FilePath, latest.ToString());
        }
    }
}