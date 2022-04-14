using Microsoft.EntityFrameworkCore;

namespace Notifications.API.Data;

public class ApplicationContext : DbContext
{
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Question> Questions { get; set; }
    
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
        
    }
}