using Newtonsoft.Json;

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
