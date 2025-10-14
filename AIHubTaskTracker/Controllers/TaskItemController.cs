using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/tasks")]
[Authorize]
public class TasksItemController : ControllerBase
{
    private readonly AppDbContext _db;

    public TasksItemController(AppDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskCreateDto dto)
    {
        var assigner = await _db.Members.FindAsync(dto.assigner_id);
        var assignee = await _db.Members.FindAsync(dto.assignee_id);
        if (assigner == null || assignee == null)
            return BadRequest(new { message = "Người giao hoặc thực hiện không hợp lệ." });

        var task = new AIHubTaskTracker.Models.TaskItem
        {
            title = dto.title,
            description = dto.description,
            assigner_id = dto.assigner_id,
            assignee_id = dto.assignee_id,
            collaborators = dto.collaborators ?? new List<int>(),
            expected_output = dto.expected_output,
            deadline = dto.deadline,
            status = dto.status ?? "To Do",
            progress_percentage = dto.progress_percentage,
            notion_link = dto.notion_link,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();
        return Ok(task);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] string? status, [FromQuery] int? assignee_id)
    {
        var query = _db.Tasks.AsQueryable();

        if (!string.IsNullOrEmpty(status))
            query = query.Where(t => t.status == status);

        if (assignee_id.HasValue)
            query = query.Where(t => t.assignee_id == assignee_id.Value);

        var tasks = await query.ToListAsync();
        return Ok(tasks);
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> Get(int id)
    {
        var task = await _db.Tasks
            .Include(t => t.assigner)
            .Include(t => t.assignee)
            .FirstOrDefaultAsync(t => t.task_id == id);
        if (task == null) return NotFound();
        return Ok(task);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskUpdateDto dto)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var oldStatus = task.status;

        task.title = dto.title ?? task.title;
        task.description = dto.description ?? task.description;
        task.expected_output = dto.expected_output ?? task.expected_output;
        task.collaborators = dto.collaborators ?? task.collaborators;
        task.deadline = dto.deadline ?? task.deadline;
        task.status = dto.status ?? task.status;
        task.progress_percentage = dto.progress_percentage ?? task.progress_percentage;
        task.notion_link = dto.notion_link ?? task.notion_link;
        task.assignee_id = dto.assignee_id ?? task.assignee_id;
        task.assigner_id = dto.assigner_id ?? task.assigner_id;
        task.updated_at = DateTime.UtcNow;

        if (oldStatus != task.status)
        {
            _db.Logs.Add(new Log
            {
                task_id = task.task_id,
                user_id = task.assignee_id,
                log_type = "Task Update",
                content = $"Status changed from '{oldStatus}' to '{task.status}'",
                severity = "INFO",
                created_at = DateTime.UtcNow
            });
        }

        await _db.SaveChangesAsync();
        return Ok(task);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Xoá task thành công" });
    }
}
