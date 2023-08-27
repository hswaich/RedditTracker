using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public class UtilityService : IUtilityService
    {
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

        public string PrintResponseHeaderStatus(ResponseHeader responseHeader, int delay)
        {
            return $"{(int)responseHeader.RateLimitRemaining}/{responseHeader.RateLimitUsed} RMNG/used calls. {responseHeader.RateLimitReset}/{delay / 1000} reset/delay secs";
        }
        
        public int GetDelay(ResponseHeader responseHeader)
        {
            if (responseHeader == null)
            {
                return 0;
            }
            else if (responseHeader.RateLimitRemaining == 0)
            {
                Console.WriteLine($"Waiting {(int)responseHeader.RateLimitReset} for API rate limit refresh.");
                return (int)responseHeader.RateLimitReset *1000;
            }
            else
            {
                return (int)(responseHeader.DelaySeconds * 1000);
            }
        }
    }
}
