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
        public async Task<ActionResult> GetDigitalDocumentToApproveListByUser(string Typelist, int? subCompanyId = null, int pageNumber = 1, int pageSize = 100)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var result = await _uninetOutPutAppService.GetDigitalDocumentToApproveListByUser(Convert.ToInt32(userId), Typelist, subCompanyId, pageNumber, pageSize);

            return Ok(result);
        }





        [Authorize]
        [HttpPost]
        [Route("ShowDigitalDocumentDetails")]
        public async Task<ActionResult> ShowDigitalDocumentDetails([FromBody] DigitalDocumentDInputRequest expensesUserDoRequest)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(expensesUserDoRequest.ClientVat_id))
            {
                ShowingDocsResults docsResults = new ShowingDocsResults();
                docsResults.Success = false;

                docsResults.ErrSec = expensesUserDoRequest.Lang == 1 ? "no documents left in the inbox" : "לא נותרו מסמכים בתיבת הדואר הניכנס";
                var ZeroDocsResponse = new ExpensesDigitalDocumentProp
                {
                    Supplier_name_Sender = null,
                    Supplier_ID = "0",
                    DocNumber = null,
                    Doctype = null,
                    DocDate = default(DateTime),
                    AmountAV = 0.0,
                    currencyName = null,
                    CurrenctRateValue = 0,
                    ExpenseTypeList = null,
                    internalCompanyId = 0,
                    Jsondocumentid = null,
                    TaxId = null,
                    AmountBeforeVat = 0.0,
                    Vat = 0.0,
                    showingDocsResults = docsResults
                };
                return Ok(ZeroDocsResponse);
            }

            // Check for other fields if necessary
            // Assuming your service handles the business logic and can work with optional fields
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
        public async Task<ActionResult> AproveDoc([FromBody] InsertUserDigitalDocRequest expensesUserDoRequest)
        {
            string message = "";
            createExpenseApiResponse result;

            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                result = await _uninetOutPutAppService.InsertUserDigitalDocToUninetSystem(expensesUserDoRequest, Convert.ToInt32(userId));
            }
            catch (Exception ex)
            {
                // Handle unexpected errors gracefully
                result = new createExpenseApiResponse
                {
                    status = false,
                    reason = "An unexpected error occurred. Please try again later."
                };

                Console.WriteLine($"Error in AproveDoc: {ex.Message}");
            }

            if (result?.status == true)
            {
                message = expensesUserDoRequest.Lang == 1
                    ? "Document transferred to Uninet successfully"
                    : "המסמכים התקבלו בהצלחה ביונינט";
            }
            else
            {
                var errorMessage = result?.reason ?? "Unknown error occurred.";
                message = expensesUserDoRequest.Lang == 1
                    ? errorMessage
                    : "אירעה שגיאה: " + errorMessage;
            }

            if (result != null)
            {
                result.textResponse = message;
            }
            else
            {
                result = new createExpenseApiResponse
                {
                    status = false,
                    textResponse = message
                };
            }

            return Ok(result);
        }









    }
}
