using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public interface IApiService
    {
        ResponseHeader ResponseHeader { get; set; }

        Task<string> GetToken();

        Task<List<RedditPost>> GetData(string subreddit);
    }
}
