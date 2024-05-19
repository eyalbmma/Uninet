using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Uninet.APP.Interfaces;
using Uninet.Domain.Models;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BillingController : ControllerBase
    {


        private IbillingService _billingService;
        public BillingController(IbillingService billingService)
        {
            _billingService = billingService;
            
        }


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


        [Authorize]
        [HttpPost("ShowUserBillingDetails")]
        public async Task<ActionResult> ShowUserBillingDetails([FromBody] UserDetails userDetails)
        {
            try
            {
                var res = await _billingService.ShowUserBillingDetails(userDetails);
                return Ok(res);
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }
}
