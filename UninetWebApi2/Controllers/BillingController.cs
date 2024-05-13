using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Uninet.Domain.Models;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {
        [Authorize]
        [HttpPost("Save")]

        public async Task<ActionResult> Save([FromBody] CreditCard creditCard)
        {

            try
            {
                var successMessage = BillingMessage.GetSuccessMessage();
                return Ok(successMessage);
            }
            catch (Exception ex) 
            { 
                var failureMessage = BillingMessage.GetFailureMessage();
                return BadRequest(failureMessage);
            }

        }

    }
}
