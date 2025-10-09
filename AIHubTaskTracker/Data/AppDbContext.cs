using AIHubTaskTracker.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;


namespace AIHubTaskTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<TaskItem> Tasks { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<TaskItem>().Property(t => t.CreatedAt).HasDefaultValueSql("getutcdate()");

            modelBuilder.Entity<TaskItem>().HasData(
            new TaskItem { Id = 1, MemberName = "Nguyễn Thành Tuấn", TaskTitle = "Tích hợp API Dashboard", Status = "In Progress", Deadline = DateTime.Parse("2025-10-15"), CreatedAt = DateTime.Parse("2025-10-10") }
            );
        }
    }
}