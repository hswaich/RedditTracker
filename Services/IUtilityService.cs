using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public interface IUtilityService
    {
        ResponseHeader GetResponseHeader(HttpResponseMessage response);

        string PrintLine(List<ISubreddit> subreddits);

        string PrintResponseHeaderStatus(ResponseHeader responseHeader, int delay);

        int CalculateDelay(ResponseHeader responseHeader, int taskCount);
    }
}
