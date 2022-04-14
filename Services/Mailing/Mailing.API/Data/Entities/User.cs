namespace Mailing.API.Data.Entities;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public bool IsSubscribedForMailing { get; set; }
}