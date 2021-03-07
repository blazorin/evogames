using System.Threading.Tasks;
using Model.Data;
using Shared;
using Shared.Dto;

namespace Model.Services
{
    public interface IUserServices
    {
        Task<User> GetUserByAuthenticationAsync(UserCredentials credentials);
        Task<User> AddUserAsync(NewUserDto newUserDto);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
    }
}