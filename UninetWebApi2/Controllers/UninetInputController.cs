using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Uninet.APP.Interfaces;
using Uninet.Domain.Models;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Azure;
using System.Xml.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Uninet.APP.Services;
using MongoDB.Bson;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization;
using Twilio.Http;
using Newtonsoft.Json.Linq;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace UninetWebApi2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UninetInputController : ControllerBase
    {
        private readonly IUninetInputAppService _uninetInputAppService;
        public IConfiguration Configuration { get; }
        //private readonly ILogger<UninetInputController> _logger;
        private readonly IUninetSimulateGreenVoiceAppServices _uninetSimulateGreenVoiceAppServices;
        public UninetInputController(IUninetInputAppService uninetInputAppServic, IUninetSimulateGreenVoiceAppServices uninetSimulateGreenVoiceAppServices, IConfiguration configuration)// ICustomerDiagnosisService CustomerDiagnosisService
        {
            Configuration = configuration;
            _uninetInputAppService = uninetInputAppServic;
            _uninetSimulateGreenVoiceAppServices = uninetSimulateGreenVoiceAppServices;
        }
        [Authorize]
        [HttpGet("GetTestResponse")]
        public async Task<List<TestResponse>> GetTestResponse()
        {
            var accessToken = HttpContext.GetTokenAsync("access_token").Result;

            return await _uninetInputAppService.GetTestResponse();
        }


       
        [HttpPost("GetBusinessExpenses")]
        public async Task<ActionResult> GetBusinessExpenses([FromBody] List<BusinessRequest> RegisterUserReq)
        {
            try
            {
               


                return Ok(null);
            }
            catch (Exception ex)
            {
                return Ok(false);
            }



        }

        




        [HttpGet("GetGreenVoiceDocByNameValueAndSave")]
        public async Task<IActionResult> GetGreenVoiceDocByNameValueAndSave(string name, string value)
        {



            using var client = new System.Net.Http.HttpClient();

            // Send an HTTP GET request to the specified URL
            var response = await client.GetAsync("https://localhost:7285/api/GreenInvoice/QueryGenericItemByNameValue/" + name + "/" + value);


            // Read the response content as a string
            var json = await response.Content.ReadAsStringAsync();

            // Deserialize the string into a BsonArray
            BsonArray bsonArray = BsonSerializer.Deserialize<BsonArray>(json);

            // Convert the BsonArray to a list of BsonDocuments
            List<BsonDocument> bsonDocuments = new List<BsonDocument>();
            foreach (BsonValue bsonValue in bsonArray)
            {
                BsonDocument bsonDocument = bsonValue.ToBsonDocument();
                bsonDocuments.Add(bsonDocument);
            }


            ///save json into MongoDb Collection
            var res = _uninetInputAppService.SavegreenvoicedocumentIntoUninet(bsonDocuments);




            //save data into sql table BusinessData




            return Ok(res);









        }


        
        [HttpPost("ReceiveWebhookMorning")]
        [AllowAnonymous]
        public async Task<IActionResult> ReceiveWebhookMorning()
        {
            try
            {
                
                string WebHookSourceid = HttpContext.Request.Query["webhooksourceid"].ToString();

                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    string json = await reader.ReadToEndAsync();

                    var res = await _uninetInputAppService.MorningReceiveWebhook(json);//

                    return Ok(json);
                };
            }
            catch (Exception ex)
            {
                return BadRequest("Failed to process webhook: " + ex.Message);
            }
        }






        [HttpPost("ReceiveWebhook")]
        public async Task<IActionResult> ReceiveWebhook()
        {
            try
            {

                string WebHookSourceid = HttpContext.Request.Query["webhooksourceid"].ToString();

                
                using (StreamReader reader = new StreamReader(Request.Body))
                {
                    string json = await reader.ReadToEndAsync();

                    var res = await _uninetInputAppService.ReceiveWebhook(json, WebHookSourceid);//

                    return Ok(res);
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Failed to process webhook: {ex.Message}");
            }
        }



        [HttpGet("GetGreenVoiceDocByNameValue")]
        public  async Task<IActionResult> GetGreenVoiceDocByNameValue(string name,string value)
        {

           

            using var client = new System.Net.Http.HttpClient();

            // Send an HTTP GET request to the specified URL
            var response = await client.GetAsync("https://localhost:7285/api/GreenInvoice/QueryGenericItemByNameValue/"+name+"/"+value);
           
            // Read the response content as a string
            var responseContent = await response.Content.ReadAsStringAsync();

            // Deserialize the JSON data into a list of objects
            //var data = JsonConvert.DeserializeObject<List<MyObject>>(responseContent);

            // Do something with the deserialized data
            //foreach (var item in data)
            //{
            //    Console.WriteLine(item.Title);
            //}

            return await Task.FromResult(new OkObjectResult(responseContent));
        }

    }
}
