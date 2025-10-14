namespace AIHubTaskTracker.Models
{
    public class BlacklistedTokens
    {
        public int Id { get; set; }
        public string Token { get; set; } = null!;
        public int UserId { get; set; }
        public DateTime RevokedAt { get; set; } = DateTime.Now;
        public DateTime ExpiresAt { get; set; } 
    }
}
