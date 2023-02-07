using Microsoft.EntityFrameworkCore;
using SocleRHHBESSAIH.Models;

namespace SocleRHHBESSAIH.Data
{
    public class SocleRHHBContext : DbContext
    {
        public SocleRHHBContext(DbContextOptions<SocleRHHBContext> options) : base(options)
        {

        }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Expense>()
                .Property(d => d.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Expense>()
                .HasOne(d => d.User)
                .WithMany(u => u.Expenses)
                .HasForeignKey(d => d.UserId);

            modelBuilder.Entity<User>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
