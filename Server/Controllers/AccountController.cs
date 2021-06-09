using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Model.Data;
using Model.Services;
using Shared;
using Shared.ApiErrors;
using Shared.Dto;
using Shared.Utils;
using Google.Apis.Auth;
using Model.Enums;
using Model.Utils;
using Shared.Enums;
using Shared.Oauth;

namespace Server.Controllers
{
    [ApiController]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        private readonly IUserServices _userServices;
        private IConfiguration _configuration { get; }

        public AccountController(IUserServices userServices, IConfiguration configuration)
        {
            _userServices = userServices;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(UserCredentials credentials)
        {
            /*
             * Previous checks
             */

            #region Login Checks

            if (User?.Identity != null && User.Identity.IsAuthenticated)
                return Forbid();

            //TODO: Recaptcha

            if (string.IsNullOrWhiteSpace(credentials.Email) || string.IsNullOrWhiteSpace(credentials.Password))
                return Unauthorized(new UnauthorizedError("email_or_password_blank"));

            if (credentials.Email.Length > 100 || !CheckValidEmail.Validate(credentials.Email))
                return Unauthorized(new UnauthorizedError("email_invalid"));

            var user = await _userServices.GetUserByAuthenticationAsync(credentials, UserLogType.Login);
            if (user == null)
                return Unauthorized(new UnauthorizedError("email_or_password_incorrect"));

            // TODO: Verificaciones de seguridad adicionales

            #endregion

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

            #region Register Checks

            if (User?.Identity != null && User.Identity.IsAuthenticated)
                return Forbid();

            if (string.IsNullOrWhiteSpace(newUser.Email) || string.IsNullOrWhiteSpace(newUser.Password))
                return Unauthorized(new UnauthorizedError("email_or_password_blank"));

            if (string.IsNullOrWhiteSpace(newUser.Name) || newUser.Name.Length < 4)
                return Unauthorized(new UnauthorizedError("username_too_small"));

            if (newUser.Name.Length > 15)
                return Unauthorized(new UnauthorizedError("username_too_big"));

            // Check username regex
            if (!newUser.Name.All(char.IsLetterOrDigit))
                return Unauthorized(new UnauthorizedError("username_symbols"));

            // Check if username is blacklisted
            if (BlackList.Names.Any(name => name == newUser.Name))
                return Unauthorized(new UnauthorizedError("username_blacklisted"));

            // Check username if registered
            var userRegistered = await _userServices.UsernameExistsAsync(newUser.Name);
            if (userRegistered)
                return Unauthorized(new UnauthorizedError("username_already_exists"));

            if (newUser.Email.Length > 100 || !CheckValidEmail.Validate(newUser.Email))
                return Unauthorized(new UnauthorizedError("email_invalid"));

            // Check email if registered
            var emailRegistered = await _userServices.EmailExistsAsync(newUser.Email);
            if (emailRegistered)
                return Unauthorized(new UnauthorizedError("email_already_exists"));

            // Check if +18
            if (newUser.Birth == null)
                return Unauthorized(new UnauthorizedError("birth_empty"));

            var notMajorAje = DateTime.Compare(newUser.Birth.Value.AddYears(18), DateTime.Now) == 1;

            if (newUser.Birth.Value.Year < DateTime.Now.Year - 100 || notMajorAje)
                return Unauthorized(new UnauthorizedError("not_major_age"));

            // Check if country is allowed
            if (!Enum.IsDefined(typeof(BlackList.Countries), newUser.Country))
                return Unauthorized(new UnauthorizedError("country_not_allowed"));

            // Check password strength
            if (newUser.Password.Length < 8)
                return Unauthorized(new UnauthorizedError("password_short"));
            if (!Regex.IsMatch(newUser.Password, @"[A-Z]"))
                return Unauthorized(new UnauthorizedError("password_must_one_capital"));
            if (!Regex.IsMatch(newUser.Password, @"[a-z]"))
                return Unauthorized(new UnauthorizedError("password_must_one_lowercase"));
            if (!Regex.IsMatch(newUser.Password, @"[0-9]"))
                return Unauthorized(new UnauthorizedError("password_must_one_digit"));

            #endregion

            /*
             * If all ok, Continues here
             */

            var user = await _userServices.AddUserAsync(newUser, UserLogType.SignUp);

            var userData = HandleGenerateToken(user);
            return Ok(userData);
        }

        /* Oauth Web API Calls */

        [HttpPost("oauth/google")]
        public async Task<IActionResult> GoogleOauth(GoogleLoginRequest request)
        {
            if (User?.Identity != null && User.Identity.IsAuthenticated)
                return Forbid();

            GoogleJsonWebSignature.Payload payload;
            try
            {
                payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken,
                    new GoogleJsonWebSignature.ValidationSettings
                    {
                        Audience = new[] { _configuration["Authentication:Google:ClientId"] }
                    });

                // It is important to add your ClientId as an audience in order to make sure
                // that the token is for Evo.Games!
            }
            catch
            {
                return Unauthorized(new UnauthorizedError("google_invalid_token"));
            }

            OAuthUserCredentials credentials = new()
            {
                Email = payload.Email,
                Username = payload.Email.Substring(0, payload.Email.IndexOf("@", StringComparison.Ordinal)
                ).Replace(".", string.Empty).Replace("-", string.Empty)
            };

            var user = await _userServices.HandleOauthAuthenticationAsync(credentials, OauthType.Google);
            if (user == null)
                return Unauthorized(new UnauthorizedError("google_oauth_error"));

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