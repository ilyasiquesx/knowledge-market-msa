using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Forum.Infrastructure.Data;

public class ContextFactory : IDesignTimeDbContextFactory<ForumContext>
{
    public ForumContext CreateDbContext(string[] args)
    {
        var builder =
            new DbContextOptionsBuilder<ForumContext>().UseNpgsql(
                "Server=localhost;Port=5432;Database=ForumStorage;User Id=postgres;Password=developer1995;");
        
        return new ForumContext(builder.Options);
    }
}