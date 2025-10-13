using AIHubTaskTracker.Models;

namespace AIHubTaskTracker.Services
{
    public static class TaskLogger
    {
        private static readonly string logPath = "task_log.txt";

        public static void Log(string action, TaskItem task)
        {
            var status = task.status ?? "Unknown";
            var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {action}: Task {task.task_id} - {task.title} ({status})";
            File.AppendAllText(logPath, text + Environment.NewLine);
        }

        // Nếu muốn async
        public static async Task LogAsync(string action, TaskItem task)
        {
            var status = task.status ?? "Unknown";
            var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {action}: Task {task.task_id} - {task.title} ({status})";
            await File.AppendAllTextAsync(logPath, text + Environment.NewLine);
        }
    }
}
