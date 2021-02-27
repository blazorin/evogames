using System.Collections.Generic;

namespace Shared.Utils
{
    public static class BlackList
    {
        public static List<string> Names { get; } = new()
        {
            "admin",
            "root",
            "owner",
            "moderator",
            "helper",
            "hitler",
            "evogames",
            "evo.games",
            "xxx",
            "momo"
        };
    }
}