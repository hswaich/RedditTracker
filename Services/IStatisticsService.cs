using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public interface IStatisticsService
    {
        List<Statistic> Statistics { get; set; }
        
        Task<bool> Process(List<RedditPostData> list);
    }
}
