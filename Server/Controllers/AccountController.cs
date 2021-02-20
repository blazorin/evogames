using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Model.Data;

namespace Server.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private string GenerateToken(User user, IEnumerable<string> roles)
        {
            var header = new JwtHeader(
                new SigningCredentials(
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Startup.JwtSecretKey)
                    ),
                    SecurityAlgorithms.HmacSha256)
            );

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Name),
                new(JwtRegisteredClaimNames.UniqueName, user.UserId)
            };
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var payload = new JwtPayload(
                issuer: "EvoGamesServer",
                audience: "EvoGamesClient",
                claims: claims,
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddDays(14)
            );
            var token = new JwtSecurityToken(header, payload);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}