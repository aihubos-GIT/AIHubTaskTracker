namespace AIHubTaskTracker.DTOs
{
    public class KPICreateDto
    {
        public int MemberId { get; set; }
        public int TasksCompleted { get; set; }
        public double Efficiency { get; set; }
        public DateTime Month { get; set; }
    }

    public class KPIUpdateDto
    {
        public int? TasksCompleted { get; set; }
        public double? Efficiency { get; set; }
        public DateTime? Month { get; set; }
    }
}
