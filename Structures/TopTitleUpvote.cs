namespace RedditTracker.Structures
{
    public record TopTitleUpvote : ISubreddit
    {
        public TopTitleUpvote(string title, int upvotes, string subreddit)
        {
            Title = title;
            Upvotes = upvotes;
            Subreddit = subreddit;
        }

        public string Title { get; set; }
        public int Upvotes { get; set; }
        public string Subreddit { get; set; }

        public int PrintColumnNumber => 2;

        public string PrintLine
        {
            get
            {
                if (string.IsNullOrEmpty(Title))
                {
                    return string.Empty;
                }
                else if (Title.Length > 10)
                {
                    return $"{Title.Substring(0, 10)}... {Upvotes}";
                }
                else
                {
                    return $"{Title} {Upvotes}";
                }
            }
        }
    }
}
