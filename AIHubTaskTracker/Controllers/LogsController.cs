using AIHubTaskTracker.Data;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/v1/logs")]
public class LogsController : ControllerBase
{
    private readonly AppDbContext _db;
    public LogsController(AppDbContext db)
    {
        _db = db;
    }

    // POST /api/v1/logs
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Log log)
    {
        log.created_at = DateTime.UtcNow;
        _db.Logs.Add(log);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Log created", log });
    }

    // GET /api/v1/logs
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var logs = await _db.Logs
            .OrderByDescending(l => l.created_at)
            .ToListAsync();

        return Ok(logs);
    }


    // GET /api/v1/tasks/{task_id}/logs
    [HttpGet("/api/v1/tasks/{task_id}/logs")]
    public async Task<IActionResult> GetLogsByTask(int task_id)
    {
        var logs = await _db.Logs
            .Where(l => l.task_id == task_id)
            .OrderByDescending(l => l.created_at)
            .ToListAsync();

        return Ok(logs);
    }
}
