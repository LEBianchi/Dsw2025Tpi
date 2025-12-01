using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Logging;

namespace Dsw2025Tpi.Application.Services
{
    public class JwtTokenService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<JwtTokenService> _logger;
        public JwtTokenService(IConfiguration config, ILogger<JwtTokenService> logger)
        {
            _config = config;
            _logger = logger;
        }

        public string GenerateToken(string username, string role, string customerId)
        {
            _logger.LogInformation("Generando token JWT para el usuario: {Username} con rol: {role}", username, role);

            var jwtConfig = _config.GetSection("Jwt");
            var keyText = jwtConfig["Key"] ?? throw new ArgumentNullException("Jwt Key");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyText));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, role),
            new Claim("customerId", customerId)
        };

            var token = new JwtSecurityToken(
                issuer: jwtConfig["Issuer"],
                audience: jwtConfig["Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(double.Parse(jwtConfig["ExpireInMinutes"] ?? "60")),
                signingCredentials: creds
                );

            _logger.LogInformation("Token generado con exito para el usuario: {username}", username);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
