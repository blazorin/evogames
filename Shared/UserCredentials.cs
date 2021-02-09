using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class UserCredentials
    {
        [Required] public string Username { get; set; }
        [Required] public string Password { get; set; }
    }
}