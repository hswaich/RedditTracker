namespace RedditTracker.Structures
{
    public record ResponseHeader
    {
        public int RateLimitUsed { get; set; }
        public decimal RateLimitRemaining { get; set; }
        public decimal RateLimitReset { get; set; }

        public decimal DelaySeconds
        {
            get
            {
                if (RateLimitRemaining == 0)
                {
                    return 0;
                }
                else
                {
                    return RateLimitReset / RateLimitRemaining;
                }
            }
        }
    }
}
