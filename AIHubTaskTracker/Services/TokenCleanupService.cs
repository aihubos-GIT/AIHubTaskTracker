using AIHubTaskTracker.Data;

public class TokenCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;

    public TokenCleanupService(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var expired = db.BlacklistedTokens.Where(t => t.ExpiresAt < DateTime.UtcNow);
            db.BlacklistedTokens.RemoveRange(expired);
            await db.SaveChangesAsync();
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
