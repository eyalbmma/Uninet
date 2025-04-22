using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Uninet.Domain.Models;          // JoinEntityRequest וכו'
using Uninet.APP.Interfaces;          // IUserServiceApp
using UninetWebApi2.Helpers;
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

        [HttpPost("Join_entity")]
        public async Task<IActionResult> JoinEntity([FromBody] JoinEntityRequest request)
        {
            var context = UserContextHelper.GetUserContext(User);

            if (context.systemType != "externalSystem")
            {
                return Unauthorized("Only external systems can call this endpoint.");
            }

            var systemGuid = context.systemGuid.Value;

            // המשך לוגיקה של JoinEntity
            var result = await _userServiceApp.JoinEntityAsync(request);

            return Ok(result);
        }

    }
}
