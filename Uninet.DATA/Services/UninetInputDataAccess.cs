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
using ThirdParty.Json.LitJson;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using System.Text.Json;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Reflection.Metadata;
using static System.Net.Mime.MediaTypeNames;

namespace Uninet.DATA.Services
{
    public class UninetInputDataAccess : IUninetInputDataAccess
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IMongoCollection<BsonDocument> _Uninetgreenvoicedocument;
        private readonly IMongoCollection<BsonDocument> _IcountCompaniesInfoCollection;
        private readonly IMongoCollection<BsonDocument> _UninetGetStaticQuestionsService;
        private readonly IMongoCollection<BsonDocument> _UninetGetLandingPageDataService;
        private readonly IMongoCollection<BsonDocument> _IcountWebhookData;
        public UninetInputDataAccess(IRepository<UninetContext> repository, IMongoClient client)//, IloginRepository loginRepository
        {
            var database = client.GetDatabase("Uninet");
            var Uninetgreenvoicedocument = database.GetCollection<BsonDocument>("UninetGreenVoiceCollection");
            _Uninetgreenvoicedocument = Uninetgreenvoicedocument;
            _repository = repository;
            var uninetGetStaticQuestionsService = database.GetCollection<BsonDocument>("UninetStaticData");
            _UninetGetStaticQuestionsService = uninetGetStaticQuestionsService;
            _IcountCompaniesInfoCollection = database.GetCollection<BsonDocument>("IcountCompanisInfo");

            var UninetGetLandingPageDataService = database.GetCollection<BsonDocument>("UninetHomepageBlocks");
            _UninetGetLandingPageDataService = UninetGetLandingPageDataService;


            _IcountWebhookData= database.GetCollection<BsonDocument>("IcountWebhookData");
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
        
        public async Task<bool> ReceiveWebhook(string json, string webhookSourceId)//
        {
            try
            {
                 //WriteToTableAsync(111, "111", "111");
                // 
                var bsonDocument = BsonDocument.Parse(json.ToString());
                var UserParam0 = new
                {
                    Taskid = 0,
                    TaskDesc = "0",
                    text = bsonDocument.ToString()
                };
                var spresult0 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam0);

                BsonValue doc_urlvalue;
                string doc_url = "";
                if (bsonDocument.TryGetValue("doc_url", out doc_urlvalue))
                {
                    doc_url = doc_urlvalue.ToString();
                    // Use vatId as needed
                }
                else
                {
                    // Check if "vat_id" is within "doc_info"
                    var docInfo = bsonDocument.GetValue("doc_info").AsBsonDocument;
                    if (docInfo.TryGetValue("doc_url", out doc_urlvalue))
                    {
                        doc_url = doc_urlvalue.ToString();
                        // Use vatId as needed
                    }
                    else
                    {
                        // Handle the case where "vat_id" is not found in either location
                        // You can add your error handling logic here.
                    }
                }
                string finalUrl = await ConvertUrl(doc_url.ToString());
                bsonDocument["doc_info"].AsBsonDocument.Add("doc_url_copy", finalUrl);
                var UserParam = new
                {
                    Taskid = 1,
                    TaskDesc = "1",
                    text = "1"
                };
                var spresult = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam);

                try
                {
                    _IcountWebhookData.InsertOne(bsonDocument);

                }
                catch (Exception ex)
                {
                    var UserParam2 = new
                    {
                        Taskid = 2,
                        TaskDesc = "2",
                        text = ex.Message
                    };
                    var spresult2 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam2);
                }






                BsonValue oidValue = bsonDocument["_id"].AsObjectId;
                string oid_value = oidValue.ToString();






                ////WriteToTableAsync(111, oid_value, oid_value);

                ObjectId objectId = ObjectId.Parse(oid_value);
                //// WriteToTableAsync(333, "333", "333");


                var filter1 = Builders<BsonDocument>.Filter.Eq("_id", objectId);
                // WriteToTableAsync(444, "444", "444");

                var existingDocument = _IcountWebhookData.Find(filter1).FirstOrDefault();
                // WriteToTableAsync(555, "555", "555");

                if (existingDocument == null)
                {

                    

                }


                var UserParam3 = new
                {
                    Taskid = 3,
                    TaskDesc = "3",
                    text = webhookSourceId
                };
                var spresult3 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam3);



                int intwebhookSourceId = Convert.ToInt32(webhookSourceId);

                var UserParam4 = new
                {
                    Taskid = intwebhookSourceId,
                    TaskDesc = webhookSourceId,
                    text = webhookSourceId
                };
                var spresult4 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam4);

                int internalCompanySenderId = _repository.GetFirstObject<LUTIcountSourceWebhookCompanyMapping>(x => x.WebHookSourceid == intwebhookSourceId).Internalcompanyid;
               
                int UserIdAttachedToCompanySenderId = _repository.GetFirstObject<Businesses>(x => x.BusinessId == internalCompanySenderId).AdminUserid;
                string OrganiztionName = _repository.GetFirstObject<Businesses>(x => x.BusinessId == internalCompanySenderId).OrganizationName;

                
                /////get businessvatid from IcountCompanisInfo
                var filter = Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", internalCompanySenderId);
                var projection = Builders<BsonDocument>.Projection.Include("company_info.vat_id").Exclude("_id");

                var result = _IcountCompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();
                string businessVatId = "";
                if (result != null)
                {
                    businessVatId = result["company_info"]["vat_id"].AsString;

                }

               

                int vatId = 0;

                // Check if "vat_id" is within the root
                BsonValue vatIdValue;
                if (bsonDocument.TryGetValue("vat_id", out vatIdValue))
                {
                    vatId = Convert.ToInt32(vatIdValue.ToString());
                    // Use vatId as needed
                }
                else
                {
                    // Check if "vat_id" is within "doc_info"
                    var docInfo = bsonDocument.GetValue("doc_info").AsBsonDocument;
                    if (docInfo.TryGetValue("vat_id", out vatIdValue))
                    {
                        vatId = Convert.ToInt32(vatIdValue.ToString());
                        // Use vatId as needed
                    }
                    else
                    {
                        // Handle the case where "vat_id" is not found in either location
                        // You can add your error handling logic here.
                    }
                }

                string clientName = "";
                BsonValue clientNameValue;
                if (bsonDocument.TryGetValue("clientname", out clientNameValue))
                {
                    clientName = clientNameValue.ToString();
                    // Use clientName as needed
                }
                else
                {
                    // Check if "clientname" is within "doc_info"
                    var docInfo = bsonDocument.GetValue("doc_info").AsBsonDocument;
                    if (docInfo.TryGetValue("client_name", out clientNameValue))
                    {
                        clientName = clientNameValue.ToString();
                        // Use clientName as needed
                    }
                    else
                    {
                        // Handle the case where "clientname" is not found in either location
                        // You can add your error handling logic here.
                    }
                }
                string email = "";




                BsonValue emailValue;
                if (bsonDocument.TryGetValue("client_email", out emailValue))
                {
                    email = emailValue.ToString();
                    // Use vatId as needed
                }
                else
                {
                    // Check if "vat_id" is within "doc_info"
                    var docInfo = bsonDocument.GetValue("doc_info").AsBsonDocument;
                    if (docInfo.TryGetValue("client_email", out emailValue))
                    {
                        email = emailValue.ToString();
                        // Use vatId as needed
                    }
                    else
                    {
                        // Handle the case where "vat_id" is not found in either location
                        // You can add your error handling logic here.
                    }
                }



                BsonValue dateissuedvalue;
                string docdateissued = "";
                if (bsonDocument.TryGetValue("dateissued", out dateissuedvalue))
                {
                    docdateissued = dateissuedvalue.ToString();
                    // Use vatId as needed
                }
                else
                {
                    // Check if "vat_id" is within "doc_info"
                    var docInfo = bsonDocument.GetValue("doc_info").AsBsonDocument;
                    if (docInfo.TryGetValue("dateissued", out dateissuedvalue))
                    {
                        docdateissued = dateissuedvalue.ToString();
                        // Use vatId as needed
                    }
                    else
                    {
                        // Handle the case where "vat_id" is not found in either location
                        // You can add your error handling logic here.
                    }
                }





                var UserParam5 = new
                {
                    Taskid = 5,
                    TaskDesc = "5",
                    text = "5"
                };
                var spresult5 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam5);


                BsonValue totalwithvatvalue;
                string totalwithvat = "";
                if (bsonDocument.TryGetValue("totalwithvat", out totalwithvatvalue))
                {
                    totalwithvat = totalwithvatvalue.ToString();
                    // Use vatId as needed
                }
                else
                {
                    // Check if "vat_id" is within "doc_info"
                    var docInfo = bsonDocument.GetValue("doc_info").AsBsonDocument;
                    if (docInfo.TryGetValue("totalwithvat", out totalwithvatvalue))
                    {
                        totalwithvat = totalwithvatvalue.ToString();
                        // Use vatId as needed
                    }
                    else
                    {
                        // Handle the case where "vat_id" is not found in either location
                        // You can add your error handling logic here.
                    }
                }










                bool ClientvatidRegisteredtOnUninet = false;


                // Find documents matching the filter
                var count = _IcountCompaniesInfoCollection.CountDocuments(filter);
                if (count > 0)
                {
                    ClientvatidRegisteredtOnUninet = true;
                }
                else
                {
                    ClientvatidRegisteredtOnUninet = false;
                }








                var newBusinessData = new BusinessData
                {
                    UserId = UserIdAttachedToCompanySenderId,
                    BusinessId = internalCompanySenderId,
                    JsonDocumentid = oid_value,
                    BusinessVatId = businessVatId,
                    ClientVat_id = vatId,
                    client_name = clientName,
                    DataSourceEnum = 2,
                    ClientEmail = email,
                    EmailSent = false,
                    DateEmailSent = null,
                    DocumentApprovedtoUninet = null,
                    supplier_name_Sender = OrganiztionName,
                    docDate = Convert.ToDateTime(docdateissued),
                    amountAV = Convert.ToDouble(totalwithvat),
                    currency_code = "ILS",     // Example value for currency_code
                    ClientvatidRegisteredtOnUninet = ClientvatidRegisteredtOnUninet,// Example value for ClientvatidRegisteredtOnUninet
                    DataSourceType = 2
                };

                var UserParam6 = new
                {
                    Taskid = 6,
                    TaskDesc = "6",
                    text = "6"
                };
                var spresult6 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam6);


                // Create a predicate to check for the existence of the record
                Expression<Func<BusinessData, bool>> predicate = bd =>
                    bd.UserId == UserIdAttachedToCompanySenderId &&
                    bd.BusinessId == Convert.ToInt32(businessVatId) &&
                    bd.JsonDocumentid == oid_value;

                // Use the GetFirstObject method to check if the record exists
                var existingBusinessData = _repository.GetFirstObject(predicate);
                if (existingBusinessData == null)
                {
                    var UserParam7 = new
                    {
                        Taskid = 7,
                        TaskDesc = "7",
                        text = "7"
                    };
                    var spresult7 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam7);

                    _repository.Create<BusinessData>(newBusinessData);
                    var UserParam8 = new
                    {
                        Taskid = 8,
                        TaskDesc = "8",
                        text = "8"
                    };
                    var spresult8 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam8);

                }


                return true;



            }
            catch (Exception ex)
            {
                var UserParam = new
                {
                    Taskid = 20,
                    TaskDesc = "20",
                    text = ex.Message +ex.InnerException
                };


                var spresult = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam); 
                return false; }
        }

        public async Task<string> ConvertUrl(string Inputurl)
        {
            string finalUrl = "";
            using (var httpClient = new HttpClient())
            {
                //string url = "https://app.icount.co.il/hash/p_print.php?code=MXVNUmk2WWY4bDUvQ2JYVHcwUlIxZm8rSnRJWlh3TGRramJwQUlqbUFVa2JrMXhQekJ3eHR3PT0%3D";
                // Send an HTTP GET request to the original URL
                HttpResponseMessage response = await httpClient.GetAsync(Inputurl);

                // Check if the request was successful
                if (response.IsSuccessStatusCode)
                {
                    // Get the final URL from the response
                    finalUrl = response.RequestMessage.RequestUri.ToString();

                }


            }
            return finalUrl;
        }

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




        public async Task<string> SendRequest(string endpointUrl, HttpMethod method, string jwtToken = null)
        {
            string result = "";
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(method, endpointUrl);

                // Add authorization header if jwtToken is provided
                if (!string.IsNullOrEmpty(jwtToken))
                {
                    request.Headers.Add("Authorization", $"Bearer {jwtToken}");
                }

                HttpResponseMessage response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode(); // Throw an exception if the request is not successful

                string responseData = await response.Content.ReadAsStringAsync();
                result = responseData;
                
            }

            return result;
        }






        //public async Task<string> PullUserDatafromExternalSystem(int Userid)
        //{


        //    List<Businesses> res = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == Userid);



        //    //on this res i have the list of businesses realted to this user 
        //    foreach (var Business in res)
        //    {
        //        //for each bussines i need to get their api key and send request to get jwt token
        //        //here i will simulate getting the jwt token 
        //        //string Jwtsimlulation= SimulateToken();//remark this meanwhile

        //        //now get the end point url for this businessId
        //        //here comes the logic that decide what apiid to callto for this example we will use 27 -- /api/v1/documents/{id}
        //        //var Endpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 27);////remark this meanwhile
        //        //string StrEndpoint = "https://private-anon-5e08cc171e-greeninvoice.apiary-mock.com" + Endpoint; ////remark this meanwhile


        //        //need to call a generic function that get the end point ,jwt token ,method (put get post ) and send the request 

        //        //HttpMethod method = new HttpMethod(Endpoint.MethodeType);////remark this meanwhile
        //        //here i call a function that will call real external system
        //        // string jsonfromExternalServiceRsult =await SendRequest(StrEndpoint, method, Jwtsimlulation);////remark this meanwhile
        //        //i need to remark this code until i will have a real api from morning or any other external service


        //        //from here i call meanwhile greenvoice controller named PullBusinessDataByTaxid
        //        using var client = new System.Net.Http.HttpClient();

        //        // Send an HTTP GET request to the specified URL
        //        var response = await client.GetAsync("https://localhost:7285/api/GreenInvoice/PullBusinessDataByTaxid/" + Business.BusinessId );


        //        // Read the response content as a string
        //        var json = await response.Content.ReadAsStringAsync();

        //        // Deserialize the string into a BsonArray
        //        BsonArray bsonArray = BsonSerializer.Deserialize<BsonArray>(json);

        //        // Convert the BsonArray to a list of BsonDocuments
        //        List<BsonDocument> bsonDocuments = new List<BsonDocument>();
        //        foreach (BsonValue bsonValue in bsonArray)
        //        {
        //            BsonDocument bsonDocument = bsonValue.ToBsonDocument();
        //            bsonDocuments.Add(bsonDocument);
        //        }


        //        ///save json into MongoDb Collection
        //        var SavingToUninetGreenvoiceCollection = SavegreenvoicedocumentIntoUninet(bsonDocuments);





        //    }
        //    return string.Empty;
        //}




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
