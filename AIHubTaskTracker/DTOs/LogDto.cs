using System;
using System.ComponentModel.DataAnnotations;

namespace AIHubTaskTracker.DTOs
{
    public class LogCreateDto
    {
        [Required]
        public int user_id { get; set; }

        public int? task_id { get; set; }

        [Required]
        [MaxLength(100)]
        public string log_type { get; set; } = string.Empty;

        [Required]
        public string content { get; set; } = string.Empty;

        [MaxLength(50)]
        public string severity { get; set; } = "INFO";
    }

    public class LogUpdateDto
    {
        [MaxLength(100)]
        public string? log_type { get; set; }

        public string? content { get; set; }

        [MaxLength(50)]
        public string? severity { get; set; }
    }
}
