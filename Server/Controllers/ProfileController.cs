using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Mvc;
using Model.Services;
using Shared.ApiErrors;
using Model.Extensions;
using Shared.ProfileHelpers;
using Shared.Utils;

namespace Server.Controllers
{
    [ApiController]
    [Authorize]
    [Route("profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileServices _profileServices;
        private IConfiguration _configuration { get; }

        public ProfileController(IProfileServices profileServices, IConfiguration configuration)
        {
            _profileServices = profileServices;
            _configuration = configuration;
        }

        [HttpGet("me")]
        public async Task<IActionResult> GetUserProfile()
        {
            var userId = User.GetId();
            var userProfile = await _profileServices.GetProfile(userId);

            return userProfile == null
                ? new ObjectResult(new InternalServerError("profile_get_error"))
                : Ok(userProfile);
        }

        [HttpPut("birth")]
        public async Task<IActionResult> UpdateBirth(UpdateProfileBirth newBirth)
        {
            // Check if +18
            if (newBirth.Birth == null)
                return Conflict(new ConflictError("birth_empty"));

            var notMajorAje = DateTime.Compare(newBirth.Birth.Value.AddYears(18), DateTime.Now) == 1;

            if (newBirth.Birth.Value.Year < DateTime.Now.Year - 100 || notMajorAje)
                return Conflict(new ConflictError("not_major_age"));

            var userId = User.GetId();
            var result = await _profileServices.UpdateBirth(userId, newBirth.Birth);

            if (result == false)
                return Conflict(new ConflictError("birth_update_error"));

            return Ok();
        }

        [HttpPut("country")]
        public async Task<IActionResult> UpdateCountry(UpdateProfileCountry newCountry)
        {
            if (string.IsNullOrEmpty(newCountry.Country))
                return Conflict(new ConflictError("country_empty"));

            if (!Enum.IsDefined(typeof(BlackList.Countries), newCountry.Country))
                return Conflict(new ConflictError("country_not_allowed"));

            var userId = User.GetId();
            var result = await _profileServices.UpdateCountry(userId, newCountry.Country);

            if (result == false)
                return Conflict(new ConflictError("country_update_error"));

            return Ok();
        }
    }
}