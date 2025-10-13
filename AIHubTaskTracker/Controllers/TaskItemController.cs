using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("tasks")]
public class TaskItemController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<TaskItemController> _logger;

    public TaskItemController(AppDbContext db, ILogger<TaskItemController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /tasks?page=1&pageSize=10
    [HttpGet]
    public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        if (page < 1 || pageSize <= 0)
            return BadRequest(new { message = "Tham số phân trang không hợp lệ." });

        var total = await _db.Tasks.CountAsync();
        var tasks = await _db.Tasks
                             .Include(t => t.assignee)
                             .Include(t => t.assigner)
                             .OrderBy(t => t.deadline)
                             .Skip((page - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync();

        return Ok(new
        {
            total,
            page,
            pageSize,
            data = tasks
        });
    }

    // GET /tasks/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<TaskItem>> Get(int id)
    {
        var task = await _db.Tasks
                            .Include(t => t.assignee)
                            .Include(t => t.assigner)
                            .FirstOrDefaultAsync(t => t.task_id == id);

        if (task == null)
            return NotFound(new { message = "Không tìm thấy task." });

        return Ok(task);
    }

    // POST /tasks
    [HttpPost]
    public async Task<ActionResult<TaskItem>> Create([FromBody] TaskCreateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var assigner = await _db.Members.FindAsync(dto.assigner_id);
        var assignee = await _db.Members.FindAsync(dto.assignee_id);
        if (assigner == null || assignee == null)
            return BadRequest(new { message = "Người giao hoặc người thực hiện không hợp lệ." });

        var task = new TaskItem
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

        // Tạo log tạo task
        var log = new Log
        {
            task_id = task.task_id,
            user_id = task.assignee_id,
            log_type = "Task Created",
            content = $"Task '{task.title}' created and assigned to {assignee.full_name}",
            severity = "INFO",
            created_at = DateTime.UtcNow
        };
        _db.Logs.Add(log);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = task.task_id }, task);
    }

    // PUT /tasks/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var task = await _db.Tasks.FindAsync(id);
        if (task == null)
            return NotFound(new { message = "Không tìm thấy task." });

        var oldStatus = task.status;

        task.title = dto.title ?? task.title;
        task.description = dto.description ?? task.description;
        task.assigner_id = dto.assigner_id ?? task.assigner_id;
        task.assignee_id = dto.assignee_id ?? task.assignee_id;
        task.collaborators = dto.collaborators ?? task.collaborators;
        task.expected_output = dto.expected_output ?? task.expected_output;
        task.deadline = dto.deadline ?? task.deadline;
        task.status = dto.status ?? task.status;
        task.progress_percentage = dto.progress_percentage ?? task.progress_percentage;
        task.notion_link = dto.notion_link ?? task.notion_link;
        task.updated_at = DateTime.UtcNow;

        _db.Tasks.Update(task);

        // Tạo log nếu status thay đổi
        if (oldStatus != task.status)
        {
            var log = new Log
            {
                task_id = task.task_id,
                user_id = task.assignee_id,
                log_type = "Task Update",
                content = $"Status changed from '{oldStatus}' to '{task.status}'",
                severity = "INFO",
                created_at = DateTime.UtcNow
            };
            _db.Logs.Add(log);
        }

        await _db.SaveChangesAsync();

        return Ok(new { message = "Cập nhật task thành công.", task });
    }

    // DELETE /tasks/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null)
            return NotFound(new { message = "Không tìm thấy task." });

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Xoá task thành công." });
    }
}
