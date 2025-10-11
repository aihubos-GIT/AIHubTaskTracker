using AIHubTaskTracker.Models;

public class Report
{
    public int Id { get; set; }

    public int MemberId { get; set; }
    public Member Member { get; set; }

    public string ReportTitle { get; set; } // thêm field title
    public string Summary { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public int? TaskId { get; set; } // cho phép không gắn Task
    public TaskItem Task { get; set; }

    public int TasksCompleted { get; set; } // thêm field
    public int TasksPending { get; set; }   // thêm field
}
