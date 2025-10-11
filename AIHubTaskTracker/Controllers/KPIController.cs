using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("kpis")]
public class KPIController : ControllerBase
{
    private readonly AppDbContext _db;

    public KPIController(AppDbContext db)
    {
        _db = db;
    }

    // GET /kpis
    [HttpGet]
    public async Task<ActionResult<IEnumerable<KPI>>> GetAll()
    {
        var kpis = await _db.Kpis.Include(k => k.Member)
                                 .OrderBy(k => k.Month)
                                 .ToListAsync();
        return Ok(kpis);
    }

    // GET /kpis/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<KPI>> Get(int id)
    {
        var kpi = await _db.Kpis.Include(k => k.Member)
                                .FirstOrDefaultAsync(k => k.Id == id);
        if (kpi == null) return NotFound();
        return Ok(kpi);
    }

    // POST /kpis
    [HttpPost]
    public async Task<ActionResult<KPI>> Create([FromBody] KPICreateDto dto)
    {
        var kpi = new KPI
        {
            MemberId = dto.MemberId,
            TasksCompleted = dto.TasksCompleted,
            Efficiency = dto.Efficiency,
            Month = dto.Month
        };

        _db.Kpis.Add(kpi);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = kpi.Id }, kpi);
    }

    // PUT /kpis/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] KPIUpdateDto dto)
    {
        var kpi = await _db.Kpis.FindAsync(id);
        if (kpi == null) return NotFound();

        kpi.TasksCompleted = dto.TasksCompleted ?? kpi.TasksCompleted;
        kpi.Efficiency = dto.Efficiency ?? kpi.Efficiency;
        kpi.Month = dto.Month ?? kpi.Month;

        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /kpis/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var kpi = await _db.Kpis.FindAsync(id);
        if (kpi == null) return NotFound();

        _db.Kpis.Remove(kpi);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
