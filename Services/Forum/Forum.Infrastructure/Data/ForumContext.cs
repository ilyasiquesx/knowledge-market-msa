using Forum.Core.Entities.Answers;
using Forum.Core.Entities.Questions;
using Forum.Core.Entities.Users;
using Forum.Core.Services;
using Microsoft.EntityFrameworkCore;

namespace Forum.Infrastructure.Data;

public class ForumContext : DbContext, IDomainContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Answer> Answers { get; set; }

    public ForumContext(DbContextOptions<ForumContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Answer>()
            .HasOne(a => a.Question)
            .WithMany(q => q.Answers)
            .HasForeignKey(a => a.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Question>()
            .HasMany(q => q.Answers)
            .WithOne(a => a.Question)
            .HasForeignKey(a => a.QuestionId);

        modelBuilder.Entity<Question>()
            .HasOne<Answer>(q => q.BestAnswer)
            .WithOne()
            .HasForeignKey<Question>(q => q.BestAnswerId)
            .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }
}