using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AIHubTaskTracker.Models
{
    /// <summary>
    /// Bảng Log (System Activity Log)
    /// Lưu lại các hành động, lỗi hệ thống, hoặc cập nhật task của người dùng.
    /// Dữ liệu này dùng để thống kê & gửi báo cáo tự động lúc 22h mỗi ngày.
    /// </summary>
    public class Log
    {
        [Key]
        public int log_id { get; set; } // Khóa chính 

        [Required]
        public int user_id { get; set; } // Người dùng đã tạo/thực hiện hành động
        [ForeignKey(nameof(user_id))]
        [JsonIgnore]
        public Member? user { get; set; } // Navigation đến bảng Member

        public int? task_id { get; set; } // Liên kết đến công việc cụ thể nếu log liên quan Task
        [ForeignKey(nameof(task_id))]
        [JsonIgnore]
        public TaskItem? task { get; set; } // Navigation đến bảng TaskItem

        [Required, MaxLength(100)]
        public string log_type { get; set; } = null!; // Loại Log (VD: Task Update, System Error, Login Success)

        [Required]
        public string content { get; set; } = null!; // Nội dung chi tiết hành động hoặc lỗi

        [Required, MaxLength(50)]
        public string severity { get; set; } = "INFO"; // Mức độ ưu tiên (VD: INFO, WARNING, ERROR)

        public DateTime created_at { get; set; } = DateTime.Now; // Thời điểm tạo log (dùng để gửi báo cáo tự động)
    }
}
