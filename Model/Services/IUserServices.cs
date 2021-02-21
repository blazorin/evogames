using System.Threading.Tasks;
using Shared;
using Shared.Dto;

namespace Model.Services
{
    public interface IUserServices
    {
        Task<UserDto> GetUserByAuthenticationAsync(UserCredentials credentials);
    }
}