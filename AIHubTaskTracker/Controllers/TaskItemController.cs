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
        
        // 1. Chuẩn hóa Status Value (Dùng TO DO nếu DTO không cung cấp)
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
            status = statusValue, // Sử dụng giá trị đã chuẩn hóa
            progress_percentage = dto.progress_percentage,
            notion_link = dto.notion_link,
            created_at = DateTime.UtcNow,
            updated_at = DateTime.UtcNow
        };

        _db.Tasks.Add(task);
        // LƯU LẦN 1: Lưu task để Entity Framework gán ID nội bộ (task.id)
        await _db.SaveChangesAsync();

        // 2. GỌI CLICKUP API VÀ LẤY ID TỪ PHẢN HỒI
        var clickUpId = await _clickUp.CreateTaskAsync(task);

        if (!string.IsNullOrEmpty(clickUpId))
        {
            // 3. CRITICAL FIX: LƯU ID CLICKUP VÀO DB
            task.clickup_id = clickUpId;
            await _clickUp.AddTagToTaskAsync(clickUpId, REQUIRED_TAG);
            _db.Tasks.Update(task);
            // LƯU LẦN 2: Commit ID ClickUp (để Update/Delete sau này hoạt động)
            await _db.SaveChangesAsync();
        }
        // ELSE: Nếu clickUpId là null, việc tích hợp ClickUp thất bại, nhưng task vẫn được lưu nội bộ.

        return Ok(task);
    }


    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TaskUpdateDto dto)
    {
        // 1. Tìm Task và kiểm tra
        var task = await _db.Tasks.FindAsync(id);
        if (task == null) return NotFound();

        var oldStatus = task.status;

        // 2. Cập nhật các trường (Nếu DTO cung cấp giá trị mới)
        task.title = dto.title ?? task.title;
        task.description = dto.description ?? task.description;
        task.status = dto.status ?? task.status;

        // Cập nhật các trường khác: deadline, output, progress, notion_link
        // Sử dụng toán tử ?? để chỉ cập nhật nếu DTO cung cấp giá trị KHÔNG phải null.
        task.expected_output = dto.expected_output ?? task.expected_output;
        task.deadline = dto.deadline ?? task.deadline; // Cập nhật cả giá trị null nếu cần
        task.progress_percentage = dto.progress_percentage ?? task.progress_percentage;
        task.notion_link = dto.notion_link ?? task.notion_link;

        task.updated_at = DateTime.UtcNow;

        // 3. Đồng bộ với ClickUp
        if (!string.IsNullOrEmpty(task.clickup_id))
        {
            // UpdateTaskAsync sẽ sử dụng task entity đã được cập nhật
            await _clickUp.UpdateTaskAsync(task);
        }

        // 4. Commit thay đổi vào Database
        await _db.SaveChangesAsync();

        // 5. Gửi thông báo Telegram (Logic hiện tại của bạn)
        if (oldStatus != "Completed" && task.status == "Completed")
        {
            await _telegram.SendMessageAsync($"✅ Done – Task '{task.title}' đã hoàn thành và đồng bộ ClickUp");
        }

        return Ok(task);
    }



    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] int? assignee_id) 
    {
        var query = _db.Tasks.AsQueryable();

        if (assignee_id.HasValue && assignee_id.Value != 0)
        {
            query = query.Where(t => t.assignee_id == assignee_id.Value);
        }
        return Ok(await query.ToListAsync());
    }



    // Trong TasksItemController.cs



    [HttpDelete("{id:int}")]

    public async Task<IActionResult> Delete(int id)

    {

        var task = await _db.Tasks.FindAsync(id);

        if (task == null) return NotFound();



        // LƯU LẠI CLICKUP ID TRƯỚC KHI XÓA KHỎI DB

        var clickUpIdToDelete = task.clickup_id;



        _db.Tasks.Remove(task);

        await _db.SaveChangesAsync();


        if (!string.IsNullOrEmpty(clickUpIdToDelete))

        {

            await _clickUp.DeleteTaskAsync(clickUpIdToDelete);

        }



        return Ok(new { message = "Xoá task thành công" });

    }

}