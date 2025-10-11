using AIHubTaskTracker.Models;

namespace AIHubTaskTracker.Services
{
    public static class TaskLogger
    {
        private static readonly string logPath = "task_log.txt";

        public static void Log(string action, TaskItem task)
        {
            var text = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {action}: Task {task.Id} - {task.TaskTitle} ({task.Status})";
            File.AppendAllText(logPath, text + Environment.NewLine);
        }
    }
}
