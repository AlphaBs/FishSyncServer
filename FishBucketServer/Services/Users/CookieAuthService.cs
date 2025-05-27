using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace AlphabetUpdateServer.Services.Users;

public sealed class CookieOptions
{
    public static readonly string SectionName = "Cookie";
    
    public TimeSpan ExpireTimeSpan { get; set; }
}

public static class CookieAuthService
{
    public static ClaimsIdentity CreateClaimsIdentity(string username, IEnumerable<string> roles)
    {
        var claims = new List<Claim>();
        claims.Add(new Claim(ClaimTypes.Name, username));
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    }
    
    public static string? GetUsername(IEnumerable<Claim> claims)
    {
        return claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
    }

    public static IEnumerable<string> GetRoles(IEnumerable<Claim> claims)
    {
        return claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value);
    }
}