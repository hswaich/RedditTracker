using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedditTracker.Structures
{
    public record Post
    {
        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("ups")]
        public int Upvotes { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
