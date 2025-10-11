using System.Text.Json.Serialization;

namespace AIHubTaskTracker.Models
{
    public class Member
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; } // "Developer", "Manager", ...
        public string Email { get; set; }
        [JsonIgnore]
        public ICollection<TaskItem> Tasks { get; set; }
    }
}
