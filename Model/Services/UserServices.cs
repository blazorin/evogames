using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Shared;
using Shared.Dto;
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

        public async Task<User> GetUserByAuthenticationAsync(UserCredentials credentials)
        {
            var passwordHashed = HashingHelper.ComputeSha256Hash(credentials.Password);

            var storedUser = await _ctx.Users
                .Where(user => credentials.Email == user.Email && passwordHashed == user.PasswordHashed)
                .FirstOrDefaultAsync();

            return storedUser;
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