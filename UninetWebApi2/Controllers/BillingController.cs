using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Uninet.APP.Interfaces;
using Uninet.APP.Services;
using Uninet.Domain.Models;
using UninetWebApi2.Helpers;
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


        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("Save")]

        public async Task<ActionResult> Save([FromBody] CreditCard creditCard)
        {

            try
            {
                var successMessage = BillingMessage.GetSuccessMessage(creditCard.Lang);
                return Ok(successMessage);
            }
            catch (Exception ex) 
            { 
                var failureMessage = BillingMessage.GetFailureMessage(creditCard.Lang);
                return BadRequest(failureMessage);
            }

        }

        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("ContinueFreeBilling")]
        public async Task<ActionResult> ContinueFreeBilling(ContinueFreeInput continueFreeInput)
        {
            try
            {
                var context = UserContextHelper.GetUserContext(User);

                int? userId = null;
                Guid? systemGuid = null;

                if (context.systemType == "adminUser")
                {
                    userId = context.userId.Value; // שמירת userId
                }
                else if (context.systemType == "externalSystem")
                {
                    return Unauthorized("This endpoint is for UI users only. Please login via the user interface.");
                }
                else
                {
                    return Unauthorized("Invalid user context.");
                }


                var ContinueFreeBillingResult = await _billingService.ContinueFreeBilling(continueFreeInput, Convert.ToInt32(userId));
                return Ok(ContinueFreeBillingResult);
            }
            catch (Exception ex) { return null; }
        }
        [Authorize(Policy = "AdminPolicy")]
        [HttpPost("ShowUserBillingDetails")]
        public async Task<ActionResult> ShowUserBillingDetails([FromBody] UserDetails userDetails)
        {
            try
            {
                var context = UserContextHelper.GetUserContext(User);

                int? userId = null;
                Guid? systemGuid = null;

                if (context.systemType == "adminUser")
                {
                    userId = context.userId.Value; // שמירת userId
                }
                else if (context.systemType == "externalSystem")
                {
                    return Unauthorized("This endpoint is for UI users only. Please login via the user interface.");
                }
                else
                {
                    return Unauthorized("Invalid user context.");
                }

                userDetails.UserId = Convert.ToInt32(userId);
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
