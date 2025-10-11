using AIHubTaskTracker.Data;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("tasklogs")]
public class TaskLogsController : ControllerBase
{
    private readonly AppDbContext _db;

    public TaskLogsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /tasklogs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskLog>>> GetAll()
    {
        var logs = await _db.TaskLogs
                            .Include(l => l.Task)
                            .Include(l => l.Member)
                            .OrderByDescending(l => l.ChangedAt)
                            .ToListAsync();
        return Ok(logs);
    }
}
