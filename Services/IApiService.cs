using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public interface IApiService
    {
        ResponseHeader ResponseHeader { get; set; }

        Task<string> GetToken();

        Task<ISubreddit> GetTopPostWithMostUpvotesAsync(string subreddit);

        Task<ISubreddit> GetUserWithMostPostsAsync(string subreddit);
    }
}
