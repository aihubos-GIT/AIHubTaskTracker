using AIHubTaskTracker.Data;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIHubTaskTracker.Controllers.Api
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class MembersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MembersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/v1/members
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var members = await _context.Members
                .Select(m => new
                {
                    id = m.user_id,      
                    name = m.full_name, 
                    email = m.email,
                    role = m.role,
                    position = m.position,
                    avatar_url = m.avatar_url,// default
                    status = m.status // mặc định online
                })
                .ToListAsync();

            return Ok(members);
        }


    }
}
