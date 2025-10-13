using System.ComponentModel.DataAnnotations;
using AIHubTaskTracker.Models.Enums;

namespace AIHubTaskTracker.DTOs
{
    public class MemberCreateDto
    {
        [Required(ErrorMessage = "Tên đầy đủ không được để trống.")]
        [MaxLength(255)]
        public string full_name { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống.")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [MaxLength(255)]
        public string email { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string password { get; set; } = null!;

        [Required(ErrorMessage = "Vai trò không được để trống.")]
        public RoleType role { get; set; }

        [Required(ErrorMessage = "Vị trí không được để trống.")]
        [MaxLength(100)]
        public string position { get; set; } = null!;
    }
    public class MemberUpdateDto
    {
        [MaxLength(255)]
        public string? full_name { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ.")]
        [MaxLength(255)]
        public string? email { get; set; }

        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự.")]
        public string? password { get; set; }

        public RoleType? role { get; set; }

        [MaxLength(100)]
        public string? position { get; set; }
    }
}
