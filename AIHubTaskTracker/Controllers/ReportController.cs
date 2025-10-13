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
    public async Task<ActionResult> GetAll()
    {
        var reports = await _db.Reports
                               .Include(r => r.reporter)
                               .OrderByDescending(r => r.created_at)
                               .ToListAsync();
        return Ok(reports);
    }

    // GET /reports/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Report>> Get(int id)
    {
        var report = await _db.Reports
                              .Include(r => r.reporter)
                              .FirstOrDefaultAsync(r => r.report_id == id);
        if (report == null)
            return NotFound(new { message = "Không tìm thấy báo cáo." });
        return Ok(report);
    }

    // POST /reports
    [HttpPost]
    public async Task<ActionResult<Report>> Create([FromBody] ReportCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var member = await _db.Members.FindAsync(dto.reporter_id);
        if (member == null)
            return BadRequest(new { message = "Người báo cáo không hợp lệ." });

        var report = new Report
        {
            reporter_id = dto.reporter_id,
            report_date = dto.report_date ?? DateTime.UtcNow,
            total_requests = dto.total_requests,
            failed_requests = dto.failed_requests,
            kpi_crud_auth_perc = dto.kpi_crud_auth_perc,
            report_status = dto.report_status ?? "On Time",
            report_summary = dto.report_summary,
            created_at = DateTime.UtcNow
        };

        _db.Reports.Add(report);
        await _db.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = report.report_id }, report);
    }

    // PUT /reports/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] ReportUpdateDto dto)
    {
        var report = await _db.Reports.FindAsync(id);
        if (report == null) return NotFound(new { message = "Không tìm thấy báo cáo." });

        report.total_requests = dto.total_requests ?? report.total_requests;
        report.failed_requests = dto.failed_requests ?? report.failed_requests;
        report.kpi_crud_auth_perc = dto.kpi_crud_auth_perc ?? report.kpi_crud_auth_perc;
        report.report_status = dto.report_status ?? report.report_status;
        report.report_summary = dto.report_summary ?? report.report_summary;

        await _db.SaveChangesAsync();
        return Ok(new { message = "Cập nhật báo cáo thành công.", report });
    }

    // DELETE /reports/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var report = await _db.Reports.FindAsync(id);
        if (report == null) return NotFound(new { message = "Không tìm thấy báo cáo." });

        _db.Reports.Remove(report);
        await _db.SaveChangesAsync();

        return Ok(new { message = "Xoá báo cáo thành công." });
    }
}
