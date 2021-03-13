using System.Threading.Tasks;
using Model.Data;
using Model.Enums;
using Model.Utils;
using Shared;
using Shared.Dto;
using Shared.Enums;

namespace Model.Services
{
    public interface IUserServices
    {
        Task<User> GetUserByAuthenticationAsync(UserCredentials credentials, UserLogType logType);
        Task<User> AddUserAsync(NewUserDto newUserDto, UserLogType logType);
        Task<User> HandleOauthAuthenticationAsync(OAuthUserCredentials credentials, OauthType oauthType);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
    }
}