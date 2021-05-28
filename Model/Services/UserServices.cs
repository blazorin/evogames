using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Enums;
using Model.Utils;
using Shared;
using Shared.Dto;
using Shared.Enums;
using Shared.Utils;

namespace Model.Services
{
    public class UserServices : IUserServices
    {
        private readonly EvoGamesContext _ctx;
        private readonly IMapper _mapper;

        public UserServices(EvoGamesContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<User> GetUserByAuthenticationAsync(UserCredentials credentials, UserLogType logType)
        {
            User storedUser;

            if (logType is UserLogType.GoogleLogin or UserLogType.DiscordLogin)
            {
                storedUser = await _ctx.Users
                    .Where(user => user.Email == credentials.Email)
                    .Include(user => user.Logs)
                    .Include(user => user.Perms)
                    .FirstOrDefaultAsync();
            }
            else
            {
                var passwordHashed = HashingHelper.ComputeSha256Hash(credentials.Password);
                storedUser = await _ctx.Users
                    .Where(user =>
                        user.Email == credentials.Email && !string.IsNullOrEmpty(user.PasswordHashed) &&
                        user.PasswordHashed == passwordHashed)
                    .Include(user => user.Logs)
                    .Include(user => user.Perms)
                    .FirstOrDefaultAsync();
            }

            if (storedUser == null)
                return null;

            storedUser.LastLogin = DateTime.Now;
            storedUser.Logs.Add(new UserLog
                {Date = DateTime.Now, UserLogId = Guid.NewGuid().ToString() + Guid.NewGuid(), UserLogType = logType});
            await _ctx.SaveChangesAsync();

            return storedUser;
        }

        public async Task<User> AddUserAsync(NewUserDto newUserDto, UserLogType logType)
        {
            var user = _mapper.Map<User>(newUserDto);

            user.Logs = new List<UserLog>
            {
                new()
                {
                    Date = DateTime.Now, UserLogType = logType, UserLogId = Guid.NewGuid().ToString() + Guid.NewGuid()
                }
            };

            user.Perms = new List<UserPerms>
            {
                new()
                {
                    PermKey = "vipType", PermValue = "newbie", UserPermsId = Guid.NewGuid().ToString()
                }
            };

            user.UserId = Guid.NewGuid().ToString();
            user.CreationDate = DateTime.Now;
            user.LastLogin = DateTime.Now;

            if (logType == UserLogType.SignUp)
                user.PasswordHashed = HashingHelper.ComputeSha256Hash(newUserDto.Password);


            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return user;
        }

        public async Task<User> HandleOauthAuthenticationAsync(OAuthUserCredentials oauthCredentials,
            OauthType oauthType)
        {
            var emailExists = await EmailExistsAsync(oauthCredentials.Email);
            if (emailExists)
            {
                var userCredentials = new UserCredentials
                {
                    Email = oauthCredentials.Email
                };

                var loginLogType = oauthType switch
                {
                    OauthType.Google => UserLogType.GoogleLogin,
                    OauthType.Discord => UserLogType.DiscordLogin,
                    _ => throw new ArgumentOutOfRangeException(nameof(oauthType), oauthType, null)
                };

                return await GetUserByAuthenticationAsync(userCredentials, loginLogType);
            }

            string username = oauthCredentials.Username.Length switch
            {
                > 15 => oauthCredentials.Username.Substring(0, 15),
                < 4 => oauthCredentials.Username + HashingHelper.GenerateRandomNo(),
                _ => oauthCredentials.Username
            };

            if (BlackList.Names.Any(word => username.Contains(word)))
                username = "goofy";

            string usernameBase = username;
            int count = 0;
            while (await UsernameExistsAsync(username))
            {
                username = (usernameBase.Length > 11 ? usernameBase.Substring(0, 11) : usernameBase) +
                           HashingHelper.GenerateRandomNo();
                count++;


                if (count < 5000) continue;
                // I hope this won't happen :)

                username = "goofy" + HashingHelper.GenerateRandomNo() + HashingHelper.GenerateRandomNo();
                // maybe add alert here, in the future
                break;
            }


            var newUser = new NewUserDto
            {
                Name = username,
                Email = oauthCredentials.Email
            };

            var registerLogType = oauthType switch
            {
                OauthType.Google => UserLogType.GoogleSignUp,
                OauthType.Discord => UserLogType.DiscordSignUp,
                _ => throw new ArgumentOutOfRangeException(nameof(oauthType), oauthType, null)
            };

            return await AddUserAsync(newUser, registerLogType);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _ctx.Users.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _ctx.Users.AnyAsync(u => u.Name == username);
        }
    }
}