using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("tasks")]
public class TasksController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<TasksController> _logger;

    public TasksController(AppDbContext db, ILogger<TasksController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
    {
        var tasks = await _db.Tasks.Include(t => t.Member)
                                   .OrderByDescending(t => t.CreatedAt)
                                   .ToListAsync();
        return Ok(tasks);
    }

    // GET /tasks/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> Get(int id)
    {
        var task = await _db.Tasks.Include(t => t.Member)
                                  .FirstOrDefaultAsync(t => t.Id == id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    // POST /tasks
    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create([FromBody] TaskCreateDto dto)
    {
        var task = new TaskItem
        {
            TaskTitle = dto.TaskTitle,
            Status = dto.Status,
            Deadline = dto.Deadline,
            MemberId = dto.MemberId,
            CreatedAt = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var oldStatus = task.Status;

        task.Status = dto.Status ?? task.Status;
        task.Deadline = dto.Deadline ?? task.Deadline;
        task.MemberId = dto.MemberId ?? task.MemberId;

        await _db.SaveChangesAsync();

        // Tạo TaskLog
        var log = new TaskLog
        {
            TaskId = task.Id,
            MemberId = task.MemberId,
            OldStatus = oldStatus,
            NewStatus = task.Status,
            ChangedAt = DateTime.UtcNow
        };
        _db.TaskLogs.Add(log);
        await _db.SaveChangesAsync();

        return NoContent();
    }


    // DELETE /tasks/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
