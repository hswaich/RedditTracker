using Newtonsoft.Json;

namespace RedditTracker.Structures
{
    public class RedditPostData
    {
        public string Subreddit { get; set; }
        
        public string Title { get; set; }

        public int Ups { get; set; }

        public string Author { get; set; }

        public decimal Created { get; set; }

        [JsonProperty("num_comments")]
        public int CommentsCount { get; set; }

        public DateTime Date
        {
            get {
                return DateTimeOffset.FromUnixTimeSeconds((long)Created).DateTime;
            }
        }
    }
}
