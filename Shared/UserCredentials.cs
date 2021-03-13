using System.ComponentModel.DataAnnotations;
using Shared.Extensions.DataAnnotations;
using Shared.Utils;

namespace Shared
{
    public class UserCredentials
    {
        [RequiredField, FieldLength(FieldLenghts.User.Mail), EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}