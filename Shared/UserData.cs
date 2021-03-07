using System.Collections.Generic;
using System.Security.Claims;
using Shared.Enums;

namespace Shared
{
    public class UserData
    {
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public Dictionary<string, string> Policies { get; set; }
        public string Token { get; set; }
        public Language Language { get; set; }
    }
}