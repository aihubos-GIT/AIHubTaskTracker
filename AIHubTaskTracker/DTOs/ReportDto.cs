using System;
using System.ComponentModel.DataAnnotations;

namespace AIHubTaskTracker.DTOs
{
    public class ReportCreateDto
    {
        [Required]
        public int reporter_id { get; set; }

        public DateTime? report_date { get; set; } 

        public int total_requests { get; set; }
        public int failed_requests { get; set; }
        public int kpi_crud_auth_perc { get; set; }

        [MaxLength(50)]
        public string? report_status { get; set; } = "On Time";

        public string? report_summary { get; set; }
    }

    public class ReportUpdateDto
    {
        public int? total_requests { get; set; }
        public int? failed_requests { get; set; }
        public int? kpi_crud_auth_perc { get; set; }
        [MaxLength(50)]
        public string? report_status { get; set; }
        public string? report_summary { get; set; }
    }
}
