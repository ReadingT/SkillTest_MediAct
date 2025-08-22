using Microsoft.EntityFrameworkCore;
using NurseScheduler.Api.Data.Models;

namespace NurseScheduler.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<ShiftAssignment> ShiftAssignments { get; set; }
    public DbSet<LeaveRequest> LeaveRequests { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // ตั้งค่า Unique constraint สำหรับ Email
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        // ตั้งค่าความสัมพันธ์ (Relationships)
        modelBuilder.Entity<ShiftAssignment>()
            .HasOne(sa => sa.User)
            .WithMany()
            .HasForeignKey(sa => sa.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ShiftAssignment>()
            .HasOne(sa => sa.Shift)
            .WithMany()
            .HasForeignKey(sa => sa.ShiftId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<LeaveRequest>()
            .HasOne(lr => lr.Approver)
            .WithMany()
            .HasForeignKey(lr => lr.ApprovedBy)
            .OnDelete(DeleteBehavior.SetNull); // ถ้า User ที่อนุมัติถูกลบ ให้ค่าเป็น NULL
    }
}