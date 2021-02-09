using System.Collections.Generic;

namespace Shared
{
    public class UserData
    {
        public string Username { get; set; }
        public IEnumerable<string> Roles { get; set; }
        public string Token { get; set; }
    }
}