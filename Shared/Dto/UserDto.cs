using System;

namespace Shared.Dto
{
    public class UserDto : UserProfileDto
    {
        public string UserId { get; set; }
        public bool IsAdmin { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreationDate { get; set; }
    }
}