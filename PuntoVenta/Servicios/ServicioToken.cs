using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using PuntoVenta.Models;

namespace PuntoVenta.Servicios

{
    public class ServicioToken
    {
        private readonly string _secretKey;
        private readonly int _expirationMinutes;
        private readonly List<ListaNegra> _blacklistedTokens;

        public ServicioToken(IConfiguration configuration)
        {
            _secretKey = configuration["Jwt:SecretKey"];
            _expirationMinutes = int.Parse(configuration["Jwt:ExpirationMinutes"]);
            _blacklistedTokens = new List<ListaNegra>();
        }

        public string GenerateJwtToken(string userId, string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, userId), new Claim(ClaimTypes.Name, username) }),
                Expires = DateTime.UtcNow.AddMinutes(_expirationMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);

            // Agregar token a la lista negra
            _blacklistedTokens.Add(new ListaNegra
            {
                Token = tokenString,
                ExpireAt = tokenDescriptor.Expires.Value
            });

            return tokenString;
        }
        public bool IsTokenBlacklisted(string token)
        {
            return _blacklistedTokens.Any(t => t.Token == token && t.ExpireAt > DateTime.UtcNow);
        }

        public ClaimsPrincipal ValidateJwtToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_secretKey);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            };

            SecurityToken validatedToken;
            var principal = tokenHandler.ValidateToken(token, validationParameters, out validatedToken);

            
            if (IsTokenBlacklisted(token))
            {
                throw new SecurityTokenException("Token is blacklisted");
            }

            return principal;
        }
        public void BlacklistToken(string token)
        {
            var blacklistedToken = _blacklistedTokens.FirstOrDefault(t => t.Token == token);
            if (blacklistedToken != null)
            {
                _blacklistedTokens.Remove(blacklistedToken);
            }
        }
    }
}
