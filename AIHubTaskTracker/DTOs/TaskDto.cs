using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AIHubTaskTracker.Models.Enums;

namespace AIHubTaskTracker.DTOs
{
    public class TaskCreateDto
    {
        [Required, MaxLength(255)]
        public string title { get; set; } = null!; // Tên công việc

        public string? description { get; set; } // Mô tả chi tiết

        [Required]
        public int assigner_id { get; set; } // Người giao

        [Required]
        public int assignee_id { get; set; } // Người thực hiện

        public List<int>? collaborators { get; set; } = new(); // Người phối hợp

        public string? expected_output { get; set; } // Kết quả mong đợi

        public DateTime? deadline { get; set; } // Deadline

        public string status { get; set; } = "To Do"; // Trạng thái: To Do, In Progress, Done, Overdue

        public int progress_percentage { get; set; } = 0; // Tỷ lệ hoàn thành

        public string? notion_link { get; set; } // Liên kết Notion
    }

    public class TaskUpdateDto
    {
        [MaxLength(255)]
        public string? title { get; set; }

        public string? description { get; set; }

        public int? assigner_id { get; set; }

        public int? assignee_id { get; set; }

        public List<int>? collaborators { get; set; }

        public string? expected_output { get; set; }

        public DateTime? deadline { get; set; }

        public string? status { get; set; }

        public int? progress_percentage { get; set; }

        public string? notion_link { get; set; }
    }
}
