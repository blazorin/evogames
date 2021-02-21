using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Model.Data;
using Model.Services;
using Shared;

namespace Server.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserServices _userServices;

        public AccountController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserCredentials credentials)
        {
            if (credentials == null)
                return Unauthorized();

            if (User?.Identity != null && User.Identity.IsAuthenticated)
                return Forbid();

            var user = await _userServices.GetUserByAuthenticationAsync(credentials);
            if (user == null)
                return Unauthorized();

            // TODO: Verificaciones de seguridad adicionales

            var policies = new List<Claim>();
            var roles = new List<string>();
            policies.AddRange(user.Perms.Select(perm => new Claim(perm.PermKey, perm.PermValue)));

            if (user.IsOwner)
                roles.Add("owner");
            if (user.IsDeveloper)
                roles.Add("developer");
            if (user.IsAdmin)
                roles.Add("admin");
            if (user.IsModerator)
                roles.Add("moderator");

            var response = new UserData
            {
                Username = user.Name,
                Language = user.Language,
                Policies = policies,
                Roles = roles,
                Token = GenerateToken(user, roles, policies)
            };

            return Ok(response);
        }

        private string GenerateToken(User user, IEnumerable<string> roles, IEnumerable<Claim> policies)
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
            claims.AddRange(policies);

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