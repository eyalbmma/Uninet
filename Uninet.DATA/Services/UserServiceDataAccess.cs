using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualBasic.FileIO;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.TwiML.Messaging;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Classes;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Requests;
using Uninet.Domain.StoredProcedures.Responses;
using static System.Net.WebRequestMethods;

namespace Uninet.DATA.Services
{
    public class UserServiceDataAccess: IUserServiceDataAccess
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IDataMailassist _dataMailassist;
        private readonly IMongoCollection<BsonDocument> _ICountCollection;
        private readonly IMongoCollection<BsonDocument> _ICountDocInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountClientInfoCollection;
        private readonly IMongoCollection<BsonDocument> _ICountCompanyInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountClientSuppliersCollection;
       
        private readonly IUninetInputDataAccess _UninetInputDataAccess;
        public IConfiguration Configuration { get; }
        public UserServiceDataAccess(IRepository<UninetContext> repository, IConfiguration configuration, IDataMailassist dataMailassist, IUninetInputDataAccess uninetInputDataAccess, IMongoClient client)//, IloginRepository loginRepository
        {

            var database = client.GetDatabase("Uninet");
            _ICountCollection = database.GetCollection<BsonDocument>("Icount");
            _ICountCompanyInfoCollection = database.GetCollection<BsonDocument>("IcountCompanisInfo");
            _ICountDocInfoCollection = database.GetCollection<BsonDocument>("IcountDocInfo");
            _IcountClientInfoCollection= database.GetCollection<BsonDocument>("icountClientInfo");
            _IcountClientSuppliersCollection = database.GetCollection<BsonDocument>("icountClientSuppliers");
            _repository = repository;
            Configuration = configuration;
            _dataMailassist = dataMailassist;
            _UninetInputDataAccess = uninetInputDataAccess;
            
        }

        protected string Generate_otp()
        {
            char[] charArr = "0123456789".ToCharArray();
            string strrandom = string.Empty;
            Random objran = new Random();
            for (int i = 0; i < 4; i++)
            {
                //It will not allow Repetation of Characters
                int pos = objran.Next(1, charArr.Length);
                if (!strrandom.Contains(charArr.GetValue(pos).ToString())) strrandom += charArr.GetValue(pos);
                else i--;
            }
            return strrandom;
        }



public static T ExtractPropertyValue<T>(string jsonString, string propertyPath)
    {
        JsonDocument jsonDocument = JsonDocument.Parse(jsonString);
        JsonElement propertyElement = jsonDocument.RootElement;

        // Traverse the property path to reach the desired property
        foreach (var propertyName in propertyPath.Split('.'))
        {
            if (propertyElement.TryGetProperty(propertyName, out var nextPropertyElement))
            {
                propertyElement = nextPropertyElement;
            }
            else
            {
                throw new ArgumentException($"Property '{propertyPath}' not found in the JSON.");
            }
        }

        // Convert and return the property value
        return propertyElement.ValueKind switch
        {
            JsonValueKind.String => propertyElement.GetString() != null ? (T)Convert.ChangeType(propertyElement.GetString(), typeof(T)) : default,
            JsonValueKind.Number => (T)Convert.ChangeType(propertyElement.GetDouble(), typeof(T)),
            JsonValueKind.True => (T)Convert.ChangeType(true, typeof(T)),
            JsonValueKind.False => (T)Convert.ChangeType(false, typeof(T)),
            _ => throw new ArgumentException($"Property '{propertyPath}' cannot be converted to type {typeof(T).Name}.")
        };
    }
        public enum MongoDbDestination
        {
            DocinfoDB,
            ClientInfoDB
        }

        public async void InsertDocumentInfo(JsonElement jsonData, MongoDbDestination destination,string SupplierVat_id)
        {
            // Convert the JsonElement to a BsonDocument
            BsonDocument document = BsonDocument.Parse(jsonData.GetRawText());

            // Check if the document already exists in the collection
            if (destination == MongoDbDestination.DocinfoDB)
            {
                var docnum = document["docnum"];
                var vatId = document["doc_info"]["vat_id"];
                var filter = Builders<BsonDocument>.Filter.And(
                     Builders<BsonDocument>.Filter.Eq("docnum", docnum),
                     Builders<BsonDocument>.Filter.Eq("doc_info.vat_id", vatId)
                 );

                var temp_url = document["doc_info"]["doc_url"];

                string finalUrl = await ConvertUrl(temp_url.ToString());
                document["doc_info"].AsBsonDocument.Add("doc_url_copy", finalUrl);

                var existingDocument = _ICountDocInfoCollection.Find(filter).FirstOrDefault();
                if (existingDocument == null)
                {
                    // Insert the document into the collection
                    _ICountDocInfoCollection.InsertOne(document);
                }
                else
                {
                    // Document already exists, handle the case accordingly
                    // For example, you can update the existing document or log an error
                    Console.WriteLine($"Document with docnum '{docnum}' already exists.");
                }
            }
            else if (destination == MongoDbDestination.ClientInfoDB)
            {
                var clientInfo = document["client_info"];
                var client_id = clientInfo["client_id"].AsString;
                var vat_id = clientInfo["vat_id"].AsString;

                // Add the "SupplierVat_id" property to the client_info node
                clientInfo["SupplierVat_id"] = SupplierVat_id;


                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("client_info.client_id", client_id),
                    Builders<BsonDocument>.Filter.Eq("client_info.vat_id", vat_id)
                );

                var existingDocument = _IcountClientInfoCollection.Find(filter).FirstOrDefault();

                if (existingDocument == null)
                {
                    // Insert the document into the collection
                    _IcountClientInfoCollection.InsertOne(document);
                }
                else
                {
                    // Document already exists, handle the case accordingly
                    // For example, you can update the existing document or log an error
                    Console.WriteLine($"Document with client_id '{client_id}' and vat_id '{vat_id}' already exists.");
                }

            }

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

        public async Task<BusinessPartnerLists> InviteBusinessPartners(int userid, int Lang)
        {
            BusinessPartnerLists BPL = new BusinessPartnerLists();
            var emailList = new List<ClientObj>(); // Initialize the list to collect email addresses
            var supplierList=new List<SupplierObj>();
            var existingUser = _repository.GetFirstObject<AdminUsers>(x => x.AdminUserid == userid);
            existingUser.ClickedButtonToInviteBusinessPartners = true;
            await _repository.UpdateAsync(existingUser);
            var UsercompanyObj = _repository.GetFirstObject<Businesses>(x => x.AdminUserid == userid);
            if (UsercompanyObj != null)
            {
                var IcountgetClientListEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 84);

                var UserexternalSystemDynamicFieldslist = _repository.GetListOfObjects<UsersExternalSystemDynamicFields>(x => x.Companyid == UsercompanyObj.BusinessId && x.Userid == userid);

                string cidvalue = null;
                string uservalue = null;
                string passvalue = null;

                foreach (var dynamicField in UserexternalSystemDynamicFieldslist)
                {
                    if (dynamicField.FieldLabelName == "cid")
                    {
                        cidvalue = dynamicField.FieldLabelValue;
                    }
                    else if (dynamicField.FieldLabelName == "user")
                    {
                        uservalue = dynamicField.FieldLabelValue;
                    }
                    else if (dynamicField.FieldLabelName == "pass")
                    {
                        passvalue = dynamicField.FieldLabelValue;
                    }
                }

                var IcountgetClientListEndpointEdited = IcountgetClientListEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                HttpMethod methodIcountgetClientListEndpoint = HttpMethod.Post;
                var ResponseIcountgetClientListEndpoint = await _UninetInputDataAccess.SendRequest(IcountgetClientListEndpointEdited, methodIcountgetClientListEndpoint);

                // Parse the JSON response
                var jsonObject = JObject.Parse(ResponseIcountgetClientListEndpoint);

                // Access the clients object
                var clients = jsonObject["clients"].ToObject<JObject>();

                // Loop through each client and collect their email addresses
                foreach (var client in clients)
                {
                    var email = client.Value["email"].ToString();
                    var client_name= client.Value["client_name"].ToString();
                    emailList.Add(new ClientObj { Email = email, Name = client_name }); // Add the email address to the list
                }
                BPL.ClientEmailList= emailList;

                var IcountgetSupplierListEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 75);
                var IcountgetSupplierListEndpointEdited = IcountgetSupplierListEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                HttpMethod methodIcountgetSupplierListEndpoint = HttpMethod.Post;
                var ResponseIcountgetSupplierListEndpoint = await _UninetInputDataAccess.SendRequest(IcountgetSupplierListEndpointEdited, methodIcountgetSupplierListEndpoint);

                // Parse the JSON response for suppliers
                var jsonObjectSuppliers = JObject.Parse(ResponseIcountgetSupplierListEndpoint);
                var suppliers = jsonObjectSuppliers["suppliers"].ToObject<JObject>();

                foreach (var supplier in suppliers)
                {
                    var email = supplier.Value["email"].ToString();
                    var supplier_name= supplier.Value["supplier_name"].ToString();
                    // Ensure the email is not empty before adding
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        supplierList.Add(new SupplierObj { Email = email, Name = supplier_name });
                    }
                    
                       
                   
                }
                BPL.SupplierList = supplierList;

            }

            return BPL; // Return the list of email addresses
        }


        public async Task<bool> ExtractClientIdsAndInsertToMongoDb(JsonElement resultsList, string cidvalue, string uservalue, string passvalue,string SupplierVat_id)
        {
            // Loop over the items in the results_list array
            foreach (JsonElement item in resultsList.EnumerateArray())
            {
                // Get the value of the client_id property
                string clientId = item.GetProperty("client_id").GetString();
                var ClinetinfoEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 73); /// call-https://api.icount.co.il/api/v3.php/company/info
                var endpointClinetinfo = ClinetinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue + "&client_id=" + clientId;
                HttpMethod methodclientinfo = HttpMethod.Get;
                // var ReponsneDocInfo = await _UninetInputDataAccess.SendRequest(endpointdocInfo, methoddocinfo);
                var ReponsneClientInfo = await _UninetInputDataAccess.SendRequest(endpointClinetinfo, methodclientinfo);
                JsonDocument jsonDocument = JsonDocument.Parse(ReponsneClientInfo);
                
                JsonElement jsonData = jsonDocument.RootElement;

                MongoDbDestination destination = MongoDbDestination.ClientInfoDB;
                  InsertDocumentInfo(jsonData, destination, SupplierVat_id);
            }
            return true;
        }
        public async Task<bool> CreateListOfDetailedDocinfoAndInsertToMongoDBCollection(JsonElement resultsList,string cidvalue, string uservalue, string passvalue)
        {

            // Iterate over each element in the 'results_list' and create a get endpoint methode to get docinfo from icount
            foreach (JsonElement item in resultsList.EnumerateArray())
            {
                // Extract the values of 'doctype' and 'docnum'
                string doctype = item.GetProperty("doctype").GetString();
                string docnum = item.GetProperty("docnum").GetString();

                var DocinfoEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 72); /// call-https://api.icount.co.il/api/v3.php/company/info
                var endpointdocInfo = DocinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue + "&doctype=" + doctype + "&docnum=" + docnum;
                HttpMethod methoddocinfo = HttpMethod.Get;
                // var ReponsneDocInfo = await _UninetInputDataAccess.SendRequest(endpointdocInfo, methoddocinfo);
                var ReponsneDocInfo = await _UninetInputDataAccess.SendRequest(endpointdocInfo, methoddocinfo);

                JsonDocument jsonDocument = JsonDocument.Parse(ReponsneDocInfo);
                JsonElement jsonData = jsonDocument.RootElement;
                MongoDbDestination destination = MongoDbDestination.DocinfoDB;
                 InsertDocumentInfo(jsonData, destination,"");

            }

            return true;


                

           
        }
        public async Task SetLastPullDataDate(int companyVatid)
        {
            

            var existingRow = await _repository.GetByIdAsync<CompanyPulledDataLog>(companyVatid);

            if (existingRow != null)
            {
                // Define the time zone ID for Israel
                string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                // Get the Israel time zone
                TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                // Convert server's DateTime.Now to Israel local time
                DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                existingRow.LastPullDataDate = israelNow;
                await _repository.UpdateAsync(existingRow);
            }
            else
            {
                // Define the time zone ID for Israel
                string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                // Get the Israel time zone
                TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                // Convert server's DateTime.Now to Israel local time
                DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                var newRow = new CompanyPulledDataLog
                {
                    CompanyVatid = companyVatid,
                    LastPullDataDate = israelNow
                };

                await _repository.CreateAsync(newRow);
            }

        }


        public async Task<JsonDocument> CallAddWebhookToIcount(LUTIcountSourceWebhookCompanyMapping row,string icountWebhookEndpointEdited)
        {
            try
            {
                int WebHookSourceid = row.WebHookSourceid;
                var IcountWebhookEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 82);//https://api.icount.co.il/api/v3.php/webhook/add
                var IcountWebhookEndpointEdited = IcountWebhookEndpoint.Endpoint + icountWebhookEndpointEdited+ WebHookSourceid + "&action=doc.create";
                HttpMethod methodIcountWebhookEndpoint = HttpMethod.Post;
                var ReponsneIcountWebhookEndpoint = await _UninetInputDataAccess.SendRequest(IcountWebhookEndpointEdited, methodIcountWebhookEndpoint);

                // Deserialize the JSON response
               return  JsonDocument.Parse(ReponsneIcountWebhookEndpoint);
            }
            catch(Exception ex) { return null; };
        }

        public async Task<ResSaveExternalCustomized> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails, string UserId)
        {
            try
            {
                DateTime startPulldata = new DateTime();
                DateTime EndPulldata = new DateTime();
                string cidvalue = null;
                string uservalue = null;
                string passvalue = null;
                var jsonObject = new
                {
                    listInputLabelDetails = spInputExternalSystemCompanyDetails.ListInputLabelDetails,
                    userid = UserId,
                    ExternalSystemId = spInputExternalSystemCompanyDetails.ExternalSystemId.ToString(),
                    CompanyId= spInputExternalSystemCompanyDetails.Companyid
                };

                // Convert the JSON object to string
                var jsonString = JsonConvert.SerializeObject(jsonObject);
                var UserParam = new
                {
                    jsonInput = jsonString
                    
                };
               
                var BusinessesObj = _repository.GetFirstObject<Businesses>(x => x.AdminUserid == Convert.ToInt32(UserId) && x.BusinessId == spInputExternalSystemCompanyDetails.Companyid);

                if (spInputExternalSystemCompanyDetails.ExternalSystemId == 2)
                {
                    //before saving data into tables we need to verify the credentials are valid
                    foreach (CustomizedDataLIst item in spInputExternalSystemCompanyDetails.ListInputLabelDetails)
                    {
                        if (item.FieldLabelName == "cid")
                        {
                            cidvalue = item.FieldLabelValue;
                        }
                        else if (item.FieldLabelName == "user")
                        {
                            uservalue = item.FieldLabelValue;
                        }
                        else if (item.FieldLabelName == "pass")
                        {
                            passvalue = item.FieldLabelValue;
                        }
                    }

                   

                    var comopanyinfoEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 70); /// call-https://api.icount.co.il/api/v3.php/company/info
                    var endpointcomopanyinfo = comopanyinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                    HttpMethod methodcomopanyinfo = HttpMethod.Get;
                    var ReponsneCompanyInfo = await _UninetInputDataAccess.SendRequest(endpointcomopanyinfo, methodcomopanyinfo);

                    // Deserialize the JSON response
                    var ReponsneCompanyInfojsonDocument = JsonDocument.Parse(ReponsneCompanyInfo);
                    var root = ReponsneCompanyInfojsonDocument.RootElement;

                    // Extract the "status" value from api response to acount
                    bool statusValue = root.GetProperty("status").GetBoolean();

                    if (statusValue)
                    {
                        //AddUserCredentialsSystemResult

                        var spresult = ExecuteGetSP_SaveUsersExternalSystemDynamicFieldsData(ConstUninetStoredprocedure.SP_SaveUsersExternalSystemDynamicFieldsData, UserParam);

                        if (spresult.AdminUserid==0)
                        {
                            //here i need to insert the logic that call to 
                            //https://api.icount.co.il/api/v3.php/webhook/add
                            //but before calling i need to find out if it already exist in our database related to an internal companyid
                            //start logic of addig webhook to icount
                            var LUTIcountSourceWebhookCompanyMappingRow = _repository.GetFirstObject<LUTIcountSourceWebhookCompanyMapping>(x => x.Internalcompanyid == spInputExternalSystemCompanyDetails.Companyid);

                            string url = "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue + "&url=https://uninetwebapi220231222123817.azurewebsites.net/api/UninetInput/ReceiveWebhook?webhooksourceid=";
                            JsonElement rootWebhookEndpointjsonDocument;
                            if (LUTIcountSourceWebhookCompanyMappingRow!=null)
                            {
                               

                                var ReponsneIcountWebhookEndpointjsonDocument = await CallAddWebhookToIcount(LUTIcountSourceWebhookCompanyMappingRow,url);
                                 rootWebhookEndpointjsonDocument = ReponsneIcountWebhookEndpointjsonDocument.RootElement;

                            }
                            else
                            {
                                var newRowLUTIcountSourceWebhookCompanyMapping = new LUTIcountSourceWebhookCompanyMapping
                                {

                                    Internalcompanyid = spInputExternalSystemCompanyDetails.Companyid
                                };

                                var insertedRow= await _repository.CreateAsyncReturnEntity(newRowLUTIcountSourceWebhookCompanyMapping);

                                var ReponsneIcountWebhookEndpointjsonDocument = await CallAddWebhookToIcount(insertedRow, url);
                                 rootWebhookEndpointjsonDocument = ReponsneIcountWebhookEndpointjsonDocument.RootElement;
                            }



                            bool status = rootWebhookEndpointjsonDocument.GetProperty("status").GetBoolean();
                            int webhookId = rootWebhookEndpointjsonDocument.GetProperty("webhook_id").GetInt32();

                            if (status)///save webhookid in table LUTIcountSourceWebhookCompanyMapping
                            {
                                var LUTIcountSourceWebhookCompanyMappingnewRow = _repository.GetFirstObject<LUTIcountSourceWebhookCompanyMapping>(x => x.Internalcompanyid == spInputExternalSystemCompanyDetails.Companyid);
                                if (LUTIcountSourceWebhookCompanyMappingnewRow!=null)
                                {
                                    LUTIcountSourceWebhookCompanyMappingnewRow.WebhookID = webhookId;

                                    // Call your UpdateAsync method to save the changes
                                    await _repository.UpdateAsync(LUTIcountSourceWebhookCompanyMappingnewRow);
                                }
                            }

                            ///end logic of adding webhook to icount and save the webhook info on table LUTIcountSourceWebhookCompanyMapping



                            // Parse the JSON string to a dynamic object
                            dynamic dynamicCompanyInfo = Newtonsoft.Json.JsonConvert.DeserializeObject(ReponsneCompanyInfo);

                            // Add the new property to the company_info object
                            dynamicCompanyInfo.company_info.InternalCompanyId = spInputExternalSystemCompanyDetails.Companyid;

                            // Convert the modified object back to JSON
                            string modifiedJson = Newtonsoft.Json.JsonConvert.SerializeObject(dynamicCompanyInfo);



                            // Get the vat_id value
                            string vatId = dynamicCompanyInfo.company_info.vat_id;

                            // Check if a document with the same vat_id already exists in the collection
                            var filter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", vatId);
                            var existingDocument = await _ICountCompanyInfoCollection.Find(filter).FirstOrDefaultAsync();

                            if (existingDocument == null)
                            {

                                // Parse the modified JSON string to a BsonDocument
                                BsonDocument modifiedCompanyInfo = BsonDocument.Parse(modifiedJson);

                                // Insert the modified document into the collection
                                try
                                {
                                    await _ICountCompanyInfoCollection.InsertOneAsync(modifiedCompanyInfo);
                                }
                                catch (Exception ex)
                                {
                                    // Log or handle the exception here
                                    Console.WriteLine($"An error occurred: {ex.Message}");
                                }
                            }
                            else
                            {
                                // Document with the same vat_id already exists, handle accordingly
                                Console.WriteLine("Document with the same vat_id already exists");
                            }


                           
                            string vatid = "";
                           
                            string propertyPathstart_date = "company_info.start_date";
                            if (ReponsneCompanyInfo != null)
                            {
                                //DateTime startDate = ExtractPropertyValue<DateTime>(ReponsneCompanyInfo.ToString(), propertyPathstart_date);
                                var AdminUserRow = _repository.GetFirstObject<AdminUsers>(x => x.AdminUserid == Convert.ToInt32(UserId));
                                // Define the time zone ID for Israel
                                string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                                // Get the Israel time zone
                                TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                                // Convert server's DateTime.Now to Israel local time
                                DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);

                                string propertyPathVatid = "company_info.vat_id";
                                vatid = ExtractPropertyValue<string>(ReponsneCompanyInfo.ToString(), propertyPathVatid);
                                startPulldata = AdminUserRow.DateCreated;
                                EndPulldata = israelNow;

                                var companyPulledDataLog = await _repository.FindAsync<CompanyPulledDataLog>(log => log.CompanyVatid == Convert.ToInt32(vatid));
                                if (companyPulledDataLog != null)
                                {
                                    startPulldata = companyPulledDataLog.LastPullDataDate;
                                }

                                await SetLastPullDataDate(Convert.ToInt32(vatid));
                            }
                            var docsearchEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 71);//call icount-https://api.icount.co.il/api/v3.php/doc/search

                            string endpointUrldocsearch = docsearchEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue + "&start_ts=" + startPulldata.ToString("MM/dd/yyyy HH:mm:ss") + "&end_ts=" + EndPulldata.ToString("MM/dd/yyyy HH:mm:ss");
                            HttpMethod methoddocsearch = HttpMethod.Get;
                            var Reponsnedocsearch = await _UninetInputDataAccess.SendRequest(endpointUrldocsearch, methoddocsearch);

                            if (Reponsnedocsearch != null)
                            {

                                // insert  compay cutomer invoices to mongodb collection name icount
                                ///JsonDocument jsonDocument = JsonDocument.Parse(Reponsnedocsearch.ToString());



                                using (JsonDocument jsonDocument = JsonDocument.Parse(Reponsnedocsearch.ToString()))
                                {
                                    string SupplierVat_id = vatid; //we send thie vat it to add it to the icountClientInfo so that each node of client will have its suplier_vat_id

                                    if (jsonDocument.RootElement.TryGetProperty("results_list", out JsonElement resultsListElement) &&
                                        resultsListElement.ValueKind == JsonValueKind.Array && resultsListElement.GetArrayLength() > 0)
                                    {
                                        // results_list exists and has items
                                        JsonElement resultsList = resultsListElement;

                                        // Your logic here
                                        InsertDocumentsToMongoDB(resultsList, vatid, UserId, spInputExternalSystemCompanyDetails.Companyid);

                                        //loop and the invoce list resultsList and get for each client a detailed client data from --https://api.icount.co.il/api/v3.php/client/info
                                        var resExtractClientIds = await ExtractClientIdsAndInsertToMongoDb(resultsList, cidvalue, uservalue, passvalue, SupplierVat_id);



                                        //loop on all invoice and get  for each invoce a detailed invoce  and save it in icountdocinfo collection
                                        //https://api.icount.co.il/api/v3.php/doc/info?cid=uninetttt&user=eyalberda&pass=Ilayshaked10&doctype=invoice&docnum=2002
                                        //foreach invoce in resultsList get property value of doctype and docnum
                                        var resCreateListOfDetail = await CreateListOfDetailedDocinfoAndInsertToMongoDBCollection(resultsList, cidvalue, uservalue, passvalue);

                                    }
                                }





                            }



                            var res = new ResSaveExternalCustomized
                            {
                                Success = true,
                                textResponse = spInputExternalSystemCompanyDetails.Lang == 1 ? "your credentials saved succesfully" : "הנתונים עבור המערכת החיצונית נשמרו בהצלחה ",
                                SystemRegisteredInuninet = true,
                                ValidExternalsystemCredenatials = true,
                                FullName = BusinessesObj.FirstName + " " + BusinessesObj.LastName
                            };
                            return res;



                        }
                        else //here i get the userid related to masteruser from adminusers table
                        {

                            //added logic eyal to return valid message  that there is already a master user that has  
                            //same credentials and this is his details 
                            //please ask the master to add you to our system than  this user will be able to enter his 
                            //email and password and get  into the master console system 

                           


                            var res = new ResSaveExternalCustomized
                            {
                                Success = false,
                                textResponse = spInputExternalSystemCompanyDetails.Lang == 1 ? "user "+ spresult.FirstName+" "+ spresult.LastName+" "+"from this organiztion "+ spresult.OrganizationName+" is the master user on your organzation please contact him via his email "+ spresult.Email+"so he can add you to our system": 
                                "משתמש " + spresult.FirstName + " " + spresult.LastName + " " + "מהארגון שלך " + spresult.OrganizationName + "הוא משתמש על בחברה שלך אנא צור איתו קשר במייל הבא " + spresult.Email + "על מנת שיוסיף אותך למערכת שלנו",
                                SystemRegisteredInuninet = false,
                                ValidExternalsystemCredenatials = true,
                                FullName = BusinessesObj.FirstName + " " + BusinessesObj.LastName
                            };
                            return res;
                        }

                    }
                    else
                    {
                        var res = new ResSaveExternalCustomized
                        {
                            Success = false,
                            textResponse = spInputExternalSystemCompanyDetails.Lang == 1 ? "we couldnt authneticate your external system credentials please try again" : "לא הצלחנו לאמת את הנתונים שסיפקת מול המערכת שאיתה אתה נירשם  אנא נסה שנית ",
                            SystemRegisteredInuninet = false,
                            ValidExternalsystemCredenatials = false,
                            FullName = BusinessesObj.FirstName + " " + BusinessesObj.LastName
            };
                        return res;
                    }





                }
                else//external data wasnt saved to database 
                {
                    //here we need to save the user with the wanted system id  in table UsersExternalSystemDynamicFields 
                    var newUsersExternalSystemDynamicFields = new UsersExternalSystemDynamicFields
                    {
                        Companyid = spInputExternalSystemCompanyDetails.Companyid,
                        Userid = Convert.ToInt32(UserId),
                        ExternalSystemId=spInputExternalSystemCompanyDetails.ExternalSystemId,
                        FieldLabelName="N",
                        FieldLabelValue= spInputExternalSystemCompanyDetails.ExternalSystemId==18? spInputExternalSystemCompanyDetails.ListInputLabelDetails[0].FieldLabelValue : "V"
                        // Set other properties as needed
                    };
                    Int32 intuserid = Convert.ToInt32(UserId);
                    var res_UninetExternalSystems = _repository.GetFirstObject<UsersExternalSystemDynamicFields> (x => x.Companyid == spInputExternalSystemCompanyDetails.Companyid && x.Userid== intuserid);
                    if (res_UninetExternalSystems != null)
                    {
                       await _repository.DeleteAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == spInputExternalSystemCompanyDetails.Companyid && x.Userid == intuserid);
                        _repository.Create<UsersExternalSystemDynamicFields>(newUsersExternalSystemDynamicFields);
                    }
                    else
                    {
                        _repository.Create<UsersExternalSystemDynamicFields>(newUsersExternalSystemDynamicFields);
                    }

                    var res = new ResSaveExternalCustomized
                        {
                            Success = false,
                            textResponse = "data wasnt saved ",
                            SystemRegisteredInuninet = false,
                           ValidExternalsystemCredenatials=null,
                            FullName = BusinessesObj.FirstName + " " + BusinessesObj.LastName
                    };
                        return res;
                    
                }
               
                

            }
            catch (Exception ex)
            {
                var res = new ResSaveExternalCustomized
                {
                    Success = false,
                    textResponse = "",
                    SystemRegisteredInuninet = false,
                    ValidExternalsystemCredenatials=null,
                    FullName = ""
                };
                return res;
            };
        }
        public void InsertDocumentsToMongoDB(JsonElement resultsList,string vatid,string InternalUserId,int InternalComopanyId )
        {
            foreach (var item in resultsList.EnumerateArray())
            {
                var document = new BsonDocument
          {
            { "doctype", item.GetProperty("doctype").GetString() },
            { "docnum", item.GetProperty("docnum").GetString() },
            { "dateissued", item.GetProperty("dateissued").GetString() },
            { "timeissued", item.GetProperty("timeissued").GetString() },
            { "client_id", item.GetProperty("client_id").GetString() },
            { "custom_client_id", item.GetProperty("custom_client_id").GetString() },
            { "currency_id", item.GetProperty("currency_id").GetString() },
            { "currency_code", item.GetProperty("currency_code").GetString() },
            { "currency", item.GetProperty("currency").GetString() },
            { "rate", item.GetProperty("rate").GetString() },
            { "total", item.GetProperty("total").GetString() },
            { "is_cancellation", item.GetProperty("is_cancellation").GetInt32() },
            { "is_cancelled", item.GetProperty("is_cancelled").GetInt32() },
            { "status", item.GetProperty("status").GetInt32() },
            { "vat_id", vatid },
            {"InternalCompanyId", InternalComopanyId.ToString() },
            {"InternalUserid",InternalUserId }
            
        };

                // Check if the document already exists in the collection
                var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("docnum", document["docnum"]),
                Builders<BsonDocument>.Filter.Eq("vat_id", document["vat_id"])
                );
                var existingDocument = _ICountCollection.Find(filter).FirstOrDefault();
                if (existingDocument == null)
                {
                    _ICountCollection.InsertOne(document);
                }
                else
                {
                    // Handle the case where the document already exists
                    // You can update the existing document or skip it based on your requirement
                }
            }
        }

        public async Task<Dictionary<int, string>> GetExternalSystems(int Lang)
        {
            var hashtable = _repository.GetHashtableOfExternalFields(Lang);

            var dictionary = hashtable.Cast<DictionaryEntry>()
                .ToDictionary(entry => (int)entry.Key, entry => (string)entry.Value);

            return dictionary;
        }


        public async Task<ExternalsystemCompanyTotalDetails> GetExternalCustomizedFieldByExternaLSystemID(int ExternalSystemId)
        {
            try
            {
                /*
                ExternalSystemCustomizeFieldResult

                public string BusinessLogUrl { get; set; }
                public  List<CustomizedDataLIst> listdata { get; set; }  
                public string VideoLink { get; set; }
                */
                List<CustomizedDataLIst> listdata = new List<CustomizedDataLIst>();
                //var res = _repository.GetListOfObjects<ExternalSystemDynamicFields>(x => x.ExternalSystemId == ExternalSystemId)

                var res = _repository.GetListOfFiledObjects(ExternalSystemId);//eyal critical add logic to all companies
                                                                              //select * from ExternalSystemDynamicFields
                                                                              //select* from LUT_ExtrenalFieldsType
                                                                              //need to add  values from this table
                                                                              //select * from LUT_UninetExternalSystems


                var res_logo_video = _repository.GetFirstObject< LUT_UninetExternalSystems>(x=>x.SyestemId== ExternalSystemId);
                foreach (var item in res)
                {
                    var customizedData = new CustomizedDataLIst
                    {
                        FieldLabelName = item.FieldLabelName,
                        FieldLabelValue=item.FieldLabelValue,
                        FiledType= item.FieldType,
                        FieldTypeDesc=item.FiledTypeDesc
                    };

                    listdata.Add(customizedData);
                }
                var ExternalSystemCustomizeFieldResultResult = new ExternalsystemCompanyTotalDetails
                {
                    ListInputLabelDetails = listdata,
                    LogoIcon = res_logo_video.Logo,
                    VideoLink = res_logo_video.Video
                };


                return ExternalSystemCustomizeFieldResultResult;

            }
            catch (Exception ex) { return null; };
        }
        protected async Task<bool> SendOtp(string from, string To, string Body)
        {
            try
            {
                string accountSid = "ACcdd2a5b796d15d9d0f1bf28214de5adf";// Environment.GetEnvironmentVariable("ACcdd2a5b796d15d9d0f1bf28214de5adf");
                string authToken = "e18d4227bb3ace9310f71971e28866c8";// Environment.GetEnvironmentVariable("e18d4227bb3ace9310f71971e28866c8");

                TwilioClient.Init(accountSid, authToken);

                var message = MessageResource.Create(
                  body: "מספר חד פעמי הוא :" + Body,
                  from: new Twilio.Types.PhoneNumber(from),//("+19142668348"),
                  to: new Twilio.Types.PhoneNumber(To)//("+972526350902")
              );
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //public async Task<bool> VerifyEmailLink(string Userguid)
        //{
        //    try
        //    {
        //        var Adminuserobj = _repository.GetFirstObject<AdminUsers>(x => x.GuidVerification == Userguid );// && x.Password == model.P
        //        if (Adminuserobj != null) 
        //        {
        //            return true;
        //        }
        //        else
        //        {
        //            return false;
        //        }
        //    }
        //    catch (Exception ex) { return false; }
        //}


        public AddUserCredentialsSystemResult ExecuteGetSP_SaveUsersExternalSystemDynamicFieldsData(string spName, object parameters)
        {
            var resultList = _repository.ExecuteGetSP<AddUserCredentialsSystemResult>(spName, parameters).ToList();

            // Ensure there is at least one result to avoid IndexOutOfRangeException
            if (resultList.Count > 0)
            {
                // Return the first item in the list, which is already of type AddUserCredentialsSystemResult
                return resultList[0];
            }

            // Return a default instance or null if no data is returned from the stored procedure
            // Depending on your application's needs, you might want to return a new instance with default values instead of null
            return null;
        }




        public bool ExecuteGetSP(string spName, object parameters)
        {
            var result = _repository.ExecuteGetSP<AddBusinessToUserResult>(spName, parameters);
            return result.ToList()[0].Result;
        }


        public async Task<AddBusinessToUserResult> RegisterBusinessToUser(UserBusinesses userBusinesses)
        {
            try
            {
                var businessobjects = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == userBusinesses.Userid).ToList();

                var existingCompanyInnerId = (await _repository.GetAllAsync<LutCompanies>())
                    .Where(b => businessobjects.Any(r => r.BusinessId == b.CompanyInnerId))
                    .Select(b => b.CompanyInnerId)
                    .ToList(); // Convert to List

                //get number of companies from json 
                int count = userBusinesses.BusinessRequests.Count;
                if (existingCompanyInnerId.Count != count)
                {
                    if (count > 0)
                    {
                        foreach (var businessRequest in userBusinesses.BusinessRequests)
                        {
                            var newCompany = new LutCompanies
                            {
                                CompanyName = null,
                                CompanyEmail = null
                                // Set other properties as needed
                            };

                            _repository.Create<LutCompanies>(newCompany);
                            int lastInsertedCompanyinneridId = newCompany.CompanyInnerId;
                            businessRequest.BusinessId = lastInsertedCompanyinneridId;
                        }
                    }

                    var existingBusinessIds = (await _repository.GetAllAsync<Businesses>())
                        .Where(b => userBusinesses.BusinessRequests.Any(r => r.BusinessId == b.BusinessId))
                        .Select(b => b.BusinessId)
                        .ToList(); // Convert to List

                    var existingBusinessRequests = userBusinesses.BusinessRequests
                        .Where(br => existingBusinessIds.Contains(br.BusinessId))
                        .ToList();

                    if (existingBusinessRequests.Count > 0)
                    {
                        var result = new AddBusinessToUserResult
                        {
                            Result = false,
                            BusinessRequests = existingBusinessRequests,
                            
                        };
                        return result;
                    }
                    else
                    {
                        var dataTable = new DataTable();
                        dataTable.Locale = new CultureInfo("he-IL");
                        dataTable.Columns.Add("BusinessId", typeof(int));
                        dataTable.Columns.Add("BusinessType", typeof(int));
                        dataTable.Columns.Add("FirstName", typeof(string));
                        dataTable.Columns.Add("LastName", typeof(string));
                        dataTable.Columns.Add("MobileNumber", typeof(string));
                        dataTable.Columns.Add("OrganizationRole", typeof(string));
                        dataTable.Columns.Add("OrganizationName", typeof(string));
                        dataTable.Columns.Add("OrganizationType", typeof(int));
                        dataTable.Columns.Add("ExternalSystemId", typeof(int));

                        Encoding utf8 = Encoding.UTF8;

                        foreach (var businessRequest in userBusinesses.BusinessRequests)
                        {
                            dataTable.Rows.Add(
                             businessRequest.BusinessId,
                             businessRequest.BusinessType,
                             utf8.GetString(utf8.GetBytes(businessRequest.FirstName)), // Convert FirstName to UTF-8
                             utf8.GetString(utf8.GetBytes(businessRequest.LastName)),  // Convert LastName to UTF-8
                             utf8.GetString(utf8.GetBytes(businessRequest.MobileNumber)), // Convert MobileNumber to UTF-8
                             utf8.GetString(utf8.GetBytes(businessRequest.OrganizationRole)), // Convert OrganizationRole to UTF-8
                             utf8.GetString(utf8.GetBytes(businessRequest.OrganizationName)), // Convert OrganizationName to UTF-8
                             businessRequest.OrganizationType,
                             businessRequest.ExternalSystemId
                         );
                        }

                        var parameter = new SqlParameter("@BusinessRequests", SqlDbType.NVarChar)
                        {
                            Value = JsonConvert.SerializeObject(dataTable, Formatting.None)
                        };

                        var UserParam = new
                        {
                            UserId = userBusinesses.Userid,
                            BusinessRequests = parameter.Value
                        };

                        bool spresult = ExecuteGetSP(ConstUninetStoredprocedure.SP_AddBusinessesToUser, UserParam);

                        // Retrieve the added business requests from the Businesses table
                        var addedBusinessRequests = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == userBusinesses.Userid).ToList();
                        // Map Businesses objects to BusinessRequest objects
                        var addedBusinessRequestModels = addedBusinessRequests.Select(br => MapBusinessToBusinessRequest(br)).ToList();
                        var result = new AddBusinessToUserResult
                        {
                            Result = spresult,
                            BusinessRequests = addedBusinessRequestModels,
                           
                        };
                       

                        return result;
                    }
                }
                else
                {
                    var result = new AddBusinessToUserResult
                    {
                        Result = false,
                        BusinessRequests = null
                        
                    };
                    return result;
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                var result = new AddBusinessToUserResult
                {
                    Result = false,
                    BusinessRequests = null
                   
                };
                return result;
            }
        }

        // Mapping method to convert Businesses to BusinessRequest
        private BusinessRequest MapBusinessToBusinessRequest(Businesses business)
        {
            return new BusinessRequest
            {
                BusinessId = business.BusinessId,
                BusinessType = business.BusinessType,
               // FirstName = business.FirstName,
                //LastName = business.LastName,
               // MobileNumber = business.MobileNumber,
                OrganizationRole = business.OrganizationRole,
                OrganizationName = business.OrganizationName,
                OrganizationType = business.OrganizationType,
                ExternalSystemId = business.ExternalSystemId
            };
        }

        public static string EncryptUserId(string userId, string key, byte[] iv)
        {
            byte[] encryptedBytes;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = StringToByteArray(key);
                aesAlg.IV = iv;
                aesAlg.Padding = PaddingMode.PKCS7; // Set the padding mode

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                byte[] userIdBytes = Encoding.UTF8.GetBytes(userId);
                encryptedBytes = encryptor.TransformFinalBlock(userIdBytes, 0, userIdBytes.Length);

                encryptor.Dispose();
            }

            return Convert.ToBase64String(encryptedBytes);
        }

        public static byte[] StringToByteArray(string hex)
        {
            int length = hex.Length / 2;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return bytes;
        }



        public static byte[] GenerateSalt(int sizeInBytes)
        {
            byte[] salt = new byte[sizeInBytes];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(salt);
            }
            return salt;
        }

        public static byte[] GenerateAesKey(string passphrase, byte[] salt)
        {
            const int keySizeInBits = 256;
            const int keySizeInBytes = keySizeInBits / 8;

            using (Rfc2898DeriveBytes deriveBytes = new Rfc2898DeriveBytes(passphrase, salt))
            {
                return deriveBytes.GetBytes(keySizeInBytes);
            }
        }
        public static byte[] GenerateRandomIV(int sizeInBytes)
        {
            byte[] iv = new byte[sizeInBytes];
            using (RNGCryptoServiceProvider rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(iv);
            }
            return iv;
        }
        public async Task<ApprovalMailIndication> SaveIndicationOfSentApprovalMailToCustomer(int Userid, string otp)
        {
            try
            {
                var UserParam = new { userid = Userid , Otp = otp };
                //byte[] salt = GenerateSalt(16);
               // byte[] key = Encoding.UTF8.GetBytes(Configuration["EncryptedUserId:key"]);
                string iv = Configuration["EncryptedUserId:iv"];
                byte[] ivBytes = Encoding.UTF8.GetBytes(iv);

               

                string encryptedUserId = EncryptUserId(Userid.ToString(), Configuration["EncryptedUserId:key"], ivBytes);
                Console.WriteLine("Encrypted User ID: " + encryptedUserId);

                var Procedureres = _repository.ExecuteGetSP<SaveIndicationOfSentApprovalMailToCustomerResponse>(ConstUninetStoredprocedure.SP_SaveIndicationOfSentApprovalMailToCustomer, UserParam);
                bool ProcedureResult = Procedureres.ToList()[0].result;
                
                var ApprovalMailIndicationResult = new ApprovalMailIndication
                {
                    result = ProcedureResult,
                    EncryptedUserid = encryptedUserId
                };
                return ApprovalMailIndicationResult;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<ResponseResendOtp> ResendOtp(ResentOtpRequest resentOtpRequest, int DecryptedUserId,int Lang)
        {
            try
            {
                SendOtpViaMailResponse sendsmtpmailres = new SendOtpViaMailResponse();
                // Check if user already has a row in useradmin so we can send them the OTP again
                var result = _repository.GetFirstObject<AdminUsers>(x => x.Email == resentOtpRequest.Email && x.AdminUserid== DecryptedUserId);
                if (result == null)
                {
                    var res = new ResponseResendOtp
                    {
                        Success = false,
                        Desc = Lang==1? "User didn't register yet, no OTP sent":"משתמש לא נירשם , לא נישלח קוד "
                    };
                    return res;
                }
                else
                {
                    if (result.ValidUser == false)
                    {
                        // Define the time zone ID for Israel
                        string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                        // Get the Israel time zone
                        TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                        // Convert server's DateTime.Now to Israel local time
                        DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                        DateTime? lastTimeOtpSent = result.DateOtpSent;
                        DateTime currentTime = israelNow;

                        // Calculate the time difference between the current time and the last OTP sent time
                        TimeSpan timeDifference = currentTime - lastTimeOtpSent.GetValueOrDefault();

                        if (timeDifference.TotalMinutes <= 5)
                        {
                            // Check if the OtpSentCounter is greater than or equal to 5
                            if (result.OtpSentCounter >= 5)
                            {
                                var res = new ResponseResendOtp
                                {
                                    Success = false,
                                    Desc = Lang == 1 ? "Can't send OTP more than five times in the last five minutes": "לא ניתן לבקש לשלוח סיסמה יותר מחמש פעמים בחמש דקות האחרונות",
                                    userid = 0,
                                    otp = null
                                };
                                return res;
                            }
                            else
                            {
                                // Continue to send OTP and update the necessary fields
                                // Send OTP logic here

                                // Update OtpSentCounter and DateOtpSent
                                sendsmtpmailres = await _dataMailassist.sendsmtpmail("סיסמה חד פעמית UNINET ", "eyalbmma@gmail.com", resentOtpRequest.Email, resentOtpRequest.TemplateId, resentOtpRequest.Lang);
                                if (sendsmtpmailres.result)
                                {
                                    if (result.OtpSentCounter==null)
                                    {
                                        result.OtpSentCounter = 0;
                                        result.OtpSentCounter++;
                                        result.DateOtpSent = currentTime;
                                        _repository.Update(result);
                                    }
                                    else
                                    {
                                        result.OtpSentCounter++;
                                       
                                        _repository.Update(result);
                                    }
                                     // Assuming you have an update method in your repository

                                    var res = new ResponseResendOtp
                                    {
                                        Success = true,
                                        Desc = Lang == 1 ? "OTP sent successfully":"קוד נשלח בהצלחה",
                                        userid= result.AdminUserid,
                                        otp= sendsmtpmailres.OTP
                                    };
                                    return res;
                                }
                                else
                                {
                                    var res = new ResponseResendOtp
                                    {
                                        Success = false,
                                        Desc = Lang == 1 ? "Error Ocured Otp wasnt sent":"ארעה שגיאה קוד לא נישלח",
                                        userid = 0,
                                        otp = null
                                    };
                                    return res;
                                }
                               
                            }
                        }
                        else
                        {
                            // Reset OtpSentCounter and DateOtpSent if the time difference is greater than 5 minutes
                            result.OtpSentCounter = 1;
                            result.DateOtpSent = currentTime;
                            _repository.Update(result); // Assuming you have an update method in your repository

                            // Send OTP logic here
                            sendsmtpmailres = await _dataMailassist.sendsmtpmail("סיסמה חד פעמית UNINET ", "eyalbmma@gmail.com", resentOtpRequest.Email, resentOtpRequest.TemplateId, resentOtpRequest.Lang);
                            if (sendsmtpmailres.result)
                            {
                                var res = new ResponseResendOtp
                                {
                                    Success = true,
                                    Desc = Lang == 1 ? "OTP sent successfully" : "קוד נשלח בהצלחה",
                                    userid = result.AdminUserid,
                                    otp = sendsmtpmailres.OTP
                                };
                                return res;
                            }
                            else
                            {
                                var res = new ResponseResendOtp
                                {
                                    Success = false,
                                    Desc = Lang == 1 ? "Error Ocured Otp wasnt sent" : "ארעה שגיאה קוד לא נישלח",
                                    userid = 0,
                                    otp = null
                                };
                                return res;
                            }
                        }
                    }
                    else
                    {
                        var res = new ResponseResendOtp
                        {
                            Success = false,
                            Desc = Lang==1? "user is already validated no need for otp":"המשתמש כבר אומת  אין צורך בשליחת קוד לאימות",
                            userid = 0,
                            otp = null
                        };
                        return res;
                    }
                }
            }
            catch (Exception ex) {

                var res = new ResponseResendOtp
                {
                    Success = false,
                    Desc = ex.Message
                };
                return res;
            }
        }

        public async Task<ReturnRegisterUser> RegisterUser(RegisterUserRequest RegisterUserReq)
        {
            try
            {
                // Define the time zone ID for Israel
                string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                // Get the Israel time zone
                TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                // Convert server's DateTime.Now to Israel local time
                DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                var NewUser = new AdminUsers
                {

                    //FirstName = RegisterUserReq.FirstName,
                    //LastName = RegisterUserReq.LastName,
                    //PhoneNumber = RegisterUserReq.PhoneNumber,
                    DateCreated = israelNow,
                    ValidUser = false,
                    Email = RegisterUserReq.Email,
                    passwordEncrypted=RegisterUserReq.Password
                };

                var result = _repository.GetFirstObject<AdminUsers>(x => x.Email == RegisterUserReq.Email );//&& x.passwordEncrypted == RegisterUserReq.Password
                if (result != null)
                {
                    

                    var returnuser = new ReturnRegisterUser
                    {
                        Userid = result.AdminUserid,
                        UserStatusIndication = 1,
                        verified= result.ValidUser
                    };

                    return returnuser;//user exist
                }
                else
                { 
                    _repository.Create<AdminUsers>(NewUser);//new user
                    // Retrieve the last inserted identity value

                    int lastInsertedId = NewUser.AdminUserid;
                    // int lastInsertedId = _repository.GetLastInsertedId(NewUser);
                    // Return the AdminUserId

                   

                    var returnuser = new ReturnRegisterUser
                    {
                        Userid = lastInsertedId,
                        UserStatusIndication = 0,
                        verified = false
                    };
                    return returnuser;
                   
                }
                   
               
            }
            catch (Exception ex)
            {
                return null;//exception
            }
        }
        private string GenerateRandomPassword()
        {
            // Implement logic to generate a random password
            // You can use libraries like System.Security.Cryptography.RandomNumberGenerator to generate secure random passwords.
            // For simplicity, this example generates a password with 8 characters, but you should consider a more robust solution.
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private const string Salt = "123456789012345"; // Replace with a secure random salt

        public static string EncryptPassword(string password)
        {
            // Combine the password and salt before hashing
            string saltedPassword = string.Concat(password, Salt);

            // Create a SHA256 hash object
            using (SHA256 sha256 = SHA256.Create())
            {
                // Compute the hash value of the salted password
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(saltedPassword));

                // Convert the hash bytes to a hexadecimal string representation
                string hashedPassword = BitConverter.ToString(hashBytes).Replace("-", "").ToLower();

                return hashedPassword;
            }
        }

        public async Task<ResetPasswordReponse> ResetPassword(ResetPasswordRequestcs resetpasswordrequest)
        {
            try
            {
                // Define the time zone ID for Israel
                string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                // Get the Israel time zone
                TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                // Convert server's DateTime.Now to Israel local time
                DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                var adminuserObj = _repository.GetFirstObject<AdminUsers>(x => x.ResetPasswordToken == resetpasswordrequest.ResetPasswordToken);
                if (adminuserObj != null) {

                    if (israelNow > adminuserObj.ExpiredpasswordTokenDate)
                    {
                        // The token has expired. You can return an error response or handle it as needed.

                        var ResResetPassword = new ResetPasswordReponse
                        {
                            success = false,
                            textResponse = resetpasswordrequest.Lang == 1 ? "token expired" : "הזמן למילוי הסיסמה שנשלחה אליך עבר נסה שנית"
                        };
                        return ResResetPassword;//BadRequest("Token has expired.");
                    }
                    else
                    {
                        adminuserObj.passwordEncrypted = resetpasswordrequest.passwordEncrypted;
                        // Save the changes to the database
                        await _repository.UpdateAsync(adminuserObj);
                        var ResResetPassword = new ResetPasswordReponse
                        {
                            success = true,
                            textResponse = resetpasswordrequest.Lang == 1 ? "password updated" : "הסיסמה שונתה בהצלחה"
                        };
                        return ResResetPassword;
                    }
                }
                else
                {
                    //user doesnt exist in database according to resetpasswordtoken
                    var ResResetPassword = new ResetPasswordReponse
                    {
                        success = false,
                        textResponse = resetpasswordrequest.Lang == 1 ? "the user doesnt exist in our system" : "המשתמש אינו קיים במערכת "
                    };
                    return ResResetPassword;

                }
            }
            catch (Exception ex)
            {
                var ResResetPassword = new ResetPasswordReponse
                {
                    success = false,
                    textResponse = resetpasswordrequest.Lang == 1 ? "an eror occured" : "ארעה שדיאה הסיסמה לא אופסה "
                };
                return ResResetPassword;
            }
        }

        public async Task<ActiveTabResponse> GetActiveTab(GetActiveTabRequest getActiveTabRequest)
        {
            try
            {
                var businessdatarow = _repository.GetFirstObject<BusinessData>(x => x.JsonDocumentid == getActiveTabRequest.JsonDocumentId);
                if (businessdatarow!=null)
                {
                    string ActiveKy = "";
                   switch (businessdatarow.DocumentApprovedtoUninet) 
                   {
                        case null:
                            ActiveKy = "Inbox";
                            break;
                        case false:
                            ActiveKy = "Rejected";
                            break;
                        case true:
                            ActiveKy = "Entered";
                            break;

                    }
                    var res = new ActiveTabResponse
                    {
                        ActiveTabNaem = ActiveKy,
                        textResponse = getActiveTabRequest.Lang == 1 ? "active tab  return " : "שם טאב פעיל נימצא"
                    };
                    return res;

                }
                else
                {
                    var res = new ActiveTabResponse
                    {
                        ActiveTabNaem = "",
                        textResponse = getActiveTabRequest.Lang == 1 ? "Failed to retriev active tab" : "כשלון באיחזור טאב פעיל "
                    };
                    return res;
                }
                
            }
            catch (Exception ex)
            {
                var res = new ActiveTabResponse
                {
                    ActiveTabNaem = "",
                    textResponse = getActiveTabRequest.Lang == 1 ? "Failed to retriev active tab" : "כשלון באיחזור טאב פעיל "
                };
                return res;
            }
        }
        public async Task<LoginWithEmailandPasswordResponse> VerifyEmailLink(string EmailGuidVerification)
        {
            try
            {
                Businesses q1q2_res = null; // Declare q1q2_res as Businesses type
                UsersExternalSystemDynamicFields q3_res = null; // Declare q3_res as UsersExternalSystemDynamicFields type
                var userexistbyemailguid = _repository.GetFirstObject<AdminUsers>(x => x.EmailGuidVerification == EmailGuidVerification);
                if (userexistbyemailguid != null)
                {
                    q1q2_res = _repository.GetFirstObject<Businesses>(x => x.AdminUserid == userexistbyemailguid.AdminUserid);
                    if (q1q2_res != null)
                    {
                        q3_res = _repository.GetFirstObject<UsersExternalSystemDynamicFields>(x => x.Userid == userexistbyemailguid.AdminUserid && x.ExternalSystemId == q1q2_res.ExternalSystemId);
                    }
                    var Response = new LoginWithEmailandPasswordResponse
                    {
                        Q1_Q2_InidicationRes = q1q2_res != null ? true : false,
                        Q3_InidicationRes = q3_res != null ? true : false,
                        Userid = userexistbyemailguid.AdminUserid,
                        verified = userexistbyemailguid.ValidUser,
                        BusinessID = q1q2_res == null ? null : q1q2_res.BusinessId,
                        FullName = q1q2_res == null ? "" : q1q2_res.FirstName + " " + q1q2_res.LastName
                    };
                    return Response;
                }
                else
                {
                    var Response = new LoginWithEmailandPasswordResponse
                    {
                        Q1_Q2_InidicationRes = false,
                        Q3_InidicationRes = false,
                        Userid = 0,
                        verified = false,
                        FullName = ""
                    };
                    return Response;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }
            public async Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest)
             {
            try
            {
                // Check if the user exists in the database based on the provided email
                var user = _repository.GetFirstObject<AdminUsers>(x => x.Email == forgotPasswordRequest.Email);
                // Define the time zone ID for Israel
                string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                // Get the Israel time zone
                TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                // Convert server's DateTime.Now to Israel local time
                DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                if (user != null)
                {
                    string ResetPasswordWebAPILink = "";
                    string UserResetPasswordToken = Guid.NewGuid().ToString();
                    user.ResetPasswordToken = UserResetPasswordToken;
                    DateTime now = israelNow;
                    DateTime futureTime = now.AddMinutes(10);
                    user.ExpiredpasswordTokenDate = futureTime;
                    
                    // Save the changes to the database
                    await _repository.UpdateAsync(user);




                    // Send the password reset email
                    _dataMailassist.sendsmtpmail("Uninet reset password", "eyalbmma@gmail.com", forgotPasswordRequest.Email, 9, 1);

                    // Return true to indicate that the password reset email was sent successfully
                    var ForgotPasswordResponse = new ForgotPasswordResponse
                    {
                        Success = true,
                        textResponse = forgotPasswordRequest.Lang == 1 ? "reset password was sent to email" : "איפוס סיסמה נישלח למייל"

                    };
                    return ForgotPasswordResponse;
                }
                else
                {
                    // The user with the provided email doesn't exist in the database
                    var ForgotPasswordResponse = new ForgotPasswordResponse
                    {
                        Success = false,
                        textResponse = forgotPasswordRequest.Lang == 1 ? "The credentials you have supllied are wrong please try again " : "הפרטים שסיפקת אינם נכונים אנא נסה שנית "

                    };
                    return ForgotPasswordResponse;
                }
            }
            catch (Exception ex)
            {
                var ForgotPasswordResponse = new ForgotPasswordResponse
                {
                    Success = false,
                    textResponse = forgotPasswordRequest.Lang == 1 ? "there was an error reseting your password" : "ארעה שגיאה באיפוס הסיסמה"

                };
                return ForgotPasswordResponse;
            }
        }
        public async Task<GoogleSigninResponse> GoogleSignIn(GoogleSignInModel googlesignInrequest)
        {
            try
            {

                string email = googlesignInrequest.Email;
                string googleId = googlesignInrequest.GoogleId;

                // 1. Check if the Google ID and email exist in the database
                var existingUseremailandgoogleid = _repository.GetFirstObject<AdminUsers>(x => x.GoogleId == googleId && x.Email == email);
                //var existingUser = await _dbContext.AdminUsers.FirstOrDefaultAsync(x => x.GoogleId == googleSignInResponse.GoogleId && x.Email == googleSignInResponse.Email);

                if (existingUseremailandgoogleid != null)
                {
                    // Scenario 3: User with the provided Google ID and email exists in the database
                    // Perform login actions and return user details or token
                    // For example, you can use a token-based authentication and return a token.
                    var resgoogleSignin = new GoogleSigninResponse
                    {
                        Success = true,
                        UserId= existingUseremailandgoogleid.AdminUserid,
                        verified= existingUseremailandgoogleid.ValidUser,
                        textResponse = googlesignInrequest.lang==1? " user is succesfully verified  ": "פרטי המשתמש אומתו בהצלחה" //email and googleid exist in DB user is
                    };
                        
                    return resgoogleSignin;
                }
                else
                {
                    // Check if the email exists in the database
                    var existingUserwithemail = _repository.GetFirstObject<AdminUsers>(u => u.Email == email);
                    if (existingUserwithemail != null)
                    {
                        // If the email exists, check if the Google ID in the database is empty or null
                        if (string.IsNullOrEmpty(existingUserwithemail.GoogleId))
                        {
                            // If the Google ID is empty, verify it using Google API
                            string googleTokenUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={googleId}";
                            using (var httpClient = new HttpClient())
                            {
                                var response = await httpClient.GetAsync(googleTokenUrl);

                                if (response.IsSuccessStatusCode)
                                {
                                    string responseBody = await response.Content.ReadAsStringAsync();
                                    var tokenInfo = JsonConvert.DeserializeObject<GoogleTokenInfo>(responseBody);

                                    if (tokenInfo.email == email)
                                    {
                                        // Update the Google ID in the database
                                        existingUserwithemail.GoogleId = googleId;
                                        await _repository.UpdateAsync(existingUserwithemail);

                                        var resgoogleSignin = new GoogleSigninResponse
                                        {
                                            Success = true,
                                            UserId = existingUseremailandgoogleid.AdminUserid,
                                            verified = existingUseremailandgoogleid.ValidUser,
                                            textResponse = googlesignInrequest.lang == 1 ? "verfication succeed" : "הזדהות מול גוגל הצליחה"//"Google ID verified and linked to the existing email"

                                        };

                                        return resgoogleSignin;
                                        
                                        //return Ok(new { Message = "Google ID verified and linked to the existing email" });
                                    }
                                    else
                                    {
                                        //return BadRequest(new { Error = "Invalid email or Google ID" });

                                        var resgoogleSignin = new GoogleSigninResponse
                                        {
                                            Success = false,
                                            UserId = null,
                                            verified = false,
                                            textResponse = googlesignInrequest.lang == 1 ? "verfication failed1" : " האימות נכשל "  //Invalid email or Google ID
                                        };

                                        return resgoogleSignin;
                                       
                                    }
                                }
                                else
                                {
                                    var resgoogleSignin = new GoogleSigninResponse
                                    {
                                        Success = false,
                                        UserId = null,
                                        verified = false,
                                        textResponse = googlesignInrequest.lang == 1? "verfication failed" : " האימות נכשל "  ////Failed to verify user with Google API
                                    };

                                    return resgoogleSignin;
                                    //return false; //BadRequest(new { Error = "Failed to verify user with Google API" });
                                }
                            }
                        }
                        else
                        {
                            // If the Google ID is not empty, verify that the provided Google ID matches the one in the database
                            if (existingUserwithemail.GoogleId == googleId)
                            {
                                var resgoogleSignin = new GoogleSigninResponse
                                {
                                    Success = true,
                                    UserId = existingUseremailandgoogleid.AdminUserid,
                                    verified = existingUseremailandgoogleid.ValidUser,
                                    textResponse = googlesignInrequest.lang == 1 ? "User already exists":"משתמש כבר קיים "
                                };
                                return resgoogleSignin;
                                // return Ok(new { Message = "User already exists" });
                                //return true;
                            }
                            else
                            {///this else is if requseted googleid doesnt match database googleid



                                //if google id request is not empty send it to google and get from it the email 
                                string googleTokenUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={googleId}";
                                using (var httpClient = new HttpClient())
                                {
                                    var response = await httpClient.GetAsync(googleTokenUrl);

                                    if (response.IsSuccessStatusCode)
                                    {
                                        string responseBody = await response.Content.ReadAsStringAsync();
                                        var tokenInfo = JsonConvert.DeserializeObject<GoogleTokenInfo>(responseBody);

                                        if (tokenInfo.email == googlesignInrequest.Email)
                                        {
                                            // Update the Google ID in the database
                                            existingUserwithemail.GoogleId = googlesignInrequest.GoogleId;
                                            await _repository.UpdateAsync(existingUserwithemail);

                                            var resgoogleSignin = new GoogleSigninResponse
                                            {
                                                Success = true,
                                                UserId = existingUserwithemail.AdminUserid,
                                                verified = existingUserwithemail.ValidUser,
                                                textResponse = googlesignInrequest.lang == 1 ? "Google ID is updated  and user is verified ":"משתמש גוגל עודכן ברשומות אצלינו"
                                            };

                                            return resgoogleSignin;
                                        }
                                        else
                                        {
                                            var resgoogleSignin = new GoogleSigninResponse
                                            {
                                                Success = false,
                                                UserId =null,
                                                verified = false,
                                                textResponse = googlesignInrequest.lang == 1 ? "verfication failed" : " האימות נכשל " //google id you supplied doesnt match your credentials  "
                                            };
                                        }


                                    }
                                    else
                                    {
                                        var resgoogleSignin = new GoogleSigninResponse
                                        {
                                            Success = false,
                                            UserId = null,
                                            verified = false,
                                            textResponse = googlesignInrequest.lang == 1 ? "verfication failed" : " האימות נכשל " // "google service authentication failed please true again   "
                                        };
                                    }

                                }
                                
                            }
                        }
                    }
                    else
                    {
                        // If the user does not exist in the database, verify using Google API
                        string googleTokenUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={googleId}";
                        using (var httpClient = new HttpClient())
                        {
                            var response = await httpClient.GetAsync(googleTokenUrl);

                            if (response.IsSuccessStatusCode)
                            {
                                string responseBody = await response.Content.ReadAsStringAsync();
                                var tokenInfo = JsonConvert.DeserializeObject<GoogleTokenInfo>(responseBody);

                                if (tokenInfo.email == email)
                                {
                                    // Create a new row in the database
                                    // Define the time zone ID for Israel
                                    string israelTimeZoneId = "Israel Standard Time"; // This is the Windows time zone ID for Israel

                                    // Get the Israel time zone
                                    TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                                    // Convert server's DateTime.Now to Israel local time
                                    DateTime israelNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);
                                    var newUser = new AdminUsers
                                    {
                                        DateCreated = israelNow,
                                        ValidUser = true,
                                        Email = googlesignInrequest.Email,
                                        GoogleId = googlesignInrequest.GoogleId
                                        // Set other properties based on your requirement
                                    };
                                    // Add the new user to the database
                                    _repository.Create<AdminUsers>(newUser);

                                    var resgoogleSignin = new GoogleSigninResponse
                                    {
                                        Success = true,
                                        UserId = newUser.AdminUserid,
                                        verified = newUser.ValidUser,
                                        textResponse = googlesignInrequest.lang == 1 ? "user created succesfully" : " משתמש נוצר בהצלחה "
                                    };
                                    return resgoogleSignin;
                                    // return CreatedAtAction(nameof(GoogleSignIn), new { Message = "User created successfully" });
                                   // return true;
                                }
                                else
                                {
                                    // return BadRequest(new { Error = "Invalid email or Google ID" });
                                    var resgoogleSignin = new GoogleSigninResponse
                                    {
                                        Success = false,
                                        UserId =null,
                                        verified =false,
                                        textResponse = googlesignInrequest.lang == 1 ? "verfication failed" : " האימות נכשל " //"Invalid email or Google ID"
                                    };
                                    return resgoogleSignin;
                                    
                                }
                            }
                            else
                            {
                                //return BadRequest(new { Error = "Failed to verify user with Google API" });

                                var resgoogleSignin = new GoogleSigninResponse
                                {
                                    Success = false,
                                    UserId = null,
                                    verified = false,
                                    textResponse = googlesignInrequest.lang == 1 ? "verfication failed" : " האימות נכשל "  //"Failed to verify user with Google API"
                                };
                                return resgoogleSignin;
                               
                            }
                        }
                    }


                }
                var defaultResponse = new GoogleSigninResponse
                {
                    Success = false,
                    UserId = null,
                    verified = false,
                    textResponse = googlesignInrequest.lang == 1 ? "failed to verify with google account unexpected error" : " האימות מול גוגל נכשל"
                };
                return defaultResponse;




            }
            catch (Exception ex)
            {
                var resgoogleSignin = new GoogleSigninResponse
                {
                    Success = false,
                    textResponse = googlesignInrequest.lang == 1? "verfication failed" + ex.InnerException + ex.Message : " האימות נכשל "
                };
                return resgoogleSignin;
                
            }
        }
        public async Task<Q1_Q4_Result> GetQ1_Q4_Indication(Q1_Q4_Request q1q4request)
        {
            try
            {
                Businesses q1q2_res = null;
                UsersExternalSystemDynamicFields q3_res = null;
                q1q2_res = _repository.GetFirstObject<Businesses>(x => x.AdminUserid == q1q4request.Userid);
                if (q1q2_res != null)
                {
                    q3_res = _repository.GetFirstObject<UsersExternalSystemDynamicFields>(x => x.Userid == q1q4request.Userid && x.ExternalSystemId == q1q2_res.ExternalSystemId);
                }
                var Response = new Q1_Q4_Result
                {
                    Q1_Q2_InidicationRes = q1q2_res != null ? true : false,
                    Q3_InidicationRes = q3_res != null ? true : false,
                    BusinessID= q1q2_res==null?null: q1q2_res.BusinessId,
                    Fullname= q1q2_res==null?"": q1q2_res.FirstName+" "+ q1q2_res.LastName
                };
                return Response;
            }
            catch(Exception ex)
            {
                var Response = new Q1_Q4_Result
                {
                    Q1_Q2_InidicationRes = false,
                    Q3_InidicationRes = false,
                    BusinessID =null,
                    Fullname = ""
                };
                return Response;
            }
        }
        public async Task<LoginWithEmailandPasswordResponse> LoginWithEmailPasswordRequest(LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest)
        {
            try
            {
                Businesses q1q2_res=null; // Declare q1q2_res as Businesses type
                UsersExternalSystemDynamicFields q3_res=null; // Declare q3_res as UsersExternalSystemDynamicFields type
                var result = _repository.GetFirstObject<AdminUsers>(x => x.Email == _LoginWithEmailPasswordRequest.Email && x.passwordEncrypted== _LoginWithEmailPasswordRequest.Password);// && x.Password == model.Password x.Email == "admin@abc.com
                if (result != null)
                {
                    string israelTimeZoneId = "Israel Standard Time";
                    TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById(israelTimeZoneId);

                    DateTime? refreshTokenExpireTime = result.RefreshTokenExpireTime; // Assuming result.RefreshTokenExpireTime is of type DateTime?

                    // Use the null-conditional operator to handle nullable DateTime
                    DateTime israelRefreshTokenNow = refreshTokenExpireTime?.ToUniversalTime() ?? DateTime.UtcNow;
                    israelRefreshTokenNow = TimeZoneInfo.ConvertTimeFromUtc(israelRefreshTokenNow, israelTimeZone);


                    if (result != null)
                    {
                        q1q2_res = _repository.GetFirstObject<Businesses>(x => x.AdminUserid == result.AdminUserid);
                        if (q1q2_res != null)
                        {
                            q3_res = _repository.GetFirstObject<UsersExternalSystemDynamicFields>(x => x.Userid == result.AdminUserid && x.ExternalSystemId == q1q2_res.ExternalSystemId);
                        }
                        var Response = new LoginWithEmailandPasswordResponse
                        {
                            Q1_Q2_InidicationRes = q1q2_res != null ? true : false,
                            Q3_InidicationRes = q3_res != null ? true : false,
                            Userid = result.AdminUserid,
                            verified = result.ValidUser,
                            BusinessID = q1q2_res == null ? null : q1q2_res.BusinessId,
                            FullName = q1q2_res == null ? "" : q1q2_res.FirstName + " " + q1q2_res.LastName,
                            RefreshTokenExpiredTime = israelRefreshTokenNow
                        };
                        return Response;
                    }
                    else
                    {
                        var Response = new LoginWithEmailandPasswordResponse
                        {
                            Q1_Q2_InidicationRes = false,
                            Q3_InidicationRes = false,
                            Userid = 0,
                            verified = false,
                            FullName = "",
                            RefreshTokenExpiredTime = null
                        };
                        return Response;
                    }
                }
                else
                {
                    var Response = new LoginWithEmailandPasswordResponse
                    {
                        Q1_Q2_InidicationRes = false,
                        Q3_InidicationRes = false,
                        Userid = 0,
                        verified = false,
                        FullName = "",
                        RefreshTokenExpiredTime = null
                    };
                    return Response;

                }
              
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       

        public async Task<LoginWithOtpResponse> RegisterWithOtpAndEncryptedUser(string otp, string DecryptedUser,int Lng)
        {
            try
            {
                var res1 = new VerifyUserByOtpUserIdAndTimeStampResponse();

                var Otparam = new { Otp = otp, Userid= DecryptedUser };
                //var res = _repository.ExecuteGetSP<VerifyUserByOtpUserIdAndTimeStampResponse>(ConstUninetStoredprocedure.SP_VerifyUserByOtpUserIdAndTimeStamp, Otparam).ToList();


                var res = _repository.ExecuteGetSP<VerifyUserByOtpUserIdAndTimeStampResponse>(ConstUninetStoredprocedure.SP_VerifyUserByOtpUserIdAndTimeStamp, Otparam).ToList();




                if (res[0].Verified == 1)
                {
                    //eyal remark i removed the send mail welcome mail because it needed  to be call after Q1-Q5 completed
                    //_dataMailassist.sendsmtpmail("You are a new member in Uninet network", "eyalbmma@gmail.com", res[0].Email, 2, 1);
                    return new LoginWithOtpResponse()
                    {
                        verified = true,
                        description = Lng == 1 ? "User has been verified Succesfully" : "המשתמש אושר בהצלחה",
                        userId = DecryptedUser

                    };
                }
                else if (res[0].Verified == 2)//otp wrong
                {
                    return new LoginWithOtpResponse()
                    {
                        verified = false,
                        description = Lng == 1 ? "you have typed a wrong otp please try again" : "הקוד שהזנת אינו תקין אנא נסה שנית"

                    };
                }
                else if (res[0].Verified == 3)
                { //time 15 minutes passed 
                    return new LoginWithOtpResponse()
                    {
                        verified = false,
                        description = Lng == 1 ? "User otp has passed the time limit please register again to get new otp" : "הזמן שהוקצב לאישור הקוד אזל אנא נסה שנית "

                    };
                }
                else
                {
                    return new LoginWithOtpResponse()
                    {
                        verified = false,
                        description = Lng == 1 ? "an error occure please try again " : "ארעה שגיאה אנא נסה שנית "

                    };
                }
               
                      
               




            }
            catch (Exception ex)
            {
                return new LoginWithOtpResponse()
                {
                    verified = false,
                    description = Lng == 1 ? "an error occure please try again ":"ארעה שגיאה אנא נסה שנית "

                };

            }
        }


        public async Task<bool> SendOtpByPhone(SendOtpRequest _sendOtpRequest)
        {
            try
            {
                var result = _repository.GetFirstObject<AdminUsers>(x => x.PhoneNumber == _sendOtpRequest.Phone);// && x.Password == model.Password x.Email == "admin@abc.com
                if (result != null)
                {
                    string Otp = "";
                    Otp = Generate_otp();
                    bool SendOtpres =await SendOtp("+19142668348", "+972526350902", Otp);
                    if (SendOtpres)
                    {

                        var OtpRaw = new { Userid = result.AdminUserid, otp = Otp };
                        var res = _repository.ExecuteGetSP<UpdateOtpResponse>(ConstUninetStoredprocedure.SP_updateInsertOTP, OtpRaw).ToList();



                    }

                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
