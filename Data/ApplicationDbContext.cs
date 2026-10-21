using CivicResolve.Models;
using Microsoft.EntityFrameworkCore;

namespace CivicResolve.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Complaint> Complaints => Set<Complaint>();
        public DbSet<ComplaintStatusHistory> ComplaintStatusHistories => Set<ComplaintStatusHistory>();
        public DbSet<CitizenFeedback> CitizenFeedbacks => Set<CitizenFeedback>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Indexes
            modelBuilder.Entity<Complaint>()
                .HasIndex(c => c.TrackingNumber)
                .IsUnique();

            modelBuilder.Entity<Complaint>()
                .HasIndex(c => c.CitizenPhone);

            modelBuilder.Entity<Complaint>()
                .HasIndex(c => c.Status);

            modelBuilder.Entity<Complaint>()
                .HasIndex(c => c.DepartmentId);

            // Relationships
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Categories)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Department)
                .WithMany(d => d.Complaints)
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Complaint>()
                .HasOne(c => c.Category)
                .WithMany(cat => cat.Complaints)
                .HasForeignKey(c => c.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ComplaintStatusHistory>()
                .HasOne(h => h.Complaint)
                .WithMany(c => c.StatusHistories)
                .HasForeignKey(h => h.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<CitizenFeedback>()
                .HasOne(f => f.Complaint)
                .WithMany(c => c.Feedbacks)
                .HasForeignKey(f => f.ComplaintId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
