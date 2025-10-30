using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using AIHubTaskTracker.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/v1/tasks")]
public class TasksItemController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ClickUpService _clickUp;
    private readonly TelegramService _telegram;

    public TasksItemController(AppDbContext db, ClickUpService clickUp, TelegramService telegram)
    {
        _db = db;
        _clickUp = clickUp;
        _telegram = telegram;
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaskCreateDto dto)
    {
        try
        {
            string statusValue = dto.status ?? "TO DO";
            const string REQUIRED_TAG = "[AIHUB_BACKEND]";

            var task = new TaskItem
            {
                title = dto.title,
                description = dto.description,
                assigner_id = dto.assigner_id,
                assignee_id = dto.assignee_id,
                collaborators = dto.collaborators ?? new List<int>(),
                expected_output = dto.expected_output,
                deadline = dto.deadline,
                status = statusValue,
                progress_percentage = dto.progress_percentage,
                notion_link = dto.notion_link,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync(); // Lưu lần 1 để có ID

            var clickUpId = await _clickUp.CreateTaskAsync(task);
            if (!string.IsNullOrEmpty(clickUpId))
            {
                task.clickup_id = clickUpId;
                await _clickUp.AddTagToTaskAsync(clickUpId, REQUIRED_TAG);
                _db.Tasks.Update(task);
                await _db.SaveChangesAsync(); // Lưu lần 2: có clickUp_id
            }

            // Gửi log Telegram
            await _telegram.SendMessageAsync($" Task mới được tạo:\n*{task.title}*\nNgười giao: `{task.assigner_id}` → Người nhận: `{task.assignee_id}`");

            return Ok(task);
        }
        catch (Exception ex)
        {
            await _telegram.SendMessageAsync($" Lỗi khi tạo task: {ex.Message}");
            return StatusCode(500, new { message = "Tạo task thất bại", error = ex.Message });
        }
    }

    // =====================
    // UPDATE TASK
    // =====================
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] TaskUpdateDto dto)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var oldStatus = task.status;
        var oldProgress = task.progress_percentage;

        // Cập nhật dữ liệu
        task.title = dto.title ?? task.title;
        task.description = dto.description ?? task.description;
        task.status = dto.status ?? task.status;
        task.expected_output = dto.expected_output ?? task.expected_output;
        task.deadline = dto.deadline ?? task.deadline;
        task.progress_percentage = dto.progress_percentage ?? task.progress_percentage;
        task.notion_link = dto.notion_link ?? task.notion_link;
        task.updated_at = DateTime.UtcNow;

        // Đồng bộ với ClickUp nếu có
        if (!string.IsNullOrEmpty(task.clickup_id))
            await _clickUp.UpdateTaskAsync(task);

        await _db.SaveChangesAsync();

        // Gửi log Telegram
        if (oldStatus != task.status)
        {
            await _telegram.SendMessageAsync($" *{task.title}* đổi trạng thái: `{oldStatus}` → `{task.status}`");
        }
        else if (oldProgress != task.progress_percentage)
        {
            await _telegram.SendMessageAsync($" *{task.title}* cập nhật tiến độ: `{oldProgress}%` → `{task.progress_percentage}%`");
        }
        else
        {
            await _telegram.SendMessageAsync($" Task *{task.title}* vừa được cập nhật nội dung.");
        }

        // Nếu Completed thì gửi thông báo đặc biệt
        if (task.status == "Completed")
        {
            await _telegram.SendMessageAsync($" Hoàn thành – Task *{task.title}* đã done và đồng bộ ClickUp!");
        }

        return Ok(task);
    }

    // =====================
    // GET ALL TASKS
    // =====================
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? assignee_id)
    {
        var query = _db.Tasks.AsQueryable();

        if (assignee_id.HasValue && assignee_id.Value != 0)
            query = query.Where(t => t.assignee_id == assignee_id.Value);

        var tasks = await query.OrderByDescending(t => t.updated_at).ToListAsync();
        return Ok(tasks);
    }

    // =====================
    // GET TASK BY ID
    // =====================
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var task = await _db.Tasks.FirstOrDefaultAsync(t => t.task_id == id);
        if (task == null)
            return NotFound(new { message = $"Task ID {id} không tồn tại." });

        return Ok(task);
    }

    // =====================
    // DELETE TASK
    // =====================
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var clickUpIdToDelete = task.clickup_id;
        string title = task.title;

        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();

        if (!string.IsNullOrEmpty(clickUpIdToDelete))
            await _clickUp.DeleteTaskAsync(clickUpIdToDelete);

        // Gửi log Telegram
        await _telegram.SendMessageAsync($" Task *{title}* đã bị xóa.");

        return Ok(new { message = "Xoá task thành công" });
    }
}
