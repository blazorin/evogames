using System;
using Shared.Enums;

namespace Shared.Dto
{
    public class UserDto : UserProfileDto
    {
        public string UserId { get; set; }
        public bool IsAdmin { get; set; }
        public decimal Balance { get; set; }
        public DateTime CreationDate { get; set; }
        public Language Language { get; set; }
    }
}