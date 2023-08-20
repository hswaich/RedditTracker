namespace RedditTracker.Structures
{
    public record ApiLimit
    {
        public int RateLimitUsed { get; set; }
        public int RateLimitRemaining { get; set; }
        public int RateLimitReset { get; set; }
    }
}
