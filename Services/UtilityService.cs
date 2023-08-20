using RedditTracker.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
