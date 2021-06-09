using System;

namespace Shared.ProfileHelpers
{
    public class UpdateProfileBirth
    {
        public DateTime? Birth { get; }

        public UpdateProfileBirth(DateTime? birth) => Birth = birth;
    }
}