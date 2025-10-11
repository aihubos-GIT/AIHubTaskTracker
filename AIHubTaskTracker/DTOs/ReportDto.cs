namespace AIHubTaskTracker.DTOs
{
    public class ReportCreateDto
    {
        public int MemberId { get; set; }
        public string ReportTitle { get; set; }
        public string Summary { get; set; }
        public DateTime? CreatedAt { get; set; } 
        public int TasksCompleted { get; set; }
        public int TasksPending { get; set; }
        public int? TaskId { get; set; }
    }

    public class ReportUpdateDto
    {
        public string? ReportTitle { get; set; }
        public string? Summary { get; set; }
        public int? TasksCompleted { get; set; }
        public int? TasksPending { get; set; }
    }
}
