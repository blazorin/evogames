using System;

namespace Shared.Dto
{
    public class AdminUserDto : UserDto
    {
        public float UnconfirmedDeposits { get; set; }
        public float UnconfirmedWithdraws { get; set; }
        public DateTime LastLogin { get; set; }
    }
}