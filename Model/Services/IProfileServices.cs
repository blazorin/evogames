using System;
using System.Threading.Tasks;
using Shared.Dto;
using Shared.Utils;

namespace Model.Services
{
    public interface IProfileServices
    {
        Task<UserProfileDto> GetProfile(string id);
        Task<bool> UpdateBirth(string id, DateTime? newBirth);
        Task<bool> UpdateCountry(string id, string country);
    }
}