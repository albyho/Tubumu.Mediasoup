using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Tubumu.Meeting.Server.Authorization;

namespace Tubumu.Meeting.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public TokenController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("createToken")]
        public string CreateToken(string userIdOrUsername)
        {
            var token = _tokenService.GenerateAccessToken(new[] { new Claim(ClaimTypes.Name, userIdOrUsername) });
            return token;
        }
    }
}
