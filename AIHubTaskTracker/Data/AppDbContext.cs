using AIHubTaskTracker.Models;
using Microsoft.EntityFrameworkCore;

namespace AIHubTaskTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Member> Members { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<Log> Logs { get; set; }
        public DbSet<Report> Reports { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ---- Member ----
            modelBuilder.Entity<Member>()
                .HasIndex(m => m.email)
                .IsUnique(); // Không trùng email

            // ---- Task ----
            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.assigner)
                .WithMany(m => m.created_tasks)
                .HasForeignKey(t => t.assigner_id)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Member thì  xóa Task được giao bởi Member

            modelBuilder.Entity<TaskItem>()
                .HasOne(t => t.assignee)
                .WithMany(m => m.assigned_tasks)
                .HasForeignKey(t => t.assignee_id)
                .OnDelete(DeleteBehavior.Cascade); // Xóa Member thì xóa Task được giao cho Member

            // ---- Log ----
            modelBuilder.Entity<Log>()
                .HasOne(l => l.user)
                .WithMany(m => m.logs)
                .HasForeignKey(l => l.user_id)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Log>()
                .HasOne(l => l.task)
                .WithMany(t => t.logs)
                .HasForeignKey(l => l.task_id)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Report ----
            modelBuilder.Entity<Report>()
                .HasOne(r => r.reporter)
                .WithMany(m => m.reports)
                .HasForeignKey(r => r.reporter_id)
                .OnDelete(DeleteBehavior.Cascade);

            // ---- Datetime fields ----
            modelBuilder.Entity<TaskItem>()
                .Property(t => t.created_at)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<TaskItem>()
                .Property(t => t.updated_at)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Report>()
                .Property(r => r.created_at)
                .HasColumnType("timestamp with time zone");

            modelBuilder.Entity<Log>()
                .Property(l => l.created_at)
                .HasColumnType("timestamp with time zone");
        }
    }
}
