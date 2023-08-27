using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public interface IUtilityService
    {
        ResponseHeader GetResponseHeader(HttpResponseMessage response);

        string PrintResponseHeaderStatus(ResponseHeader responseHeader, int delay);

        int GetDelay(ResponseHeader responseHeader);
    }
}
