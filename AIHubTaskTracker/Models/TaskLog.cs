namespace AIHubTaskTracker.Models
{
    public class TaskLog
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public TaskItem Task { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public DateTime ChangedAt { get; set; }
    }
}
