using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditTracker.Structures
{
    public record ApiLimit
    {
        public int RateLimitUsed { get; set; }
        public int RateLimitRemaining { get; set; }
        public int RateLimitReset { get; set; }
    }
}
