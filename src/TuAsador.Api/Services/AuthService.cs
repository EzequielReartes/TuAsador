using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using TuAsador.Application.Common.Interfaces;
using TuAsador.Application.Features.Auth;
using TuAsador.Domain.Entities;

namespace TuAsador.Api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly IConfiguration _config;

    public AuthService(UserManager<User> userManager, IConfiguration config)
    {
        _userManager = userManager;
        _config = config;
    }

    public async Task<AuthResponse> LoginAsync(string email, string password)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null || !await _userManager.CheckPasswordAsync(user, password))
            throw new UnauthorizedAccessException("Email o contraseña incorrectos");

        if (!user.IsActive)
            throw new UnauthorizedAccessException("Usuario deshabilitado. Contacte al administrador.");

        var token = GenerateToken(user);
        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email!,
            Role = user.Role,
            ProfilePictureUrl = user.ProfilePictureData != null
                ? $"/api/profile-picture?userId={user.Id}"
                : user.ProfilePictureUrl
        };
    }

    public async Task<AuthResponse> RegisterAsync(string name, string email, string password, string? whatsApp, string role)
    {
        var existing = await _userManager.FindByEmailAsync(email);
        if (existing != null)
            throw new InvalidOperationException("El email ya está registrado");

        var user = new User
        {
            UserName = email.Split('@')[0],
            Email = email,
            Name = name,
            WhatsApp = whatsApp,
            Role = role
        };

        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join("; ", result.Errors.Select(e => e.Description));
            throw new InvalidOperationException(errors);
        }

        await _userManager.AddToRoleAsync(user, role);

        var token = GenerateToken(user);
        return new AuthResponse
        {
            Token = token,
            UserId = user.Id,
            Name = user.Name,
            Email = user.Email!,
            Role = user.Role,
            ProfilePictureUrl = user.ProfilePictureData != null
                ? $"/api/profile-picture?userId={user.Id}"
                : user.ProfilePictureUrl
        };
    }

    private string GenerateToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Email, user.Email!),
            new(ClaimTypes.Name, user.Name)
        };

        var roles = _userManager.GetRolesAsync(user).Result;
        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:ExpireDays"] ?? "7")),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
