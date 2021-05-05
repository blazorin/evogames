using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Model.Data;
using Shared.Dto;
using Shared.Enums;
using Shared.Utils;

namespace Model.Services
{
    public class ProfileServices : IProfileServices
    {
        private readonly EvoGamesContext _ctx;
        private readonly IMapper _mapper;

        public ProfileServices(EvoGamesContext ctx, IMapper mapper)
        {
            _ctx = ctx;
            _mapper = mapper;
        }

        public async Task<UserProfileDto> GetProfile(string id)
        {
            var storedProfile = await _ctx.Users
                .Where(u => u.UserId == id)
                .ProjectTo<UserProfileDto>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            return storedProfile;
        }

        public async Task<bool> UpdateBirth(string id, DateTime? newBirth)
        {
            var storedProfile = await FindCrowdUser(id);

            storedProfile.Birth = newBirth;
            storedProfile.Logs.Add(new UserLog
            {
                Date = DateTime.Now, UserLogId = Guid.NewGuid().ToString() + Guid.NewGuid(),
                UserLogType = UserLogType.BirthChanged
            });

            return await _ctx.SaveChangesAsync() != 1;
        }

        public async Task<bool> UpdateCountry(string id, string country)
        {
            var storedProfile = await FindCrowdUser(id);

            storedProfile.Country = country;
            storedProfile.Logs.Add(new UserLog
            {
                Date = DateTime.Now, UserLogId = Guid.NewGuid().ToString() + Guid.NewGuid(),
                UserLogType = UserLogType.CountryChanged
            });

            return await _ctx.SaveChangesAsync() != 1;
        }

        private async Task<User> FindCrowdUser(string id)
        {
            return await _ctx.Users
                .Where(u => u.UserId == id)
                .Include(u => u.Logs)
                .FirstOrDefaultAsync();
        }
    }
}