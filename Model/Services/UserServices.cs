using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Model.Utils;
using Shared;
using Shared.Dto;
using Shared.Enums;

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

        public async Task<User> GetUserByAuthenticationAsync(UserCredentials credentials)
        {
            var passwordHashed = HashingHelper.ComputeSha256Hash(credentials.Password);

            var storedUser = await _ctx.Users
                .Where(user => user.Email == credentials.Email && user.PasswordHashed == passwordHashed)
                .Include(user => user.Logs)
                .Include(user => user.Perms)
                .FirstOrDefaultAsync();
            if (storedUser == null)
                return null;

            storedUser.LastLogin = DateTime.Now;
            storedUser.Logs.Add(new UserLog
                {Date = DateTime.Now, UserLogId = Guid.NewGuid().ToString(), UserLogType = UserLogType.Login});
            await _ctx.SaveChangesAsync();

            return storedUser;
        }

        public async Task<User> AddUserAsync(NewUserDto newUserDto)
        {
            var user = _mapper.Map<User>(newUserDto);

            user.Logs = new List<UserLog>
            {
                new()
                {
                    Date = DateTime.Now, UserLogId = Guid.NewGuid().ToString(), UserLogType = UserLogType.SignUp
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
            user.PasswordHashed = HashingHelper.ComputeSha256Hash(newUserDto.Password);


            await _ctx.Users.AddAsync(user);
            await _ctx.SaveChangesAsync();

            return user;
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