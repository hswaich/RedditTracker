namespace RedditTracker.Structures
{
    public class Statistic
    {
        public Statistic(string subreddit)
        {
            isAuthorPostsInitialized = false;
            Subreddit = subreddit;
            InitialTitleUpvotesCount = new Dictionary<string, int>();
            InitialAuthorPosts = new Dictionary<string, List<string>>();
            
            TitleUpvotesCount = new Dictionary<string, int>();
            AuthorPosts = new Dictionary<string, List<string>>();
        }
        
        public string Subreddit { get; set; }
        
        public bool isAuthorPostsInitialized { get; set; }

        public Dictionary<string, int> InitialTitleUpvotesCount { get; set; }
        public Dictionary<string, List<string>> InitialAuthorPosts { get; set; }

        public Dictionary<string, int> TitleUpvotesCount { get; set; }
        public Dictionary<string, List<string>> AuthorPosts { get; set; }
    }
}
