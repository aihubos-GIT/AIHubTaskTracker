using AIHubTaskTracker.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/reports")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<ReportsController> _logger;
    public ReportsController(AppDbContext db, ILogger<ReportsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // POST /api/v1/reports/telegram-trigger
    [HttpPost("telegram-trigger")]
    public async Task<IActionResult> TelegramTrigger()
    {
        try
        {
            // Lấy 10 log mới nhất để gửi báo cáo
            var logs = await _db.Logs
                .OrderByDescending(l => l.created_at)
                .Take(10)
                .ToListAsync();


            _logger.LogInformation("Telegram report triggered. Count: {Count}", logs.Count);

            return Ok(new { message = "Telegram report triggered", count = logs.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to trigger Telegram report");
            return StatusCode(500, new { message = ex.Message });
        }
    }
}
