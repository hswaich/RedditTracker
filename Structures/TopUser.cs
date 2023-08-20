namespace RedditTracker.Structures
{
    public record TopUser : ISubreddit
    {
        public TopUser(string user, string subreddit)
        {
            User = user;
            Subreddit = subreddit;
        }

        public string User { get; set; }
        public string Subreddit { get; set; }

        public int PrintColumnNumber => 1;

        public string PrintLine => User;
    }
}
