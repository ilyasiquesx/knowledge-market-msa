using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Authentication.API.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Authentication.API.Token;

public interface ITokenProvider
{
    string GetTokenForUser(User user);
}

public class DefaultTokenProvider : ITokenProvider
{
    private readonly JwtOptions _options;

    public DefaultTokenProvider(IOptions<JwtOptions> options)
    {
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public string GetTokenForUser(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

        var utcNow = DateTime.UtcNow;
        var expires = utcNow.AddMinutes(_options.LifetimeInMinutes);
        var credentials = new SigningCredentials(_options.GetSecurityKey(), SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            notBefore: utcNow,
            claims: claims,
            expires: expires,
            signingCredentials: credentials);
        
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
        return encodedJwt;
    }
}