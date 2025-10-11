namespace AIHubTaskTracker.DTOs
{
    public class MemberCreateDto
    {
        public string Name { get; set; }
        public string Role { get; set; }
        public string Email { get; set; }
    }

    public class MemberUpdateDto
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? Email { get; set; }
    }
}
