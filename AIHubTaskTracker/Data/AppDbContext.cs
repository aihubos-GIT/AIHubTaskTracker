using AIHubTaskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AIHubTaskTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<KPI> Kpis { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<TaskLog> TaskLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.CreatedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.Deadline)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Report>()
                .Property(r => r.CreatedAt)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<KPI>()
                .Property(k => k.Month)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<TaskLog>()
                .Property(tl => tl.ChangedAt)
                .HasColumnType("timestamp with time zone");
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Tìm các TaskItem bị modify
            var modifiedTasks = ChangeTracker.Entries<TaskItem>()
                                             .Where(e => e.State == EntityState.Modified);

            foreach (var entry in modifiedTasks)
            {
                var oldStatus = entry.OriginalValues.GetValue<string>("Status");
                var newStatus = entry.CurrentValues.GetValue<string>("Status");

                // Nếu status thay đổi thì tạo log
                if (oldStatus != newStatus)
                {
                    TaskLogs.Add(new TaskLog
                    {
                        TaskId = entry.Entity.Id,
                        MemberId = entry.Entity.MemberId, // Member chịu trách nhiệm task
                        OldStatus = oldStatus,
                        NewStatus = newStatus,
                        ChangedAt = DateTime.UtcNow
                    });
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
