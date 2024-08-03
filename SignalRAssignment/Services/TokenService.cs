using Microsoft.IdentityModel.Tokens;
using SignalRAssignment.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace SignalRAssignment.Services
{
    public class TokenService
    {
        private readonly RSA _privateKey;

        public TokenService()
        {
            _privateKey = GetPrivateKey("C:\\Users\\Raiku\\source\\repos\\SignalRAssignment\\SignalRAssignment\\Key\\privateKey.pem");
        }

        public string GenerateToken(Member user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim("Role", user.Type ? "Staff" : "User")
            };

            var creds = new SigningCredentials(new RsaSecurityKey(_privateKey), SecurityAlgorithms.RsaSha256);

            var token = new JwtSecurityToken(
                issuer: "RaiYugi",
                audience: "Saint",
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static RSA GetPrivateKey(string privateKeyPath)
        {
            using var rsa = RSA.Create();
            var privateKey = File.ReadAllText(privateKeyPath);
            rsa.ImportFromPem(privateKey.ToCharArray());
            return rsa;
        }
    }

}
