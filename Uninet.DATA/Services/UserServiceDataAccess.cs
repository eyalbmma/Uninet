using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic.FileIO;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ThirdParty.Json.LitJson;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
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

        private readonly IUninetInputDataAccess _UninetInputDataAccess;
        public IConfiguration Configuration { get; }
        public UserServiceDataAccess(IRepository<UninetContext> repository, IConfiguration configuration, IDataMailassist dataMailassist, IUninetInputDataAccess uninetInputDataAccess, IMongoClient client)//, IloginRepository loginRepository
        {

            var database = client.GetDatabase("Uninet");
            _ICountCollection = database.GetCollection<BsonDocument>("Icount");
            _ICountCompanyInfoCollection = database.GetCollection<BsonDocument>("IcountCompanisInfo");
            _ICountDocInfoCollection = database.GetCollection<BsonDocument>("IcountDocInfo");
            _IcountClientInfoCollection= database.GetCollection<BsonDocument>("icountClientInfo");
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
        public void InsertDocumentInfo(JsonElement jsonData, MongoDbDestination destination)
        {
            // Convert the JsonElement to a BsonDocument
            BsonDocument document = BsonDocument.Parse(jsonData.GetRawText());

            // Check if the document already exists in the collection
            if (destination == MongoDbDestination.DocinfoDB)
            {
                var docnum = document["docnum"];
                var filter = Builders<BsonDocument>.Filter.Eq("docnum", docnum);
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

                var filter = Builders<BsonDocument>.Filter.Eq("client_info.client_id", client_id);
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
                    Console.WriteLine($"Document with client_id '{client_id}' already exists.");
                }
            }

        }




        public async Task<bool> ExtractClientIdsAndInsertToMongoDb(JsonElement resultsList, string cidvalue, string uservalue, string passvalue)
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
                InsertDocumentInfo(jsonData, destination);
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
                InsertDocumentInfo(jsonData, destination);

            }

            return true;


                

           
        }
        public async Task SetLastPullDataDate(int companyVatid)
        {
            

            var existingRow = await _repository.GetByIdAsync<CompanyPulledDataLog>(companyVatid);

            if (existingRow != null)
            {
                existingRow.LastPullDataDate = DateTime.Now;
                await _repository.UpdateAsync(existingRow);
            }
            else
            {
                var newRow = new CompanyPulledDataLog
                {
                    CompanyVatid = companyVatid,
                    LastPullDataDate = DateTime.Now
                };

                await _repository.CreateAsync(newRow);
            }

        }
        public async Task<bool> SaveExternalCustomizedExternalSystemId(SpInputExternalSystemCompanyDetails spInputExternalSystemCompanyDetails, string UserId)
        {
            try
            {
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
                
               


                bool spresult = ExecuteGetSP(ConstUninetStoredprocedure.SP_SaveUsersExternalSystemDynamicFieldsData, UserParam);
                if (spresult)
                {
                    /// here comes the logic of calling web api of external system (mvp external is icount)
                    /// to get the documents of the user  that was just registered to the system
                    string cidvalue = null;
                    string uservalue = null;
                    string passvalue = null;
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

                    ///get the comopany info to know what was the started date to get documents from this started date

                    var comopanyinfoEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 70); /// call-https://api.icount.co.il/api/v3.php/company/info
                    var endpointcomopanyinfo = comopanyinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                    HttpMethod methodcomopanyinfo = HttpMethod.Get;
                    var ReponsneCompanyInfo=await _UninetInputDataAccess.SendRequest(endpointcomopanyinfo, methodcomopanyinfo);

                    ////this part insert a ReponsneCompanyInfo modified with new property InternalCompanyId to the new collection IcountCompanisInfo 
                    ///
                   

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
                        await _ICountCompanyInfoCollection.InsertOneAsync(modifiedCompanyInfo);
                    }
                    else
                    {
                        // Document with the same vat_id already exists, handle accordingly
                        Console.WriteLine("Document with the same vat_id already exists");
                    }



                    string vatid = "";
                    DateTime startPulldata = new DateTime();
                    DateTime EndPulldata = new DateTime(); 
                    string propertyPathstart_date = "company_info.start_date";
                    if (ReponsneCompanyInfo != null)
                    {
                        DateTime startDate = ExtractPropertyValue<DateTime>(ReponsneCompanyInfo.ToString(), propertyPathstart_date);



                        string propertyPathVatid = "company_info.vat_id";
                         vatid = ExtractPropertyValue<string>(ReponsneCompanyInfo.ToString(), propertyPathVatid);
                         startPulldata = startDate;
                         EndPulldata = DateTime.Now;
                         EndPulldata = EndPulldata.AddDays(-7);
                        var companyPulledDataLog = await _repository.FindAsync<CompanyPulledDataLog>(log => log.CompanyVatid == Convert.ToInt32(vatid));
                        if (companyPulledDataLog != null)
                        {
                            startPulldata = companyPulledDataLog.LastPullDataDate;
                        }

                        await SetLastPullDataDate(Convert.ToInt32(vatid));
                    }
                    var docsearchEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 71);//call icount-https://api.icount.co.il/api/v3.php/doc/search

                    string endpointUrldocsearch = docsearchEndpoint.Endpoint + "?cid="+ cidvalue+"&user="+uservalue+ "&pass=" + passvalue+"&start_ts="+ startPulldata.ToString()+"&end_ts="+ EndPulldata;
                    HttpMethod methoddocsearch = HttpMethod.Get;
                    var Reponsnedocsearch = await _UninetInputDataAccess.SendRequest(endpointUrldocsearch, methoddocsearch);
                    
                    if (Reponsnedocsearch != null)
                    {

                        // insert  compay cutomer invoices to mongodb collection name icount
                        JsonDocument jsonDocument = JsonDocument.Parse(Reponsnedocsearch.ToString());
                        JsonElement resultsList = jsonDocument.RootElement.GetProperty("results_list");
                        InsertDocumentsToMongoDB(resultsList, vatid, UserId, spInputExternalSystemCompanyDetails.Companyid);


                        //loop and the invoce list resultsList and get for each client a detailed client data from --https://api.icount.co.il/api/v3.php/client/info
                        var resExtractClientIds = await ExtractClientIdsAndInsertToMongoDb(resultsList, cidvalue, uservalue, passvalue);



                        //loop on all invoice and get  for each invoce a detailed invoce  and save it in icountdocinfo collection
                        //https://api.icount.co.il/api/v3.php/doc/info?cid=uninetttt&user=eyalberda&pass=Ilayshaked10&doctype=invoice&docnum=2002
                        //foreach invoce in resultsList get property value of doctype and docnum
                        var res = await CreateListOfDetailedDocinfoAndInsertToMongoDBCollection(resultsList, cidvalue, uservalue, passvalue);


                    }







                    //// Call with jwtToken provided
                    //string jwtToken = "your-jwt-token";
                    //string responseData2 = await SendRequest(endpointUrl, method, jwtToken);
                    //Console.WriteLine(responseData2);


                    //
                }
                return spresult;
                

            }
            catch (Exception ex) { return false; };
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
                var filter = Builders<BsonDocument>.Filter.Eq("docnum", document["docnum"]);
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

        public async Task<Dictionary<int, string>> GetExternalSystems()
        {
            var hashtable = _repository.GetHashtableOfExternalFields();

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

                var res = _repository.GetListOfFiledObjects(ExternalSystemId);


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
                            BusinessRequests = existingBusinessRequests
                        };
                        return result;
                    }
                    else
                    {
                        var dataTable = new DataTable();
                        dataTable.Columns.Add("BusinessId", typeof(int));
                        dataTable.Columns.Add("BusinessType", typeof(int));
                        dataTable.Columns.Add("FirstName", typeof(string));
                        dataTable.Columns.Add("LastName", typeof(string));
                        dataTable.Columns.Add("MobileNumber", typeof(string));
                        dataTable.Columns.Add("OrganizationRole", typeof(string));
                        dataTable.Columns.Add("OrganizationName", typeof(string));
                        dataTable.Columns.Add("OrganizationType", typeof(int));
                        dataTable.Columns.Add("ExternalSystemId", typeof(int));

                        foreach (var businessRequest in userBusinesses.BusinessRequests)
                        {
                            dataTable.Rows.Add(
                                businessRequest.BusinessId,
                                businessRequest.BusinessType,
                                businessRequest.FirstName,
                                businessRequest.LastName,
                                businessRequest.MobileNumber,
                                businessRequest.OrganizationRole,
                                businessRequest.OrganizationName,
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
                            BusinessRequests = addedBusinessRequestModels
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
                return null;
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
        public async Task<ReturnRegisterUser> RegisterUser(RegisterUserRequest RegisterUserReq)
        {
            try
            {
                var NewUser = new AdminUsers
                {

                    //FirstName = RegisterUserReq.FirstName,
                    //LastName = RegisterUserReq.LastName,
                    //PhoneNumber = RegisterUserReq.PhoneNumber,
                    DateCreated = DateTime.Now,
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
                        UserStatusIndication = 1
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
                        UserStatusIndication = 0
                    };
                    return returnuser;
                   
                }
                   
               
            }
            catch (Exception ex)
            {
                return null;//exception
            }
        }

        
        public async Task<LoginWithEmailandPasswordResponse> LoginWithEmailPasswordRequest(LoginWithEmailPasswordRequest _LoginWithEmailPasswordRequest)
        {
            try
            {
                var result = _repository.GetFirstObject<AdminUsers>(x => x.Email == _LoginWithEmailPasswordRequest.Email && x.passwordEncrypted== _LoginWithEmailPasswordRequest.Password);// && x.Password == model.Password x.Email == "admin@abc.com
                var Response = new LoginWithEmailandPasswordResponse
                {

                    Userid = result.AdminUserid
                };
               return Response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
       

        public async Task<LoginWithOtpResponse> RegisterWithOtpAndEncryptedUser(string otp, string DecryptedUser)
        {
            try
            {
                var res1 = new VerifyUserByOtpUserIdAndTimeStampResponse();

                var Otparam = new { Otp = otp, Userid= DecryptedUser };
                //var res = _repository.ExecuteGetSP<VerifyUserByOtpUserIdAndTimeStampResponse>(ConstUninetStoredprocedure.SP_VerifyUserByOtpUserIdAndTimeStamp, Otparam).ToList();


                var res = _repository.ExecuteGetSP<VerifyUserByOtpUserIdAndTimeStampResponse>(ConstUninetStoredprocedure.SP_VerifyUserByOtpUserIdAndTimeStamp, Otparam).ToList();

    






                if (res != null)
                {
                    if (res[0].Verified)
                    {
                        //send mail welcome mail to user after he loged in with otp
                        _dataMailassist.sendsmtpmail("You are a new member in Uninet network", "eyalbmma@gmail.com", res[0].Email, 2, 1);
                        return new LoginWithOtpResponse()
                        {
                            verified = res[0].Verified,
                            description = "User has been verified Succesfully",
                            userId = DecryptedUser

                        };
                    }else
                    {
                        return new LoginWithOtpResponse()
                        {
                            verified = res[0].Verified,
                            description = "User otp has passed the time limit please register again to get new otp"

                        };
                    }
                    
                }
                else
                    return new LoginWithOtpResponse()
                    {
                        verified = res[0].Verified,
                        description = "User otp has passed the time limit please register again to get new otp"

                    };




            }
            catch (Exception ex)
            {
                return null;
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
