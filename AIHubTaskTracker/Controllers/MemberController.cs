using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("members")]
public class MembersController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ILogger<MembersController> _logger;

    public MembersController(AppDbContext db, ILogger<MembersController> logger)
    {
        _db = db;
        _logger = logger;
    }

    // GET /members
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Member>>> GetAll()
    {
        var members = await _db.Members
                               .Include(m => m.Tasks) // load tasks khi get
                               .OrderBy(m => m.Name)
                               .ToListAsync();
        return Ok(members);
    }

    // GET /members/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<Member>> Get(int id)
    {
        var member = await _db.Members
                              .Include(m => m.Tasks)
                              .FirstOrDefaultAsync(m => m.Id == id);
        if (member == null) return NotFound(new { message = "Member not found" });
        return Ok(member);
    }

    // POST /members
    [HttpPost]
    public async Task<ActionResult<Member>> Create([FromBody] MemberCreateDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var member = new Member
        {
            Name = dto.Name,
            Email = dto.Email,
            Role = dto.Role,
            Tasks = new List<TaskItem>() 
        };

        _db.Members.Add(member);
        await _db.SaveChangesAsync();
        return CreatedAtAction(nameof(Get), new { id = member.Id }, member);
    }

    // PUT /members/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] MemberUpdateDto dto)
    {
        var member = await _db.Members.FindAsync(id);
        if (member == null) return NotFound(new { message = "Member not found" });

        member.Name = dto.Name ?? member.Name;
        member.Email = dto.Email ?? member.Email;
        member.Role = dto.Role ?? member.Role;

        _db.Members.Update(member);
        await _db.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /members/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var member = await _db.Members.FindAsync(id);
        if (member == null) return NotFound(new { message = "Member not found" });

        _db.Members.Remove(member);
        await _db.SaveChangesAsync();
        return NoContent();
    }
}
