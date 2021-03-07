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
using Shared.ApiErrors;
using Shared.Dto;
using Shared.Utils;

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
            /*
             * Previous checks
             */

            if (User?.Identity != null && User.Identity.IsAuthenticated)
                return Forbid();

            if (string.IsNullOrWhiteSpace(credentials.Email) || string.IsNullOrWhiteSpace(credentials.Password))
                return Unauthorized(new UnauthorizedError("email_or_password_blank"));

            if (credentials.Email.Length > 100)
                return Unauthorized(new UnauthorizedError("email_too_big"));

            var user = await _userServices.GetUserByAuthenticationAsync(credentials);
            if (user == null)
                return Unauthorized(new UnauthorizedError("email_or_password_incorrect"));

            // TODO: Verificaciones de seguridad adicionales

            /*
             * If all ok, Continues here
             */

            var userData = HandleGenerateToken(user);
            return Ok(userData);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(NewUserDto newUser)
        {
            /*
             * Previous checks
             */

            if (User?.Identity != null && User.Identity.IsAuthenticated)
                return Forbid();

            if (string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.Password))
                return Unauthorized(new UnauthorizedError("email_or_password_blank"));

            if (string.IsNullOrEmpty(newUser.Name) || newUser.Name.Length < 4)
                return Unauthorized(new UnauthorizedError("username_too_small"));

            if (newUser.Name.Length > 20)
                return Unauthorized(new UnauthorizedError("username_too_big"));

            if (newUser.Email.Length > 100)
                return Unauthorized(new UnauthorizedError("email_too_big"));

            // Check email if registered
            var emailRegistered = await _userServices.EmailExistsAsync(newUser.Email);
            if (emailRegistered)
                return Unauthorized(new UnauthorizedError("email_already_used"));

            // Check user if registered
            var userRegistered = await _userServices.UsernameExistsAsync(newUser.Name);
            if (userRegistered)
                return Unauthorized(new UnauthorizedError("username_already_used"));

            // Check if +18
            var isMajorAje = newUser.Birth.Year + 18 < DateTime.Now.Year;
            if (newUser.Birth == DateTime.MinValue || newUser.Birth.Year < DateTime.Now.Year - 100 || !isMajorAje)
                return Unauthorized(new UnauthorizedError("not_major_age"));

            // Check if username is blacklisted
            var blackList = (IEnumerable<string>) BlackList.Names;
            if (blackList.Any(name => name == newUser.Name))
                return Unauthorized(new UnauthorizedError("username_blacklisted"));

            // Check if country is allowed
            if (!Enum.IsDefined(typeof(BlackList.Countries), newUser.Country))
                return Unauthorized(new UnauthorizedError("country_not_allowed"));


            /*
             * If all ok, Continues here
             */

            var user = await _userServices.AddUserAsync(newUser);

            var userData = HandleGenerateToken(user);
            return Ok(userData);
        }


        [HttpGet("email/{email?}")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            var result = await _userServices.EmailExistsAsync(email);
            return Ok(result);
        }

        [HttpGet("username/{username?}")]
        public async Task<IActionResult> CheckUsername(string username)
        {
            var result = await _userServices.UsernameExistsAsync(username);
            return Ok(result);
        }

        private UserData HandleGenerateToken(User user)
        {
            // Claims are not supported to be sent in JSON (no default constructor), so I've made this workaround
            var policiesInClaims = new List<Claim>();
            policiesInClaims.AddRange(user.Perms.Select(perm => new Claim(perm.PermKey, perm.PermValue)));

            var policiesInString =
                user.Perms.ToDictionary(perm => perm.PermKey, perm => perm.PermValue);

            var roles = new List<string>();

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
                Policies = policiesInString,
                Roles = roles,
                Token = GenerateToken(user, roles, policiesInClaims)
            };
            return response;
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
                new(ClaimTypes.NameIdentifier, user.UserId)
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