using RedditTracker.Structures;

namespace RedditTracker.Repository
{
    public interface IDatabaseRepository
    {
        Task<bool> CheckDatabaseConnectionAndTable();
        
        Task Insert(List<SubredditMessage> subredditMessages);
    }
}