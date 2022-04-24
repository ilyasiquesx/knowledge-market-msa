using Forum.Core.Entities.Answers;
using Forum.Core.Entities.Questions;
using Forum.Core.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Forum.Core.Services;

public interface IDomainContext
{
    public DbSet<User> Users { get; }
    public DbSet<Question> Questions { get; }
    public DbSet<Answer> Answers { get; }
    public Task<int> SaveChangesAsync(CancellationToken token);
}