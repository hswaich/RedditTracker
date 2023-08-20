using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditTracker.Structures
{
    public record ApiKeys
    {
        public string clientId { get; set; }
        public string clientSecret { get; set; }
    }
}
