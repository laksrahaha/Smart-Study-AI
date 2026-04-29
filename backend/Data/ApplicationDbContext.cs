using Microsoft.EntityFrameworkCore;
using SmartStudyAI.Backend.Models;

namespace SmartStudyAI.Backend.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Course> Courses { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Optional: configure relationships, indexes, etc.
            modelBuilder.Entity<Course>()
                .HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId);
        }
    }
}