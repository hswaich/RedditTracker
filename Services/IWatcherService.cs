namespace RedditTracker.Services
{
    public interface IWatcherService
    {
        Task Run(List<string> subreddits);
    }
}
