using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Uninet.APP.Interfaces;
using Uninet.APP.Services;
using Uninet.Domain.Models;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UninetOutPutControllercs : ControllerBase
    {
        private readonly IUninetOutPutAppService _uninetOutPutAppService;
        public UninetOutPutControllercs(IUninetOutPutAppService uninetOutPutAppService)// ICustomerDiagnosisService CustomerDiagnosisService
        {

            _uninetOutPutAppService = uninetOutPutAppService;
           
        }

        [Authorize]
        [HttpGet("GetDigitalDocumentToApproveListByUser")]
        public async Task<ActionResult> GetDigitalDocumentToApproveListByUser()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _uninetOutPutAppService.GetDigitalDocumentToApproveListByUser(Convert.ToInt32(userId));
            return Ok(result);

            //return Ok(_uninetOutPutAppService.GetDigitalDocumentToApproveListByUser(Convert.ToInt32(userId)));
            
        }


        [Authorize]
        [HttpPost("ShowDigitalDocumentDetails")]
        
        public async Task<ActionResult> ShowDigitalDocumentDetails([FromBody] ExpensesUserDoRequest expensesUserDoRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _uninetOutPutAppService.ShowDigitalDocumentDetails(expensesUserDoRequest, Convert.ToInt32(userId));

            return Ok(result);
        }


        [Authorize]
        [HttpPost("InsertUserDigitalDocToUninetSystem")]
        //this method gets a 
        public async Task<ActionResult> InsertUserDigitalDocToUninetSystem([FromBody] ExpensesUserDoRequest expensesUserDoRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _uninetOutPutAppService.InsertUserDigitalDocToUninetSystem(expensesUserDoRequest,Convert.ToInt32(userId));

            return Ok(null);
        }




    }
}
