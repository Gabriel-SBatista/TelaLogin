using Login.Core.Entities;
using Login.Core.Presenter;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Login.Core.Services.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly string _stringKey;

        public TokenService(string stringKey)
        {
            _stringKey = stringKey;
        }

        public string GenerateToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim("id", user.Id.ToString()),
            new Claim("username", user.Username),
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_stringKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                        expires: DateTime.Now.AddHours(3),
                        claims: claims,
                        signingCredentials: creds
                        );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public TokenInfo? ValidateToken(string token)
        {
            if (token == null)
            {
                return null;
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_stringKey);

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateAudience = false,
                    ValidateIssuer = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                return new TokenInfo
                {
                    UserId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value),
                    Username = jwtToken.Claims.First(x => x.Type == "username").Value
                };
            }
            catch
            {
                return null;
            }
        }
    }
}
