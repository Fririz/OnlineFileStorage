using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using IdentityService.Application.Contracts;
using IdentityService.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.Application;

public class JwtTokenWorker : IJwtTokenWorker
{
    private readonly ILogger<JwtTokenWorker> _logger;
    private readonly JwtSecurityTokenHandler _handler;
    private readonly TokenValidationParameters _validationParameters;
    private readonly SigningCredentials _signingCredentials;
    private readonly string? _issuer;
    private readonly string? _audience;

    public JwtTokenWorker(IConfiguration configuration, ILogger<JwtTokenWorker> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _handler = new JwtSecurityTokenHandler();

        string? jwtKey = configuration.GetValue<string>("Jwt:Key");
        _issuer = configuration.GetValue<string>("Jwt:Issuer");
        _audience = configuration.GetValue<string>("Jwt:Audience");

        if (string.IsNullOrWhiteSpace(jwtKey))
            throw new InvalidOperationException("Jwt:Key is not set");
        if (string.IsNullOrWhiteSpace(_issuer))
            throw new InvalidOperationException("Jwt:Issuer is not set");
        if (string.IsNullOrWhiteSpace(_audience))
            throw new InvalidOperationException("Jwt:Audience is not set");

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
        var securityKey = new SymmetricSecurityKey(keyBytes);

        _signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = securityKey,

            ValidateLifetime = true,

            ValidateIssuer = true,
            ValidIssuer = _issuer,

            ValidateAudience = true,
            ValidAudience = _audience,

            ClockSkew = TimeSpan.Zero
        };
    }

    public string GenerateToken(User user)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Role, user.RoleOfUser.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: _signingCredentials);

        return _handler.WriteToken(token);
    }

    public bool CheckToken(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return false;

        try
        {
            _handler.ValidateToken(token, _validationParameters, out SecurityToken _);
            return true;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "JWT validation error: {Message}", ex.Message);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error with jwt parsing");
            return false;
        }
    }
}
