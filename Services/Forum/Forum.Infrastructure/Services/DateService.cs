using Forum.Core.Services;

namespace Forum.Infrastructure.Services;

public class DateService : IDateService
{
    public DateTime Now => DateTime.UtcNow;
}