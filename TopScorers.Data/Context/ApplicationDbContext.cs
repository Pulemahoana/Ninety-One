using Microsoft.EntityFrameworkCore;
using TopScorers.Core.Models;

namespace TopScorers.Data.Context;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<TestScore> TestScores { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TestScore>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SecondName).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Score).IsRequired();
        });
    }
}