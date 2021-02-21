using System;
using Shared.Enums;

namespace Shared.Dto
{
    public class UserDto : UserProfileDto
    {
        public string UserId { get; set; }

        public bool IsOwner { get; set; }
        public bool IsDeveloper { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsModerator { get; set; }

        public decimal Balance { get; set; }
        public DateTime CreationDate { get; set; }
        public Language Language { get; set; }
    }
}