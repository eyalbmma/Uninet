using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Uninet.APP.Interfaces;
using Uninet.APP.Services;

namespace UninetWebApi2.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class GetStaticDataController : ControllerBase
    {
        private readonly IUninetInputAppService _uninetInputAppService;
        public GetStaticDataController(IUninetInputAppService uninetInputAppServic)// ICustomerDiagnosisService CustomerDiagnosisService
        {

            _uninetInputAppService = uninetInputAppServic;
            
        }

        [HttpGet("{questionNumber}/{language}")]
        public IActionResult GetQuestion(int questionNumber, string language)
        {
            var res = _uninetInputAppService.GetQuestion(questionNumber, language);
            return Ok(res.Result.ToJson());

           
        }




        [HttpGet("{language}")]
        public IActionResult GetLandingPageContent(string language)
        {
            var res = _uninetInputAppService.GetLandingPageContent( language);
            return Ok(res.Result.ToJson());


        }

    }
}
