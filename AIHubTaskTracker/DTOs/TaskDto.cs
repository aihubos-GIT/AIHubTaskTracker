using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AIHubTaskTracker.DTOs
{
    public class TaskCreateDto
    {
        [Required, MaxLength(255)]
        public string title { get; set; } = null!;

        public string? description { get; set; }

        [Required]
        public int assigner_id { get; set; }

        [Required]
        public int assignee_id { get; set; }

        public List<int>? collaborators { get; set; } = new();

        public string? expected_output { get; set; }

        public DateTime? deadline { get; set; }

        public string status { get; set; } = "To Do";

        public int progress_percentage { get; set; } = 0;

        public string? notion_link { get; set; }
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
