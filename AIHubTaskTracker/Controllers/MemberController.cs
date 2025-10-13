using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace AIHubTaskTracker.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<MembersController> _logger;

        public MembersController(AppDbContext db, ILogger<MembersController> logger)
        {
            _db = db;
            _logger = logger;
        }

        //  GET: api/members?page=1&pageSize=10
        [HttpGet]
        public async Task<ActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize <= 0)
                return BadRequest(new { message = "Tham số phân trang không hợp lệ." });

            var total = await _db.Members.CountAsync();
            var members = await _db.Members
                                   .OrderBy(m => m.full_name)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .Select(m => new
                                   {
                                       m.user_id,
                                       m.full_name,
                                       m.email,
                                       role = m.role.ToString(),
                                       m.position,
                                       m.created_at,
                                       m.updated_at
                                   })
                                   .ToListAsync();

            return Ok(new
            {
                total,
                page,
                pageSize,
                data = members
            });
        }

        // GET: api/members/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Member>> GetById(int id)
        {
            var member = await _db.Members
                                  .Include(m => m.assigned_tasks)
                                  .Include(m => m.created_tasks)
                                  .FirstOrDefaultAsync(m => m.user_id == id);

            if (member == null)
                return NotFound(new { message = "Không tìm thấy thành viên." });

            return Ok(member);
        }
        // POST: api/members
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] MemberCreateDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (await _db.Members.AnyAsync(x => x.email == dto.email))
                return Conflict(new { message = "Email đã tồn tại trong hệ thống." });

            var member = new Member
            {
                full_name = dto.full_name,
                email = dto.email,
                password_hash = HashPassword(dto.password),
                role = dto.role,
                position = dto.position,
                created_at = DateTime.UtcNow,
                updated_at = DateTime.UtcNow
            };

            _db.Members.Add(member);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = member.user_id }, member);
        }

        // PUT: api/members/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] MemberUpdateDto dto)
        {
            var member = await _db.Members.FindAsync(id);
            if (member == null)
                return NotFound(new { message = "Không tìm thấy thành viên." });

            if (!string.IsNullOrEmpty(dto.email))
            {
                var exists = await _db.Members.AnyAsync(x => x.email == dto.email && x.user_id != id);
                if (exists)
                    return Conflict(new { message = "Email đã tồn tại trong hệ thống." });
            }

            member.full_name = dto.full_name ?? member.full_name;
            member.email = dto.email ?? member.email;
            member.position = dto.position ?? member.position;
            member.role = dto.role ?? member.role;

            if (!string.IsNullOrEmpty(dto.password))
                member.password_hash = HashPassword(dto.password);

            member.updated_at = DateTime.UtcNow;

            _db.Members.Update(member);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Cập nhật thành công.", member });
        }

        // DELETE: api/members/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var member = await _db.Members
                                  .Include(m => m.assigned_tasks)
                                  .Include(m => m.created_tasks)
                                  .FirstOrDefaultAsync(m => m.user_id == id);

            if (member == null)
                return NotFound(new { message = "Không tìm thấy thành viên." });

            _db.Members.Remove(member);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Xoá thành viên thành công." });
        }
        //  Hash mật khẩu bằng SHA256
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
}
