using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Notifications.API.Options;

public class JwtOptions
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public int LifetimeInMinutes { get; set; }
    public string SecurityKey { get; set; }

    public SymmetricSecurityKey GetSecurityKey()
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(SecurityKey));
    }
}