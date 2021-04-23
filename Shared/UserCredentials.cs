using System.ComponentModel.DataAnnotations;
using Shared.Extensions.DataAnnotations;
using Shared.Utils;

namespace Shared
{
    public class UserCredentials
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }
}