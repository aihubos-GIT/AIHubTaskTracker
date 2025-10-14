using AIHubTaskTracker.Data;
using AIHubTaskTracker.DTOs;
using AIHubTaskTracker.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using AIHubTaskTracker.Models.Enums;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;
    public AuthController(AppDbContext db, ILogger<AuthController> logger, IConfiguration config)
    {
        _db = db;
        _logger = logger;
        _config = config;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] MemberCreateDto dto)
    {
        if (await _db.Members.AnyAsync(x => x.email == dto.email))
            return Conflict(new { message = "Email đã tồn tại." });

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

        return Ok(new { message = "Đăng ký thành công", member_id = member.user_id });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        try
        {
            var member = await _db.Members.FirstOrDefaultAsync(m => m.email == dto.email);
            if (member == null || member.password_hash != HashPassword(dto.password))
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });

            var jwtKey = _config["Jwt:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                return StatusCode(500, new { message = "JWT Key missing in configuration!" });
            }

            var token = GenerateJwtToken(member, _config["Jwt:Key"], _config["Jwt:Issuer"], _config["Jwt:Audience"]);
            return Ok(new { token });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Login failed");
            return StatusCode(500, new { message = ex.Message });
        }
    }


    private string GenerateJwtToken(Member member, string key, string issuer, string audience)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: new[]
            {
            new Claim("id", member.user_id.ToString()),
            new Claim("email", member.email)
            },
            expires: DateTime.UtcNow.AddMinutes(60),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);
    }


    private static string HashPassword(string password)
    {
        using var sha = System.Security.Cryptography.SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }


    [Authorize]
    [HttpGet("/api/v1/users/profile")]
    public async Task<IActionResult> Profile()
    {
        var userId = int.Parse(User.FindFirstValue("id")!);
        var member = await _db.Members.FindAsync(userId);
        if (member == null) return NotFound();
        return Ok(member);
    }
    [Authorize]
    [HttpPut("/api/v1/users/profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] MemberUpdateDto dto)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue("id")!);
            var member = await _db.Members.FindAsync(userId);
            if (member == null)
                return NotFound(new { message = "Người dùng không tồn tại." });
            if (!string.IsNullOrEmpty(dto.full_name))
                member.full_name = dto.full_name;

            if (!string.IsNullOrEmpty(dto.email))
            {
                // Kiểm tra email trùng
                if (await _db.Members.AnyAsync(m => m.email == dto.email && m.user_id != userId))
                    return Conflict(new { message = "Email đã được sử dụng." });

                member.email = dto.email;
            }
            if (!string.IsNullOrEmpty(dto.password))
            {
                member.password_hash = HashPassword(dto.password);
            }
            member.role = dto.role;
            member.position = dto.position;
            member.updated_at = DateTime.UtcNow;

            _db.Members.Update(member);
            await _db.SaveChangesAsync();

            return Ok(new { message = "Cập nhật profile thành công", member });
        }
        catch (Exception ex)
        {
            _logger?.LogError(ex, "Update profile failed");
            return StatusCode(500, new { message = ex.Message });
        }
    }
    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        if (!string.IsNullOrEmpty(token))
        {
            var userId = int.Parse(User.FindFirstValue("id")!);

            // Logout tất cả token cũ của user
            var now = DateTime.UtcNow;
            var allTokens = await _db.BlacklistedTokens
                .Where(t => t.UserId == userId && t.ExpiresAt > now)
                .ToListAsync();

            foreach (var t in allTokens)
                t.ExpiresAt = now; 

            // Thêm token hiện tại vào blacklist
            _db.BlacklistedTokens.Add(new BlacklistedTokens
            {
                Token = token,
                UserId = userId,
                RevokedAt = now,
                ExpiresAt = now.AddMinutes(30)
            });

            await _db.SaveChangesAsync();
        }

        return Ok(new { message = "Đăng xuất thành công ! " });
    }


}
