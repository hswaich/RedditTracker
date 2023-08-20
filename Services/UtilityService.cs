using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public class UtilityService : IUtilityService
    {
        private const int subredditNameMaxLength = 10;
        private const int subredditMessageMaxLength = 20;

        public ResponseHeader GetResponseHeader(HttpResponseMessage response)
        {
            var responseHeader = new ResponseHeader();
            if (response.Headers.TryGetValues("X-Ratelimit-Used", out var rateLimitUsedValues))
            {
                bool parse = int.TryParse(rateLimitUsedValues.FirstOrDefault(), out int result);
                if (parse)
                {
                    responseHeader.RateLimitUsed = result;
                }
            }
            if (response.Headers.TryGetValues("X-Ratelimit-Remaining", out var rateLimitRemainingValues))
            {
                bool parse = decimal.TryParse(rateLimitRemainingValues.FirstOrDefault(), out decimal result);
                if (parse)
                {
                    responseHeader.RateLimitRemaining = result;
                }
            }
            if (response.Headers.TryGetValues("X-Ratelimit-Reset", out var rateLimitResetValues))
            {
                bool parse = decimal.TryParse(rateLimitResetValues.FirstOrDefault(), out decimal result);
                if (parse)
                {
                    responseHeader.RateLimitReset = result;
                }
            }
            return responseHeader;
        }

        public string PrintLine(List<ISubreddit> subreddits)
        {
            var subreddit = subreddits.FirstOrDefault().Subreddit;

            List<string> printArray = new List<string>();
            printArray.Add(PrintLine(subreddit, subredditNameMaxLength));
            foreach (var result in subreddits.OrderBy(x => x.PrintColumnNumber))
            {
                printArray.Add(PrintLine(result.PrintLine, subredditMessageMaxLength));
            }

            return String.Join(" | ", printArray);
        }

        public string PrintResponseHeaderStatus(ResponseHeader responseHeader, int delay)
        {
            return $"{(int)responseHeader.RateLimitRemaining}/{responseHeader.RateLimitUsed} RMNG/used calls. {responseHeader.RateLimitReset}/{delay / 1000} reset/delay secs";
        }

        public int CalculateDelay(ResponseHeader responseHeader, int taskCount)
        {
            if (responseHeader == null)
            {
                return 0;
            }
            else if (taskCount > responseHeader.RateLimitRemaining)
            {
                Console.WriteLine($"Waiting {(int)responseHeader.RateLimitReset} for API rate limit refresh.");
                return (int)responseHeader.RateLimitReset;
            }
            else
            {
                return (int)(responseHeader.Delay * 1000 * taskCount);
            }
        }

        private string PrintLine(string line, int length)
        {
            string truncatedString = line.Length > length ? line.Substring(0, length) : line;
            string paddedString = truncatedString.PadRight(length);
            return paddedString;
        }
    }
}
