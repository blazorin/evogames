using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace Shared.Dto
{
    public class UserProfileDto
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public DateTime? Birth { get; set; }

        public string Country { get; set; }
    }
}