namespace AIHubTaskTracker.Models
{
    public class KPI
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public Member Member { get; set; }

        public int TasksCompleted { get; set; }
        public double Efficiency { get; set; }
        public DateTime Month { get; set; } 

    }
}
