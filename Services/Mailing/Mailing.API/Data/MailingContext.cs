using Mailing.API.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Mailing.API.Data;

public class MailingContext : DbContext
{
    public DbSet<InboxMessage> InboxMessages { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Question> Questions { get; set; }

    public MailingContext(DbContextOptions<MailingContext> opt) : base(opt)
    {
    }

    public async Task<InboxMessage> GetOldestMessage()
    {
        return await InboxMessages.OrderBy(m => m.CreatedAt).FirstOrDefaultAsync();
    }
}