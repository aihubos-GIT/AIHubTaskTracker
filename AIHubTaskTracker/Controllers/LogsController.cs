using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("logs")]
public class LogsController : ControllerBase
{
    private readonly AppDbContext _db;

    public LogsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /logs
    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var logs = await _db.Logs
                            .Include(l => l.user)
                            .Include(l => l.task)
                            .OrderByDescending(l => l.created_at)
                            .ToListAsync();
        return Ok(logs);
    }

    // GET /logs/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Log>> Get(int id)
    {
        var log = await _db.Logs
                           .Include(l => l.user)
                           .Include(l => l.task)
                           .FirstOrDefaultAsync(l => l.log_id == id);

        if (log == null) return NotFound(new { message = "Không tìm thấy log." });
        return Ok(log);
    }

    // POST /logs
    [HttpPost]
    public async Task<ActionResult<Log>> Create([FromBody] LogCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var user = await _db.Members.FindAsync(dto.user_id);
        if (user == null)
            return BadRequest(new { message = "Người dùng không hợp lệ." });

        var log = new Log
        {
            user_id = dto.user_id,
            task_id = dto.task_id,
            log_type = dto.log_type,
            content = dto.content,
            severity = dto.severity ?? "INFO",
            created_at = DateTime.UtcNow
        };

        _db.Logs.Add(log);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = log.log_id }, log);
    }

    // PUT /logs/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] LogUpdateDto dto)
    {
        var log = await _db.Logs.FindAsync(id);
        if (log == null) return NotFound(new { message = "Không tìm thấy log." });

        log.log_type = dto.log_type ?? log.log_type;
        log.content = dto.content ?? log.content;
        log.severity = dto.severity ?? log.severity;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Cập nhật log thành công.", log });
    }

    // DELETE /logs/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var log = await _db.Logs.FindAsync(id);
        if (log == null) return NotFound(new { message = "Không tìm thấy log." });

        _db.Logs.Remove(log);
        await _db.SaveChangesAsync();
        return Ok(new { message = "Xoá log thành công." });
    }
}
