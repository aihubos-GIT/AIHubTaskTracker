using System;
using System.ComponentModel.DataAnnotations;

namespace AIHubTaskTracker.DTOs
{
    public class ReportCreateDto
    {
        [Required]
        public int reporter_id { get; set; }

        public DateTime report_date { get; set; } = DateTime.UtcNow;

        [Range(0, int.MaxValue)]
        public int total_requests { get; set; } = 0;

        [Range(0, int.MaxValue)]
        public int failed_requests { get; set; } = 0;

        [Range(0, 100)]
        public int kpi_crud_auth_perc { get; set; } = 0;

        [MaxLength(50)]
        public string report_status { get; set; } = "On Time";

        public string? report_summary { get; set; }
    }

    // DTO cập nhật báo cáo
    public class ReportUpdateDto
    {
        [Range(0, int.MaxValue)]
        public int? total_requests { get; set; }

        [Range(0, int.MaxValue)]
        public int? failed_requests { get; set; }

        [Range(0, 100)]
        public int? kpi_crud_auth_perc { get; set; }

        [MaxLength(50)]
        public string? report_status { get; set; }

        public string? report_summary { get; set; }
    }
}
