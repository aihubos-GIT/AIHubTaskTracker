namespace AIHubTaskTracker.DTOs
{
    public class TaskCreateDto
    {
        public string TaskTitle { get; set; }
        public string Status { get; set; }
        public DateTime Deadline { get; set; }
        public int MemberId { get; set; } 
    }

    public class TaskUpdateDto
    {
        public string? TaskTitle { get; set; }
        public string? Status { get; set; }
        public DateTime? Deadline { get; set; }
        public int? MemberId { get; set; }
    }
}
