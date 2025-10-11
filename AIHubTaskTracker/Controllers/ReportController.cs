using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("reports")]
public class ReportsController : ControllerBase
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db)
    {
        _db = db;
    }

    // GET /reports
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Report>>> GetAll()
    {
        var reports = await _db.Reports.Include(r => r.Member)
                                       .OrderByDescending(r => r.CreatedAt)
                                       .ToListAsync();
        return Ok(reports);
    }

    // GET /reports/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Report>> Get(int id)
    {
        var report = await _db.Reports.Include(r => r.Member)
                                      .FirstOrDefaultAsync(r => r.Id == id);
        if (report == null) return NotFound();
        return Ok(report);
    }

    // POST /reports
    [HttpPost]
    public async Task<ActionResult<Report>> Create([FromBody] ReportCreateDto dto)
    {
        var report = new Report
        {
            MemberId = dto.MemberId,
            ReportTitle = dto.ReportTitle,
            Summary = dto.Summary,
            CreatedAt = dto.CreatedAt ?? DateTime.UtcNow,
            TasksCompleted = dto.TasksCompleted,
            TasksPending = dto.TasksPending,
            TaskId = dto.TaskId 
        };

        _db.Reports.Add(report);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = report.Id }, report);
    }

    // PUT /reports/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReportUpdateDto dto)
    {
        var report = await _db.Reports.FindAsync(id);
        if (report == null) return NotFound();

        report.Summary = dto.Summary ?? report.Summary;
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /reports/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var report = await _db.Reports.FindAsync(id);
        if (report == null) return NotFound();

        _db.Reports.Remove(report);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
