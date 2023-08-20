namespace RedditTracker.Structures
{
    public record SubredditMessage
    {
        public string Subreddit { get; set; }
        public string Message { get; set; }
    }
}
