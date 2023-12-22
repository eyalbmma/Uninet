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
        //here we get all document that the client is getting to his uninet system approved rejected and not each one of them 
        [Authorize]
        [HttpGet("GetDigitalDocumentToApproveListByUser")]
        public async Task<ActionResult> GetDigitalDocumentToApproveListByUser(string Typelist)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _uninetOutPutAppService.GetDigitalDocumentToApproveListByUser(Convert.ToInt32(userId), Typelist);

            //var result = await _uninetOutPutAppService.test(Convert.ToInt32(userId), Typelist);
            return Ok(result);

            //return Ok(_uninetOutPutAppService.GetDigitalDocumentToApproveListByUser(Convert.ToInt32(userId)));
            
        }


        [Authorize]
        [HttpPost("ShowDigitalDocumentDetails")]
        
        public async Task<ActionResult> ShowDigitalDocumentDetails([FromBody] DigitalDocumentDInputRequest expensesUserDoRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _uninetOutPutAppService.ShowDigitalDocumentDetails(expensesUserDoRequest, Convert.ToInt32(userId));

            return Ok(result);
        }


        [Authorize]
        [HttpPost("RejectDocument")]
        //this method gets a 
        public async Task<ActionResult> RejectDocument([FromBody] RequestRejectDocument requestRejectDocument)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _uninetOutPutAppService.RejectDocument(requestRejectDocument);

            return Ok(result);
        }

        [Authorize]
        [HttpPost("InsertUserDigitalDocToUninet")]
        //this method gets a 
        //[FromBody] InsertUserDigitalDocRequest expensesUserDoRequest
        public async Task<ActionResult> InsertUserDigitalDocToUninet()
        {
            //var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //var result = await _uninetOutPutAppService.InsertUserDigitalDocToUninetSystem(expensesUserDoRequest, Convert.ToInt32(userId));

            return Ok(true);
        }


        [Authorize]
        [HttpPost("AddexpenseType")]
        //this method gets a 
        //[FromBody] InsertUserDigitalDocRequest expensesUserDoRequest
        public async Task<ActionResult> AddexpenseType([FromBody] AddexpenseTypeRequest addexpenseTypeRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _uninetOutPutAppService.AddexpenseType(addexpenseTypeRequest, Convert.ToInt32(userId));
            return Ok(result);
        }




        [Authorize]
        [HttpPost("AproveDoc")]
        //this method gets a 
        //[FromBody] InsertUserDigitalDocRequest expensesUserDoRequest
        public async Task<ActionResult> AproveDoc([FromBody] InsertUserDigitalDocRequest expensesUserDoRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var result = await _uninetOutPutAppService.InsertUserDigitalDocToUninetSystem(expensesUserDoRequest, Convert.ToInt32(userId));

            string message = "";
            if (result.status)
            {
                
                    message = expensesUserDoRequest.Lang == 1 ? "Document transfered to Uninet System" : "המסמכים התקבלו בהצלחה במערכות יונינט";
               


            }
            else
            {
                message = expensesUserDoRequest.Lang == 1 ? "There was An Error Document wasnt transfered to Uninet system" : "ארעה שגיאה המסמכים לא התקבלו במערכת יונינט";
            }
            result.textResponse= message;

            return Ok(result);
        }







    }
}
