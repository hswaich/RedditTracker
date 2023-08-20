namespace RedditTracker.Structures
{
    public interface ISubreddit
    {
        public string Subreddit { get; set; }

        public int PrintColumnNumber { get; }

        public string PrintLine { get; }
    }
}
