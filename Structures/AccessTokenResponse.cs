namespace RedditTracker.Structures
{
    public record AccessTokenResponse
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public int expires_in { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
