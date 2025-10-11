using AIHubTaskTracker.Data;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AIHubTaskTracker.Controllers
{
    [ApiController]
    [Route("tasks")]
    public class TasksController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<TasksController> _logger;

        public TasksController(AppDbContext db, ILogger<TasksController> logger)
        {
            _db = db;
            _logger = logger;
        }

        //  GET /tasks
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TaskItem>>> GetAll()
        {
            var tasks = await _db.Tasks.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return Ok(tasks);
        }

        //  GET /tasks/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<TaskItem>> Get(int id)
        {
            var item = await _db.Tasks.FindAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        //  POST /tasks
        [HttpPost]
        public async Task<ActionResult<TaskItem>> Create([FromBody] TaskItem dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var task = new TaskItem
            {
                MemberName = dto.MemberName,
                TaskTitle = dto.TaskTitle,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? "Pending" : dto.Status,
                Deadline = dto.Deadline,
                CreatedAt = DateTime.UtcNow
            };

            _db.Tasks.Add(task);
            await _db.SaveChangesAsync();

            return CreatedAtAction(nameof(Get), new { id = task.Id }, task);
        }

        //  PUT /tasks/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] TaskItem dto)
        {
            var item = await _db.Tasks.FindAsync(id);
            if (item == null) return NotFound();

            item.Status = dto.Status ?? item.Status;
            item.TaskTitle = dto.TaskTitle ?? item.TaskTitle;
            item.MemberName = dto.MemberName ?? item.MemberName;
            item.Deadline = dto.Deadline != default ? dto.Deadline : item.Deadline;

            _db.Tasks.Update(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }

        // DELETE /tasks/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var item = await _db.Tasks.FindAsync(id);
            if (item == null) return NotFound();

            _db.Tasks.Remove(item);
            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}
