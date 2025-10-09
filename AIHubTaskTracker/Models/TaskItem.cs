using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace AIHubTaskTracker.Models
{
    public class TaskItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        [MaxLength(200)]
        public string MemberName { get; set; } = string.Empty;


        [Required]
        [MaxLength(500)]
        public string TaskTitle { get; set; } = string.Empty;


        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = "Pending";


        public DateTime? Deadline { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}