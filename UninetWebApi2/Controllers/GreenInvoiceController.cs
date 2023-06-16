using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Uninet.APP.Interfaces;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GreenInvoiceController : ControllerBase
    {
        private readonly IUninetSimulateGreenVoiceAppServices _uninetSimulateGreenVoiceAppServices;
        //private readonly ILogger<UninetInputController> _logger;

        public GreenInvoiceController(IUninetSimulateGreenVoiceAppServices uninetSimulateGreenVoiceAppServices)// ICustomerDiagnosisService CustomerDiagnosisService
        {

            _uninetSimulateGreenVoiceAppServices = uninetSimulateGreenVoiceAppServices;

        }

        [HttpGet("PullBusinessDataByTaxid/{Taxid}")]
        public async Task<IActionResult> PullBusinessDataByTaxid(string Taxid)
        {
            


            string jsonstring = await _uninetSimulateGreenVoiceAppServices.PullBusinessDataByTaxid(Taxid);
            //List<string> jsonstring = new List<string>();
            //foreach (BsonDocument document in List)
            //{
            //    jsonstring.Add(document.ToJson());
            //}

            return Ok(jsonstring);

        }



        [HttpGet("QueryGenericItemByNameValue/{name}/{value}")]
        public async Task<IActionResult> QueryGenericItemByNameValue(string name, string value)
        {
            //Task<List<BsonDocument>>


            string jsonstring = await _uninetSimulateGreenVoiceAppServices.GeneralQueryBynameValue(name, value);
            //List<string> jsonstring = new List<string>();
            //foreach (BsonDocument document in List)
            //{
            //    jsonstring.Add(document.ToJson());
            //}

            return Ok(jsonstring);

        }



    }
}
