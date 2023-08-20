using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditTracker.Structures
{
    public interface ISubreddit
    {
        public string Subreddit { get; set; }

        public int PrintColumnNumber { get; }

        public string PrintLine { get; }
    }
}
