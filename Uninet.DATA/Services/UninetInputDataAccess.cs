using Amazon.Runtime.Internal.Util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Intrinsics.X86;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using Twilio.Jwt.AccessToken;
using Twilio.TwiML.Voice;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;
using static Azure.Core.HttpHeader;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using MongoDB.Bson.Serialization;

namespace Uninet.DATA.Services
{
    public class UninetInputDataAccess : IUninetInputDataAccess
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IMongoCollection<BsonDocument> _Uninetgreenvoicedocument;

        private readonly IMongoCollection<BsonDocument> _UninetGetStaticQuestionsService;
        private readonly IMongoCollection<BsonDocument> _UninetGetLandingPageDataService;
        public UninetInputDataAccess(IRepository<UninetContext> repository, IMongoClient client)//, IloginRepository loginRepository
        {
            var database = client.GetDatabase("Uninet");
            var Uninetgreenvoicedocument = database.GetCollection<BsonDocument>("UninetGreenVoiceCollection");
            _Uninetgreenvoicedocument = Uninetgreenvoicedocument;
            _repository = repository;
            var uninetGetStaticQuestionsService = database.GetCollection<BsonDocument>("UninetStaticData");
            _UninetGetStaticQuestionsService = uninetGetStaticQuestionsService;


            var UninetGetLandingPageDataService = database.GetCollection<BsonDocument>("UninetHomepageBlocks");
            _UninetGetLandingPageDataService = UninetGetLandingPageDataService;
        }



        //[HttpGet("{questionNumber}/{language}")]
        //public IActionResult GetQuestion(int questionNumber, string language)
        //{
        //    var filter = Builders<BsonDocument>.Filter.And(
        //        Builders<BsonDocument>.Filter.Eq("Questionnumber", questionNumber),
        //        Builders<BsonDocument>.Filter.Eq("Questions.Language", language)
        //    );

        //    var projection = Builders<BsonDocument>.Projection.Include("Questions.$");

        //    var document = _collection.Find(filter).Project(projection).FirstOrDefault();

        //    if (document == null)
        //    {
        //        return NotFound();
        //    }

        //    var question = document["Questions"][0].AsBsonDocument;

        //    return Ok(question.ToJson());
        //}

        public async Task<BsonDocument> GetQuestion(int questionNumber, string language)
        {
            try
            {




                var filter = Builders<BsonDocument>.Filter.Eq("Questionnumber", questionNumber);
                var document = _UninetGetStaticQuestionsService.Find(filter).FirstOrDefault();
                if (document != null)
                {
                    var questions = document["Questions"].AsBsonArray;

                    // access the first item in the "Fields" array
                    if (language == "English")
                    {
                        var question = questions[0].AsBsonDocument;
                        return question;
                    }
                    else
                    {
                        var question = questions[0].AsBsonDocument;
                        return question;
                    }

                }



                return null;


            }
            catch (Exception ex) { return null; }
        }



        public async Task<List<BsonDocument>> GetLandingPageContent(string language)
        {
            try
            {
                List<BsonDocument> ResultLIst = new List<BsonDocument>();
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId("64412ba23c1fe35afcf51ee8"));
                var document = await _UninetGetLandingPageDataService.Find(filter).FirstOrDefaultAsync();

                for (int i = 1; i <= 6; i++)
                {
                    var blockName = $"Block{i}";
                    if (document.Contains(blockName))
                    {
                        var block = document.GetValue(blockName).AsBsonArray;
                        if (language == "English")
                        {
                            ResultLIst.Add(block[0].AsBsonDocument);
                        }
                        else
                        {
                            ResultLIst.Add(block[1].AsBsonDocument);
                        }
                        
                    }
                }
                return ResultLIst;
            }
            catch (Exception ex) { return null; }

        }

        protected string SimulateToken()
        {
            // Define the secret key used to sign the JWT token
            string secretKey = "12345";

            // Define the claims for the JWT token
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, "user123"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            // Create the JWT token
            var token = new JwtSecurityToken(
                issuer: "my_issuer",
                audience: "my_audience",
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                    SecurityAlgorithms.HmacSha256Signature)
            );

            // Convert the JWT token to a string
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Now you have a JWT token string that you can use for testing or other purposes
           return tokenString;
        }


     

    public async Task<string> SendRequest(string endpointUrl, HttpMethod method, string jwtToken)
    {
        // Create a new instance of HttpClient
        using (HttpClient client = new HttpClient())
        {
            // Set the authorization header with the JWT token
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwtToken);

            // Create a new instance of HttpRequestMessage with the specified endpoint URL and HTTP method
            var request = new HttpRequestMessage(method, endpointUrl);

            // Send the HTTP request and await the response
            var response = await client.SendAsync(request);

            // Read the response content as a string and return it
            return await response.Content.ReadAsStringAsync();
        }
    }



         public async Task<string> PullUserDatafromExternalSystem(int Userid)
        {

           
            List<Businesses> res = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == Userid);
           
           
          
            //on this res i have the list of businesses realted to this user 
            foreach (var Business in res)
            {
                //for each bussines i need to get their api key and send request to get jwt token
                //here i will simulate getting the jwt token 
                //string Jwtsimlulation= SimulateToken();//remark this meanwhile

                //now get the end point url for this businessId
                //here comes the logic that decide what apiid to callto for this example we will use 27 -- /api/v1/documents/{id}
                //var Endpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 27);////remark this meanwhile
                //string StrEndpoint = "https://private-anon-5e08cc171e-greeninvoice.apiary-mock.com" + Endpoint; ////remark this meanwhile


                //need to call a generic function that get the end point ,jwt token ,method (put get post ) and send the request 

                //HttpMethod method = new HttpMethod(Endpoint.MethodeType);////remark this meanwhile
                //here i call a function that will call real external system
                // string jsonfromExternalServiceRsult =await SendRequest(StrEndpoint, method, Jwtsimlulation);////remark this meanwhile
                //i need to remark this code until i will have a real api from morning or any other external service


                //from here i call meanwhile greenvoice controller named PullBusinessDataByTaxid
                using var client = new System.Net.Http.HttpClient();

                // Send an HTTP GET request to the specified URL
                var response = await client.GetAsync("https://localhost:7285/api/GreenInvoice/PullBusinessDataByTaxid/" + Business.BusinessId );
                             

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
                var SavingToUninetGreenvoiceCollection = SavegreenvoicedocumentIntoUninet(bsonDocuments);





            }
            return string.Empty;
        }




        public async Task<bool> SavegreenvoicedocumentIntoUninet(List<BsonDocument> InputData)
        {
            try
            {



                for (var i = 0; i < InputData.Count; i++)
                {

                    BsonString IDString = InputData[i]["id"].AsString;
                    
                    var filter = Builders<BsonDocument>.Filter.Eq("id", IDString);
                    var existingDocument = _Uninetgreenvoicedocument.Find(filter).FirstOrDefault();

                    if (existingDocument == null)
                    {
                        // If the document does not exist, add it to a list of documents to insert
                        _Uninetgreenvoicedocument.InsertOne(InputData[i]);
                    }
                }
                //insert Data into UninetGreenVoiceCollection MongoDB
               





                //insert data into sql DB table BusinessData

                for (var i=0;i< InputData.Count;i++)
                {
                    List<string> EmailLIst = new List<string>();
                    BsonDocument client = InputData[i]["client"].AsBsonDocument;
                    BsonArray emails = client["emails"].AsBsonArray;
                    foreach (BsonValue email in emails)
                    {
                        EmailLIst.Add(email.AsString);
                    }
                    var EmailListstr = String.Join(",", EmailLIst);

                    BsonDocument business = InputData[i]["business"].AsBsonDocument;
                    string taxId = business["taxId"].AsString;



                    string JsonDocumentid = InputData[i]["id"].AsString;

                    var dataTable = new DataTable();
                    dataTable.Columns.Add("BusinessId", typeof(int));
                    dataTable.Columns.Add("JsonDocumentid", typeof(string));
                    dataTable.Columns.Add("DataSourceEnum", typeof(int));
                    dataTable.Columns.Add("ClientEmail", typeof(string));
                    dataTable.Columns.Add("EmailSent", typeof(bool));
                    dataTable.Columns.Add("DateEmailSent", typeof(DateTime));


                    dataTable.Rows.Add(
                        taxId,
                        JsonDocumentid,
                        1,
                        EmailListstr,
                        false,
                        null
                        );

                    var json = JsonConvert.SerializeObject(dataTable, Formatting.None);
                    var parameter = new SqlParameter("@BusinessData", SqlDbType.NVarChar)
                    {
                        Value = json
                    };
                    var UserParam = new
                    {

                        BusinessRequests = parameter.Value // retrieve the value of the parameter
                    };
                    var result = _repository.ExecuteGetSP<AddBusinessDataToSQLFromGreenINvoiceResponse>(ConstUninetStoredprocedure.SP_InsertGreenvoiceJsonDetailsIntoDB, UserParam);
                    try
                    {
                        var res = result.ToList();
                    }
                    catch(Exception ex) { }
                   
                }
                //extract documnet from uninet mongo
                return true;
            }
            catch (Exception ex) { return false; }
        }




        public async Task<List<TestResponse>> GetTestResponse()
        {
            try
            {
                List<TestResponse> res = new List<TestResponse>();
                res = _repository.ExecuteGetSP<TestResponse>("dbo.GetTestData").ToList();
                return res;
            }
            catch (Exception ex) { return null; }
           
        }
    }
}
