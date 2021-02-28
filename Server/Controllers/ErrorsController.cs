using System.Net;
using Microsoft.AspNetCore.Mvc;
using Shared.ApiErrors;

namespace Server.Controllers
{
    [ApiController]
    [Route("errors")]
    public class ErrorsController : ControllerBase
    {
        [Route("{code}")]
        public IActionResult Error(int code)
        {
            HttpStatusCode parsedCode = (HttpStatusCode) code;
            ApiError error = new ApiError(code, parsedCode.ToString());

            return new ObjectResult(error);
        }
    }
}