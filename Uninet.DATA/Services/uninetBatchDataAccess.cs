using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Uninet.Domain.Entities;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Interfaces;
using MongoDB.Driver;
using Uninet.DATA.Interfaces;
using Microsoft.Data.SqlClient;
using System.Data;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;
using Newtonsoft.Json;
using System.Text.Json;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Requests;
using Twilio.Rest.Api.V2010.Account.Usage.Record;
using static Uninet.DATA.Services.UserServiceDataAccess;
using Amazon.Runtime.Internal.Util;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace Uninet.DATA.Services
{
    public  class uninetBatchDataAccess: IuninetBatchDataAccess
    {
        private readonly IBatchRepository<UninetBatchContext> _repository;
        private readonly IMongoCollection<BsonDocument> _Uninetgreenvoicedocument;

        private readonly IMongoCollection<BsonDocument> _ICountCollection;
        private readonly IMongoCollection<BsonDocument> _ICountDocInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountClientInfoCollection;
        private readonly IBatchDataMailassist _batchdataMailassist;
        //private readonly IMailassist _mailasist;
        public uninetBatchDataAccess(IBatchRepository<UninetBatchContext> repository, IMongoClient client, IBatchDataMailassist batchdataMailassist)//, IloginRepository loginRepository
        {
            var database = client.GetDatabase("Uninet");
            _batchdataMailassist = batchdataMailassist;

            _ICountCollection = database.GetCollection<BsonDocument>("Icount");
            _ICountDocInfoCollection = database.GetCollection<BsonDocument>("IcountDocInfo");
            _IcountClientInfoCollection = database.GetCollection<BsonDocument>("icountClientInfo");

            var Uninetgreenvoicedocument = database.GetCollection<BsonDocument>("UninetGreenVoiceCollection");
            _Uninetgreenvoicedocument = Uninetgreenvoicedocument;
            
            
            
            
            _repository = repository;
            
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

                for (var i = 0; i < InputData.Count; i++)
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
                    catch (Exception ex) { }

                }
                //extract documnet from uninet mongo
                return true;
            }
            catch (Exception ex) { return false; }
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
        public async Task SetLastPullDataDate(CompanyPulledDataLog companyPulledDataLog,int companyVatid)
        {


            

            if (companyPulledDataLog != null)
            {
                companyPulledDataLog.LastPullDataDate = DateTime.Now;
                await _repository.UpdateAsync(companyPulledDataLog);
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
        public void InsertDocumentsToMongoDB(JsonElement resultsList, string vatid, string InternalUserId, int InternalComopanyId)
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
            { "vat_id", vatid } ,
            {"InternalCompanyId",InternalComopanyId.ToString() },
            {"InternalUserid", InternalUserId}
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
        public void InsertDocumentInfo(JsonElement jsonData, MongoDbDestination destination,string SupplierVat_id)
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
                var ReponsneClientInfo = await SendRequest(endpointClinetinfo, methodclientinfo);
                JsonDocument jsonDocument = JsonDocument.Parse(ReponsneClientInfo);
                JsonElement jsonData = jsonDocument.RootElement;

                MongoDbDestination destination = MongoDbDestination.ClientInfoDB;
                InsertDocumentInfo(jsonData, destination, SupplierVat_id);
            }
            return true;
        }
        //we get to this function with single row a userid  from AdminUsers table

        public async Task<bool> CreateListOfDetailedDocinfoAndInsertToMongoDBCollection(JsonElement resultsList, string cidvalue, string uservalue, string passvalue)
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
                var ReponsneDocInfo = await SendRequest(endpointdocInfo, methoddocinfo);

                JsonDocument jsonDocument = JsonDocument.Parse(ReponsneDocInfo);
                JsonElement jsonData = jsonDocument.RootElement;
                MongoDbDestination destination = MongoDbDestination.DocinfoDB;
                InsertDocumentInfo(jsonData, destination,"");//i dont pass  SupplierVat_id to doc info because i dont add the attribute SupplierVat_id to the nodes of the collection

            }

            return true;





        }

        public async Task<string> PullUserDatafromExternalSystem(int Userid)
        {
            List<Businesses> res = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == Userid);

            //loop on [dbo].[Businesses] for the same userid that may have many businesses related to him
            foreach (var Business in res)
            {
                var UserexternalSystemDynamicFieldslist = _repository.GetListOfObjects<UsersExternalSystemDynamicFields>(x => x.Companyid == Business.BusinessId && x.Userid== Business.AdminUserid);

                //List<CustomizedDataLIst> ListInputLabelDetails { get; set; }

                string cidvalue = null;
                string uservalue = null;
                string passvalue = null;
                

                foreach (var dynamicField in UserexternalSystemDynamicFieldslist)
                {
                    string fieldLabelName = dynamicField.FieldLabelName;
                    string fieldLabelValue = dynamicField.FieldLabelValue;

                    if (fieldLabelName == "cid")
                    {
                         cidvalue = fieldLabelValue;
                        // Use the cid value as needed
                    }
                    else if (fieldLabelName == "user")
                    {
                         uservalue = fieldLabelValue;
                        // Use the user value as needed
                    }
                    else if (fieldLabelName == "pass")
                    {
                        passvalue = fieldLabelValue;
                        // Use the pass value as needed
                    }
                }

                ///get the comopany info to know what was the started date to get documents from this started date

                var comopanyinfoEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 70); /// call-https://api.icount.co.il/api/v3.php/company/info
                var endpointcomopanyinfo = comopanyinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                HttpMethod methodcomopanyinfo = HttpMethod.Get;
                var ReponsneCompanyInfo = await SendRequest(endpointcomopanyinfo, methodcomopanyinfo);
                //exstract the date started from companyinfo
                // string jsonResponse = "Your JSON response goes here";
                string propertyPathstart_date = "company_info.start_date";

                DateTime startDate = ExtractPropertyValue<DateTime>(ReponsneCompanyInfo.ToString(), propertyPathstart_date);



                string propertyPathVatid = "company_info.vat_id";
                string vatid = ExtractPropertyValue<string>(ReponsneCompanyInfo.ToString(), propertyPathVatid);
                DateTime startPulldata = startDate;
                DateTime EndPulldata = DateTime.Now;
                int intVatid = Convert.ToInt32(vatid.TrimStart('0'));
                CompanyPulledDataLog companyPulledDataLog =  _repository.GetFirstObject<CompanyPulledDataLog>(x => x.CompanyVatid == intVatid);
                if (companyPulledDataLog != null)
                {
                    startPulldata = companyPulledDataLog.LastPullDataDate;
                }
                startPulldata= startPulldata.AddDays(-4);//added eyal becuse icount doesnt bring exact data
                await SetLastPullDataDate(companyPulledDataLog, intVatid);

                var docsearchEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 71);//call icount-https://api.icount.co.il/api/v3.php/doc/search

                string endpointUrldocsearch = docsearchEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue + "&start_ts=" + startPulldata.ToString() + "&end_ts=" + EndPulldata;
                HttpMethod methoddocsearch = HttpMethod.Get;
                var Reponsnedocsearch = await SendRequest(endpointUrldocsearch, methoddocsearch);



                // insert  compay cutomer invoices to mongodb collection name icount
                JsonDocument jsonDocument = JsonDocument.Parse(Reponsnedocsearch.ToString());

                string SupplierVat_id = vatid; //we send thie vat it to add it to the icountClientInfo so that each node of client will have its suplier_vat_id

                //JsonElement resultsList = jsonDocument.RootElement.GetProperty("results_list");
                if (jsonDocument.RootElement.TryGetProperty("results_list", out JsonElement resultsListElement) &&
                                resultsListElement.ValueKind == JsonValueKind.Array && resultsListElement.GetArrayLength() > 0)
                {
                    // results_list exists and has items
                    JsonElement resultsList = resultsListElement;


                    InsertDocumentsToMongoDB(resultsList, vatid, Userid.ToString(), Business.BusinessId);


                    //loop and the invoce list resultsList and get for each client a detailed client data from --https://api.icount.co.il/api/v3.php/client/info
                    var resExtractClientIds = await ExtractClientIdsAndInsertToMongoDb(resultsList, cidvalue, uservalue, passvalue,SupplierVat_id);



                    //loop on all invoice and get  for each invoce a detailed invoce  and save it in icountdocinfo collection
                    //https://api.icount.co.il/api/v3.php/doc/info?cid=uninetttt&user=eyalberda&pass=Ilayshaked10&doctype=invoice&docnum=2002
                    //foreach invoce in resultsList get property value of doctype and docnum
                    var res1 = await CreateListOfDetailedDocinfoAndInsertToMongoDBCollection(resultsList, cidvalue, uservalue, passvalue);


                }




            }
            /*

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
                 var response = await client.GetAsync("https://localhost:7285/api/GreenInvoice/PullBusinessDataByTaxid/" + Business.BusinessId);


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
             */
            return string.Empty;
        }





        public async Task<string> ExtractUserCompanyLogicExpensesAndSendAsExpensesToSideB(BusinessRequestFoeExpenses businessRequest)
        {


            var filter = Builders<BsonDocument>.Filter.Eq("InternalCompanyId", businessRequest.BusinessId) &
                 Builders<BsonDocument>.Filter.Eq("InternalUserid", businessRequest.AdminUserid);

            var items = await _ICountCollection.Find(filter).ToListAsync();


            List<ClientInfo> clientInfoList = new List<ClientInfo>();
            //in the items list we have all items (receipt,invoice etc.. ) from  icount 
            //new we need to loop each one and get his client_id
            foreach (var item in items)
            {
                var clientId = item.GetValue("client_id").AsString;
                var SenderBusinessId = item.GetValue("vat_id").AsString;
                var filterClientinfo = Builders<BsonDocument>.Filter.And(
    Builders<BsonDocument>.Filter.Eq("client_info.client_id", clientId),
    Builders<BsonDocument>.Filter.Eq("client_info.SupplierVat_id", SenderBusinessId)
);
                var ClientInfoitems = await _IcountClientInfoCollection.Find(filterClientinfo).ToListAsync();

                

                foreach (var Clientitem in ClientInfoitems)
                {
                    var vatId = Clientitem["client_info"]["vat_id"].AsString;
                    var companyName = Clientitem["client_info"]["company_name"].AsString;
                    var clientName = Clientitem["client_info"]["client_name"].AsString;
                    var email = Clientitem["client_info"]["email"].AsString;
                    var mobile = Clientitem["client_info"]["mobile"].AsString;

                    if (!clientInfoList.Any(c => c.VatId == vatId))
                    {
                        var clientInfo = new ClientInfo
                        {
                            VatId = vatId,
                            CompanyName = companyName,
                            ClientName = clientName,
                            Email = email,
                            Mobile = mobile,
                            SenderName = businessRequest.FirstName + " " + businessRequest.LastName,
                            BusinessSenderVatid= SenderBusinessId
                        };

                        clientInfoList.Add(clientInfo);
                    }

                }





                // The clientInfoList now contains the objects for the matched items









            }
            ///now we loop over all clientInfoList that has cliet info and for each client get his docinfo from IcountDocInfo collection
            ///and send  to the client email a mail  with a link to his pdf
            // Iterate through the clientInfoList
            foreach (var clientInfo in clientInfoList)
            {
                string clientVatId = clientInfo.VatId; // Replace VatId with the actual property name in the clientInfo object
                var filterClientDocinfo = Builders<BsonDocument>.Filter.Eq("doc_info.vat_id", clientVatId);
                var clientDocInfoItems = await _ICountDocInfoCollection.Find(filterClientDocinfo).ToListAsync();

                foreach (var clientDocInfoItem in clientDocInfoItems)
                {
                    string docUrl = clientDocInfoItem["doc_info"]["doc_url"].ToString();
                    string _doctype= clientDocInfoItem["doctype"].ToString();
                    var _RequestMailObject = new RequestedMailObject
                    {
                        Sendername = clientInfo.SenderName,
                        DocType = _doctype,
                        RecipientName = clientInfo.ClientName,
                        DocLink= docUrl
                    };

                    //remark eyal need to insert to table BusinessData 
                    var UserParam = new
                    {

                        UserId = businessRequest.AdminUserid,
                        BusinessId= businessRequest.BusinessId,
                        BusinessVatId= clientInfo.BusinessSenderVatid,
                        JsonDocumentid =clientDocInfoItem["_id"].ToString(),
                        ClientVat_id= clientVatId,
                        client_name= clientInfo.ClientName,
                        DataSourceEnum=2,
                        ClientEmail= clientInfo.Email,
                        EmailSent=false
                       

                    };
                    var result = _repository.ExecuteGetSP<InsertBusinessData_Result>(ConstUninetStoredprocedure.SP_InsertBusinessData, UserParam);
                    try
                    {
                        bool spresult = result.ToList()[0].Success;
                        if (spresult)
                        {
                            var resmail = await _batchdataMailassist.sendsmtpmail(" UNINET מסמך הגיע אליך מ  ", "eyalbmma@gmail.com", clientInfo.Email, 4, 1, _RequestMailObject);
                            if (resmail.result)
                            {
                                var UserParam2 = new
                                {

                                    UserId = businessRequest.AdminUserid,
                                    BusinessId = businessRequest.BusinessId,
                                    JsonDocumentid = clientDocInfoItem["_id"].ToString()
                                    


                                };
                                var resultupdate = _repository.ExecuteGetSP<UpdateBusinessDataEmailSent_Response>(ConstUninetStoredprocedure.SP_UpdateBusinessDataEmailSent, UserParam2).ToList();
                            }
                        }
                    }
                    catch (Exception ex) { }

                   
                   
                    
                   

                    // Use the docUrl as needed
                }




            }


            return "Success"; // Return the appropriate response
        }




      


    }
}
