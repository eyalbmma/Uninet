using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uninet.Domain.Models;          // JoinEntityRequest וכו'
using Uninet.APP.Interfaces;          // IUserServiceApp

namespace UninetWebApi2.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EntityOnboardingController : ControllerBase
    {
        private readonly IUserServiceApp _userServiceApp;

        public EntityOnboardingController(IUserServiceApp userServiceApp)
        {
            _userServiceApp = userServiceApp;
        }
        [Authorize]
        [HttpPost("Join_entity")]
        public async Task<IActionResult> JoinEntity([FromBody] JoinEntityRequest request)
        {
            // שליפת מזהה המשתמש מתוך ה-JWT Token
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not found in token.");

            var result = await _userServiceApp.JoinEntityAsync(request, userId);
            return Ok(result);
        }
    }
}
