using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditTracker.Structures
{
    public record ResponseHeader
    {
        public int RateLimitUsed { get; set; }
        public decimal RateLimitRemaining { get; set; }
        public decimal RateLimitReset { get; set; }

        public decimal Delay
        {
            get
            {
                if (RateLimitRemaining == 0)
                {
                    return 0;
                }
                else
                {
                    return RateLimitReset / RateLimitRemaining;
                }
            }
        }
    }
}
