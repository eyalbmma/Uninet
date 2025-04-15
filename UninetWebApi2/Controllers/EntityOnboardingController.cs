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

        [HttpPost("Join_entity")]
        public async Task<IActionResult> JoinEntity([FromBody] JoinEntityRequest request)
        {
            foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState); // הצגת שגיאות קלט
            var result = await _userServiceApp.JoinEntityAsync(request);
            return Ok(result);
        }
    }
}
