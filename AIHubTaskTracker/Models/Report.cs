using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AIHubTaskTracker.Models
{  /// <summary>
   /// Bảng Report (KPI / Daily Reports)
   /// Lưu thông tin báo cáo tiến độ hàng ngày của từng thành viên.
   /// Dữ liệu bao gồm tổng số request API, số lỗi, KPI CRUD + Auth, 
   /// và tình trạng nộp báo cáo (đúng hạn hay trễ).
   /// Dùng để thống kê và đánh giá hiệu suất cá nhân / nhóm.
   /// </summary>
    public class Report
    {
        [Key]
        public int report_id { get; set; } //  Khóa chính

        [Required]
        public int reporter_id { get; set; } //  Người báo cáo (liên kết Member)

        [Required]
        public DateTime report_date { get; set; } //  Ngày báo cáo (hàng ngày)

        public int total_requests { get; set; } //  Tổng số API Request
        public int failed_requests { get; set; } //  Số lượng Request Lỗi

        public int kpi_crud_auth_perc { get; set; } //  KPI Hoàn thành CRUD + Auth (%)

        [MaxLength(50)]
        public string report_status { get; set; } = "On Time"; //  Trạng thái báo cáo (On Time / Late)

        public string? report_summary { get; set; } //  Nội dung / tổng kết / đề xuất cải tiến

        public DateTime created_at { get; set; } = DateTime.Now; //  Ngày tạo

        [JsonIgnore]
        public Member? reporter { get; set; } // Liên kết tới Member (Người báo cáo)
    }
}
