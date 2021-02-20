using System.Collections.Generic;
using Shared.Enums;

namespace Shared
{
    public class UserData
    {
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string Token { get; set; }
        public Language Language { get; set; }
    }
}