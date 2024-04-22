using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace AlphabetUpdateServer.Services;

public sealed class JwtOptions
{
    public static readonly string SectionName = "Jwt";

    [Required]
    public required string Key { get; init; }

    [Required]
    public required string Issuer { get; init; }

    [Required]
    public required string Audience { get; init; }

    [Required]
    public required int ExpiresInSecond { get; init; }
}

public class JwtAuthService
{
    public const string RoleClaimName = "roles";
    public const string SchemeName = JwtBearerDefaults.AuthenticationScheme;

    private readonly IOptions<JwtOptions> _options;
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public JwtAuthService(IOptions<JwtOptions> options)
    {
        _options = options;
    }

    public static SymmetricSecurityKey CreateSecurityKey(string key)
    {
        return new SymmetricSecurityKey(Convert.FromHexString(key));
    }

    public string GenerateJwt(string username, string role)
    {
        var securityKey = CreateSecurityKey(_options.Value.Key);
        var token = new JwtSecurityToken(
            issuer: _options.Value.Issuer, 
            audience: _options.Value.Audience,
            claims: 
            [
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(RoleClaimName, role),
            ],
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddSeconds(_options.Value.ExpiresInSecond),
            signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature));

        return _tokenHandler.WriteToken(token);
    }
}
