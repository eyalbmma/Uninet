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
using System.Globalization;
using Uninet.Domain.Classes;

namespace Uninet.DATA.Services
{
    public class UninetInputDataAccess : IUninetInputDataAccess
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IMongoCollection<BsonDocument> _Uninetgreenvoicedocument;
        private readonly IMongoCollection<BsonDocument> _IcountCompaniesInfoCollection;
        private readonly IMongoCollection<BsonDocument> _MorningCompanisInfoCollection;
        private readonly IMongoCollection<BsonDocument> _UninetGetStaticQuestionsService;
        private readonly IMongoCollection<BsonDocument> _UninetGetLandingPageDataService;
        private readonly IMongoCollection<BsonDocument> _IcountWebhookData;
        private readonly IMongoCollection<BsonDocument> _MorningWebhookData;
        private readonly IDataMailassist _dataMailassist;
        public UninetInputDataAccess(IRepository<UninetContext> repository, IMongoClient client, IDataMailassist dataMailassist)//, IloginRepository loginRepository
        {
            var database = client.GetDatabase("Uninet");
            var Uninetgreenvoicedocument = database.GetCollection<BsonDocument>("UninetGreenVoiceCollection");
            _Uninetgreenvoicedocument = Uninetgreenvoicedocument;
            _repository = repository;
            var uninetGetStaticQuestionsService = database.GetCollection<BsonDocument>("UninetStaticData");
            _UninetGetStaticQuestionsService = uninetGetStaticQuestionsService;
            _IcountCompaniesInfoCollection = database.GetCollection<BsonDocument>("IcountCompanisInfo");
            _MorningCompanisInfoCollection= database.GetCollection<BsonDocument>("MorningCompanisInfo");
            var UninetGetLandingPageDataService = database.GetCollection<BsonDocument>("UninetHomepageBlocks");
            _UninetGetLandingPageDataService = UninetGetLandingPageDataService;


            _IcountWebhookData= database.GetCollection<BsonDocument>("IcountWebhookData");


            _MorningWebhookData = database.GetCollection<BsonDocument>("MorningWebHookData");

            _dataMailassist = dataMailassist;
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
        private async Task<string> FetchAndUpdateNewToken(int companyId, int subcompanyId, int userId, int externalSystemId, UsersExternalSystemDynamicFields usersExternalSystemDynamicFieldsResult)
        {
            try
            {
                var tokenCompanyInfoEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 88);
                if (tokenCompanyInfoEndpoint == null)
                {
                    throw new Exception("Token endpoint information not found.");
                }

                string apiTokenValue = "";
                string secretKeyValue = "";

                // Retrieve ApiToken and SecretKey values from the database
                var credentials = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                    x => x.Companyid == companyId && x.Userid == userId && x.SubCompayId == subcompanyId && x.ExternalSystemId == externalSystemId
                );

                foreach (var item in credentials)
                {
                    if (item.FieldLabelName == "ApiToken")
                    {
                        apiTokenValue = item.FieldLabelValue;
                    }
                    else if (item.FieldLabelName == "SecretKey")
                    {
                        secretKeyValue = item.FieldLabelValue;
                    }
                }

                // Step 4: Request a new token
                var payload = new
                {
                    id = apiTokenValue,
                    secret = secretKeyValue
                };
                string jsonPayload = JsonConvert.SerializeObject(payload);

                // Send the request
                Console.WriteLine($"Sending token request to: {tokenCompanyInfoEndpoint.Endpoint}");
                var responseCompanyInfo = await SendRequest(tokenCompanyInfoEndpoint.Endpoint, HttpMethod.Post,null, jsonPayload);

                // Parse the response to extract the token and expiration time
                var jsonResponse = JObject.Parse(responseCompanyInfo);
                string newToken = jsonResponse["token"].ToString();
                long expires = (long)jsonResponse["expires"];
                DateTime newExpirationDate = DateTimeOffset.FromUnixTimeSeconds(expires).UtcDateTime;

                // Step 5: Update the token and expiration date in the database
                if (usersExternalSystemDynamicFieldsResult != null)
                {
                    usersExternalSystemDynamicFieldsResult.Token = newToken;
                    usersExternalSystemDynamicFieldsResult.TokenExpiration = newExpirationDate.ToString("o"); // ISO 8601 format
                    await _repository.UpdateAsync(usersExternalSystemDynamicFieldsResult);
                }

                return newToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchAndUpdateNewToken: {ex.Message}");
                return "";
            }
        }
        private bool IsTokenExpired(string tokenExpiration)
        {
            try
            {
                // Parse the ISO 8601 date format directly
                DateTime expirationDate = DateTime.Parse(tokenExpiration, null, DateTimeStyles.RoundtripKind);
                return DateTime.UtcNow >= expirationDate;
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Failed to parse TokenExpiration: {ex.Message}");
                return true; // Treat as expired if parsing fails
            }
        }

        public async Task<string> GetNewToken(int companyId, int subcompanyId, int userId, int externalSystemId)
        {
            try
            {
                string newToken = "";

                // Step 1: Retrieve the token information from the database
                var usersExternalSystemDynamicFieldsResult = await _repository.GetFirstObjectAsync<UsersExternalSystemDynamicFields>(
                    x => x.Companyid == companyId && x.Userid == userId && x.SubCompayId == subcompanyId && x.ExternalSystemId == externalSystemId
                );

                if (usersExternalSystemDynamicFieldsResult == null || IsTokenExpired(usersExternalSystemDynamicFieldsResult.TokenExpiration))
                {
                    // Call the helper function to fetch and update the token
                    newToken = await FetchAndUpdateNewToken(companyId, subcompanyId, userId, externalSystemId, usersExternalSystemDynamicFieldsResult);
                }
                else
                {
                    // If the token is valid, return it
                    newToken = usersExternalSystemDynamicFieldsResult.Token;
                }

                return newToken;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetNewToken: {ex.Message}");
                return "";
            }
        }
        public async Task<string> GetNameById(string jsonString, long idToFind)
        {
            // Parse the JSON string into a list of dictionaries
            List<Dictionary<string, object>> data = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(jsonString);

            // Find the object with the matching id
            var matchingObject = data.FirstOrDefault(obj => (long)obj["id"] == idToFind);

            // Return the name if found, otherwise return an empty string
            return matchingObject != null ? matchingObject["name"].ToString() : string.Empty;
        }
        public async Task<bool> MorningReceiveWebhook(string json, string WebHookSourceid)
        {
            try
            {
                // Parse WebHookSourceId to extract SubCompanyId and SourceId
                string SubCompanyid = WebHookSourceid.Split("_")[1];
                string Str_WebHookSourceid = WebHookSourceid.Split("_")[0];
                int intWebHookSourceid = Convert.ToInt32(Str_WebHookSourceid);
                var internalCompanySenderIdObj = await _repository.GetFirstObjectAsync<LUTIcountSourceWebhookCompanyMapping>(
                    x => x.WebHookSourceid == intWebHookSourceid && x.SubCompanyId == Convert.ToInt32(SubCompanyid)
                );
                var UserIdAttachedToCompanySenderIdObj = await _repository.GetFirstObjectAsync<Businesses>(
                    x => x.BusinessId == internalCompanySenderIdObj.Internalcompanyid
                );
                // Parse the incoming JSON
                var bsonDocument = BsonDocument.Parse(json);

                // Extract required fields
                string type = bsonDocument.GetValue("type", "").ToString();

                // Fetch typename from the Morning API based on `type`
                string newToken = await GetNewToken(internalCompanySenderIdObj.Internalcompanyid, Convert.ToInt32(SubCompanyid) ,UserIdAttachedToCompanySenderIdObj.AdminUserid ,6);//internalCompanyId, Client_SubCompanyid, userId, 6
                var EndpointDocInfoObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 92); // Endpoint for typename
                string DocInfoEndpoint = $"{EndpointDocInfoObj.Endpoint}";
                string typenameResponse = await SendRequest(DocInfoEndpoint, HttpMethod.Get, newToken);
                string typename = await GetNameById(typenameResponse,Convert.ToInt64(type));

                // Add typename to the BSON document
                bsonDocument.Add("typename", typename);

                // Parse remaining fields from the BSON document
                string businessId = bsonDocument.GetValue("businessId", "").ToString();
                string date = bsonDocument.GetValue("date", "").ToString();
                string total = bsonDocument.GetValue("total", "").ToString();

                var recipient = bsonDocument.GetValue("recipient", new BsonDocument()).AsBsonDocument;
                string recipientName = recipient.GetValue("name", "").ToString();
                string recipientTaxId = recipient.GetValue("taxId", "").ToString();
                string recipientEmail = recipient.GetValue("emails", new BsonArray())
                    .AsBsonArray.FirstOrDefault()?.ToString();

                var files = bsonDocument.GetValue("files", new BsonDocument()).AsBsonDocument;
                var downloadLinks = files.GetValue("downloadLinks", new BsonDocument()).AsBsonDocument;
                string docUrl = downloadLinks.Contains("origin") ? downloadLinks["origin"].ToString() : string.Empty;

                string finalUrl = await ConvertMorningUrl(docUrl);

                // Update or add the "processedUrl" field in downloadLinks
                if (!downloadLinks.Contains("processedUrl"))
                {
                    downloadLinks.Add("processedUrl", finalUrl);
                }
                else
                {
                    downloadLinks["processedUrl"] = finalUrl;
                }

                // Insert the updated BSON document into the webhook collection
                await _MorningWebhookData.InsertOneAsync(bsonDocument);

                // Parse WebHookSourceId to retrieve mappings
                

                
                int internalCompanySenderId = internalCompanySenderIdObj.Internalcompanyid;

                
                int UserIdAttachedToCompanySenderId = UserIdAttachedToCompanySenderIdObj.AdminUserid;

                var OrganiztionNameObj = await _repository.GetFirstObjectAsync<Businesses>(
                    x => x.BusinessId == internalCompanySenderId
                );
                string OrganiztionName = OrganiztionNameObj.OrganizationName;

                // Check if the business is registered in MorningCompanisInfo
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("InternalCompanyId", internalCompanySenderId),
                    Builders<BsonDocument>.Filter.Eq("SubCompanyId", Convert.ToInt32(SubCompanyid))
                );

                var projection = Builders<BsonDocument>.Projection.Include("taxId").Exclude("_id");
                var resultMorningCompanyinfo = _MorningCompanisInfoCollection.Find(filter).Project(projection).FirstOrDefault();

                string businessVatId = resultMorningCompanyinfo?["taxId"].AsString ?? string.Empty;
                bool isRegisteredOnUninet = _MorningCompanisInfoCollection.CountDocuments(filter) > 0;

                // Prepare BusinessData object
                var newBusinessData = new BusinessData
                {
                    UserId = UserIdAttachedToCompanySenderId,
                    BusinessId = internalCompanySenderId,
                    SubCompanyId = Convert.ToInt32(SubCompanyid),
                    JsonDocumentid = bsonDocument["_id"].AsObjectId.ToString(),
                    BusinessVatId = businessVatId,
                    ClientVat_id = Convert.ToInt32(recipientTaxId),
                    client_name = recipientName,
                    DataSourceEnum = 6, // 6 for Morning
                    ClientEmail = recipientEmail,
                    EmailSent = false,
                    DateEmailSent = null,
                    DocumentApprovedtoUninet = null,
                    supplier_name_Sender = OrganiztionName,
                    docDate = Convert.ToDateTime(date),
                    amountAV = Convert.ToDouble(total),
                    currency_code = bsonDocument.GetValue("currency", "").ToString(),
                    ClientvatidRegisteredtOnUninet = isRegisteredOnUninet,
                    DataSourceType = 2 // for webhook data collection
                };

                // Check for existing BusinessData object
                Expression<Func<BusinessData, bool>> predicate = bd =>
                    bd.UserId == UserIdAttachedToCompanySenderId &&
                    bd.BusinessId == internalCompanySenderId &&
                    bd.SubCompanyId == Convert.ToInt32(SubCompanyid) &&
                    bd.JsonDocumentid == ObjectId.Parse(newBusinessData.JsonDocumentid).ToString();

                var existingBusinessData = await _repository.GetFirstObjectAsync(predicate);
                if (existingBusinessData == null)
                {
                    _repository.Create<BusinessData>(newBusinessData);

                    var _RequestMailObject = new RequestedMailObject
                    {
                        Sendername = OrganiztionName,
                        DocType = "Document",
                        RecipientName = recipientName,
                        DocLink = finalUrl
                    };

                    await _dataMailassist.sendsmtpmail(
                        "UNINET מסמך הגיע אליך מ ",
                        "eyalbmma@gmail.com",
                        recipientEmail,
                        isRegisteredOnUninet ? 7 : 5,
                        1,
                        _RequestMailObject,
                        UserIdAttachedToCompanySenderId.ToString(),
                        newBusinessData.JsonDocumentid
                    );
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in MorningReceiveWebhook: {ex.Message}");
                return false;
            }
        }
        private string ExtractTypenameFromResponse(string response)
        {
            try
            {
                var jsonDocument = JsonDocument.Parse(response);
                var paymentPluginArray = jsonDocument.RootElement.GetProperty("paymentPlugins");

                // Extract the `friendlyName` of the first plugin
                if (paymentPluginArray.GetArrayLength() > 0)
                {
                    var firstPlugin = paymentPluginArray[0];
                    return firstPlugin.GetProperty("friendlyName").GetString();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting typename: {ex.Message}");
            }

            return "Unknown";
        }

        //public async Task<bool> MorningReceiveWebhook(string json, string WebHookSourceid)
        //{
        //    try
        //    {
        //        string SubCompanyid = WebHookSourceid.Split("_")[1];
        //        string Str_WebHookSourceid = WebHookSourceid.Split("_")[0];

        //        var bsonDocument = BsonDocument.Parse(json);

        //        string businessId = bsonDocument.GetValue("businessId", "").ToString();
        //        string businessType = bsonDocument.GetValue("businessType", "").ToString();
        //        string currency = bsonDocument.GetValue("currency", "").ToString();
        //        string country = bsonDocument.GetValue("country", "").ToString();
        //        string date = bsonDocument.GetValue("date", "").ToString();
        //        string total = bsonDocument.GetValue("total", "").ToString();
        //        string description = bsonDocument.GetValue("description", "").ToString();
        //        string remarks = bsonDocument.GetValue("remarks", "").ToString();

        //        var recipient = bsonDocument.GetValue("recipient", new BsonDocument()).AsBsonDocument;
        //        string recipientName = recipient.GetValue("name", "").ToString();
        //        string recipientTaxId = recipient.GetValue("taxId", "").ToString(); // Extract the taxId

        //        string recipientEmail = recipient.GetValue("emails", new BsonArray())
        //                        .AsBsonArray.FirstOrDefault()?.ToString();

        //        var files = bsonDocument.GetValue("files", new BsonDocument()).AsBsonDocument;
        //        var downloadLinks = files.GetValue("downloadLinks", new BsonDocument()).AsBsonDocument;
        //        string docUrl = downloadLinks.Contains("origin") ? downloadLinks["origin"].ToString() : string.Empty;

        //        string finalUrl = await ConvertMorningUrl(docUrl);

        //        if (!downloadLinks.Contains("processedUrl"))
        //        {
        //            // Add "processedUrl" if it doesn't exist
        //            downloadLinks.Add("processedUrl", finalUrl);
        //        }
        //        else
        //        {
        //            // Update the value of "processedUrl" if it exists
        //            downloadLinks["processedUrl"] = finalUrl;
        //        }

        //        try
        //        {
        //            await _MorningWebhookData.InsertOneAsync(bsonDocument);
        //        }
        //        catch (Exception)
        //        {
        //            // Handle insert exceptions if needed
        //        }

        //        // Parse the WebHookSourceId and retrieve related mappings
        //        int intWebHookSourceid = Convert.ToInt32(Str_WebHookSourceid);

        //        var internalCompanySenderIdObj = await _repository.GetFirstObjectAsync<LUTIcountSourceWebhookCompanyMapping>(
        //            x => x.WebHookSourceid == intWebHookSourceid && x.SubCompanyId == Convert.ToInt32(SubCompanyid)
        //        );
        //        int internalCompanySenderId = internalCompanySenderIdObj.Internalcompanyid;

        //        var UserIdAttachedToCompanySenderIdObj = await _repository.GetFirstObjectAsync<Businesses>(
        //            x => x.BusinessId == internalCompanySenderId
        //        );
        //        int UserIdAttachedToCompanySenderId = UserIdAttachedToCompanySenderIdObj.AdminUserid;

        //        var OrganiztionNameObj = await _repository.GetFirstObjectAsync<Businesses>(
        //            x => x.BusinessId == internalCompanySenderId
        //        );
        //        string OrganiztionName = OrganiztionNameObj.OrganizationName;

        //        // Check if the business is registered
        //        var filter = Builders<BsonDocument>.Filter.And(
        //            Builders<BsonDocument>.Filter.Eq("InternalCompanyId", internalCompanySenderId),
        //            Builders<BsonDocument>.Filter.Eq("SubCompanyId", Convert.ToInt32(SubCompanyid))
        //        );

        //        var projection = Builders<BsonDocument>.Projection.Include("taxId").Exclude("_id");

        //        var resultMorningCompanyinfo = _MorningCompanisInfoCollection.Find(filter).Project(projection).FirstOrDefault();




        //        string businessVatId = "";
        //        if (resultMorningCompanyinfo != null)
        //        {
        //            businessVatId = resultMorningCompanyinfo["taxId"].AsString;
        //        }



        //        bool isRegisteredOnUninet = _MorningCompanisInfoCollection.CountDocuments(filter) > 0;

        //        // Prepare BusinessData object
        //        var newBusinessData = new BusinessData
        //        {
        //            UserId = UserIdAttachedToCompanySenderId,
        //            BusinessId = internalCompanySenderId,
        //            SubCompanyId = Convert.ToInt32(SubCompanyid),
        //            JsonDocumentid = bsonDocument["_id"].AsObjectId.ToString(),
        //            BusinessVatId = businessVatId,
        //            ClientVat_id = Convert.ToInt32(recipientTaxId),
        //            client_name = recipientName,
        //            DataSourceEnum = 6,//6 for morning 2 for icount
        //            ClientEmail = recipientEmail,
        //            EmailSent = false,
        //            DateEmailSent = null,
        //            DocumentApprovedtoUninet = null,
        //            supplier_name_Sender = OrganiztionName,
        //            docDate = Convert.ToDateTime(date),
        //            amountAV = Convert.ToDouble(total),
        //            currency_code = currency,
        //            ClientvatidRegisteredtOnUninet = isRegisteredOnUninet,
        //            DataSourceType = 2 //for webhook data collection
        //        };

        //        // Check if the BusinessData object already exists
        //        //Expression<Func<BusinessData, bool>> predicate = bd =>
        //        //    bd.UserId == UserIdAttachedToCompanySenderId &&
        //        //    bd.BusinessId == internalCompanySenderId &&
        //        //    bd.SubCompanyId == Convert.ToInt32(SubCompanyid) &&
        //        //    bd.JsonDocumentid == newBusinessData.JsonDocumentid;


        //        Expression<Func<BusinessData, bool>> predicate = bd =>
        //        bd.UserId == UserIdAttachedToCompanySenderId &&
        //        bd.BusinessId == internalCompanySenderId &&
        //        bd.SubCompanyId == Convert.ToInt32(SubCompanyid) &&
        //        bd.JsonDocumentid == ObjectId.Parse(newBusinessData.JsonDocumentid).ToString();



        //        var existingBusinessData = await _repository.GetFirstObjectAsync(predicate);
        //        if (existingBusinessData == null)
        //        {
        //            _repository.Create<BusinessData>(newBusinessData);

        //            var _RequestMailObject = new RequestedMailObject
        //            {
        //                Sendername = OrganiztionName,
        //                DocType = "Document",
        //                RecipientName = recipientName,
        //                DocLink = finalUrl
        //            };

        //            await _dataMailassist.sendsmtpmail(
        //                " UNINET מסמך הגיע אליך מ  ",
        //                "eyalbmma@gmail.com",
        //                recipientEmail,
        //                isRegisteredOnUninet ? 7 : 5,
        //                1,
        //                _RequestMailObject,
        //                UserIdAttachedToCompanySenderId.ToString(),
        //                newBusinessData.JsonDocumentid
        //            );
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log the exception
        //        Console.WriteLine($"Error in MorningReceiveWebhook: {ex.Message}");
        //        return false;
        //    }
        //}



        public async Task<bool> ReceiveWebhook(string json, string WebHookSourceid)//
        {
            try
            {
                string SubCompanyid= WebHookSourceid.Split("_")[1];
                string Str_WebHookSourceid = WebHookSourceid.Split("_")[0];
               
                var bsonDocument = BsonDocument.Parse(json.ToString());
                ////add eyal logic need to get only json document with this doctype format 
                ///reciept,invoice,deal,invrec,order,refund,delcert
                ///// Define a list of allowed document types
                var allowedDocTypes = new HashSet<string> { "receipt", "invoice", "deal", "invrec", "order", "refund", "delcert" };
                BsonValue doc_typevalue;
                string doc_type = "";
                if (bsonDocument.TryGetValue("doctype", out doc_typevalue))
                {
                    doc_type = doc_typevalue.ToString().ToLower();
                    // Use vatId as needed
                }

                if (allowedDocTypes.Contains(doc_type))
                {

                    //var UserParam0 = new
                    //{
                    //    Taskid = -1,
                    //    TaskDesc = "SubCompanyid"+ SubCompanyid+ " Str_WebHookSourceid " + Str_WebHookSourceid,
                    //    text = json
                    //};
                    //var spresult0 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam0);

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


                    //var UserParam = new
                    //{
                    //    Taskid = 1,
                    //    TaskDesc = "1",
                    //    text = "1"
                    //};
                    //var spresult = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam);

                    try
                    {
                        _IcountWebhookData.InsertOne(bsonDocument);

                    }
                    catch (Exception ex)
                    {
                        //var UserParam2 = new
                        //{
                        //    Taskid = 2,
                        //    TaskDesc = "2",
                        //    text = ex.Message
                        //};
                        //var spresult2 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam2);
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


                    //var UserParam3 = new
                    //{
                    //    Taskid = 3,
                    //    TaskDesc = "3",
                    //    text = Str_WebHookSourceid
                    //};
                    //var spresult3 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam3);



                    int intWebHookSourceid = Convert.ToInt32(Str_WebHookSourceid);

                    //var UserParam4 = new
                    //{
                    //    Taskid = 4,
                    //    TaskDesc = Str_WebHookSourceid,
                    //    text = Str_WebHookSourceid
                    //};
                    //var spresult4 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam4);
                    //remark eyal the resul of this line businessVatId = result["company_info"]["vat_id"].AsString; is 540270165 which is wrong
                    //because we take this filter 
                    //var filter = Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", internalCompanySenderId);
                    //where internalCompanySenderId is 1372
                    ////but we have two lines with 1372 in _IcountCompaniesInfoCollection one with "SubCompanyId": 13 and the other with 
                    /// "SubCompanyId": 5 and we nee to filter with "SubCompanyId": 13 
                    ///
                    var internalCompanySenderIdObj = await _repository.GetFirstObjectAsync<LUTIcountSourceWebhookCompanyMapping>(x => x.WebHookSourceid == intWebHookSourceid && x.SubCompanyId == Convert.ToInt32(SubCompanyid));
                    int internalCompanySenderId = internalCompanySenderIdObj.Internalcompanyid;
                    var UserIdAttachedToCompanySenderIdObj= await _repository.GetFirstObjectAsync<Businesses>(x => x.BusinessId == internalCompanySenderId);
                    int UserIdAttachedToCompanySenderId = UserIdAttachedToCompanySenderIdObj.AdminUserid;


                    var OrganiztionNameObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.BusinessId == internalCompanySenderId);
                    string OrganiztionName = OrganiztionNameObj.OrganizationName;


                    /////get businessvatid from IcountCompanisInfo
                                    var filter = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", internalCompanySenderId),
                        Builders<BsonDocument>.Filter.Eq("company_info.SubCompanyId", Convert.ToInt32(SubCompanyid))
                    );
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






                    BsonValue totalwithnicuivalue;
                    string totalwithnicui = "";
                    if (bsonDocument.TryGetValue("totalwithnicui", out totalwithnicuivalue))
                    {
                        totalwithnicui = totalwithnicuivalue.ToString();
                        // Use vatId as needed
                    }
                    else
                    {
                        // Check if "vat_id" is within "doc_info"
                        var docInfo = bsonDocument.GetValue("doc_info").AsBsonDocument;
                        if (docInfo.TryGetValue("totalwithnicui", out totalwithnicuivalue))
                        {
                            totalwithnicui = totalwithnicuivalue.ToString();
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



                    //var UserParam5 = new
                    //{
                    //    Taskid = 5,
                    //    TaskDesc = "5",
                    //    text = "5"
                    //};
                    //var spresult5 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam5);






                    var newBusinessData = new BusinessData
                    {
                        UserId = UserIdAttachedToCompanySenderId,
                        BusinessId = internalCompanySenderId,
                        SubCompanyId= Convert.ToInt32(SubCompanyid),
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
                        amountAV = Convert.ToDouble(totalwithvat == "" ? totalwithnicui : totalwithvat),
                        currency_code = "ILS",     // Example value for currency_code
                        ClientvatidRegisteredtOnUninet = ClientvatidRegisteredtOnUninet,// Example value for ClientvatidRegisteredtOnUninet
                        DataSourceType = 2
                    };

                    //var UserParam6 = new
                    //{
                    //    Taskid = 6,
                    //    TaskDesc = "6",
                    //    text = "6"
                    //};
                    //var spresult6 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam6);


                    // Create a predicate to check for the existence of the record
                    Expression<Func<BusinessData, bool>> predicate = bd =>
                        bd.UserId == UserIdAttachedToCompanySenderId &&
                        bd.BusinessId == Convert.ToInt32(businessVatId) && bd.SubCompanyId== Convert.ToInt32(SubCompanyid) &&
                        bd.JsonDocumentid == oid_value;

                    // Use the GetFirstObject method to check if the record exists
                    var existingBusinessData = await _repository.GetFirstObjectAsync(predicate);
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

                        string Doctype = "";
                        BsonValue DoctypeValue;
                        if (bsonDocument.TryGetValue("doctype", out DoctypeValue))
                        {
                            Doctype = DoctypeValue.ToString();
                            // Use clientName as needed
                        }
                        var _RequestMailObject = new RequestedMailObject
                        {
                            Sendername = OrganiztionName,
                            DocType = Doctype,
                            RecipientName = clientName,
                            DocLink = finalUrl
                        };
                        //UserIdAttachedToCompanySenderId
                        var resmail = await _dataMailassist.sendsmtpmail(" UNINET מסמך הגיע אליך מ  ", "eyalbmma@gmail.com", email, ClientvatidRegisteredtOnUninet == true ? 7 : 5, 1, _RequestMailObject, UserIdAttachedToCompanySenderId.ToString(), oid_value);


                    }
                    return true;
                }
                else
                {
                    return false;
                }


                



            }
            catch (Exception ex)
            {
                var UserParam = new
                {
                    Taskid = 21,
                    TaskDesc = "21",
                    text = ex.Message + ex.InnerException,
                    StackTrace = ex.StackTrace  // Add the stack trace information
                };

                var spresult = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam);
                return false;
            }

        }


        
        public async Task<string> ConvertMorningUrl(string Inputurl)
        {
            string JsonDocUrl = "";
            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(Inputurl);

            if (response.IsSuccessStatusCode)
            {
                // Read the file content as a byte array
                var fileBytes = await response.Content.ReadAsByteArrayAsync();

                // Convert the file content to Base64 and create a data URL
                var base64String = Convert.ToBase64String(fileBytes);
                JsonDocUrl = $"data:application/pdf;base64,{base64String}";
            }
            else
            {
                JsonDocUrl = string.Empty; // Set to empty if the fetch fails
            }
            return JsonDocUrl;
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




        public async Task<string> SendRequest(string endpointUrl, HttpMethod method, string jwtToken = null, string jsonBody = null)
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

                // Add JSON payload if provided (for POST or PUT requests)
                if (!string.IsNullOrEmpty(jsonBody) && (method == HttpMethod.Post || method == HttpMethod.Put))
                {
                    request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
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
        //        //var Endpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 27);////remark this meanwhile
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
                    var result =await _repository.ExecuteGetSPAsync<AddBusinessDataToSQLFromGreenINvoiceResponse>(ConstUninetStoredprocedure.SP_InsertGreenvoiceJsonDetailsIntoDB, UserParam);
                    try
                    {
                        var res = result;
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
