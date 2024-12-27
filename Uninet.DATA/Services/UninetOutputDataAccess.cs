using Amazon.Runtime.Internal.Transform;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Classes;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Numerics;
using Twilio.TwiML.Voice;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;
using Amazon.Runtime.Internal.Endpoints.StandardLibrary;
using Task = System.Threading.Tasks.Task;
using System.Net;
//Refactor the following method to improve performance


namespace Uninet.DATA.Services
{
    public class ApiResponse
    {
        [JsonProperty("api")]
        public ApiDetailsCurrencyInfo Api { get; set; }

        [JsonProperty("status")]
        public bool Status { get; set; }

        [JsonProperty("reason")]
        public string Reason { get; set; }

        [JsonProperty("currency_rates")]
        public Dictionary<string, decimal> CurrencyRates { get; set; }
    }

    public class ApiDetailsCurrencyInfo
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("tz")]
        public int Tz { get; set; }

        [JsonProperty("ts")]
        public double Ts { get; set; }

        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("rid")]
        public int Rid { get; set; }

        [JsonProperty("module")]
        public string Module { get; set; }

        [JsonProperty("method")]
        public string Method { get; set; }
    }
    public class CurrencyRateRequest
    {
        public string sid { get; set; }
        public string cid { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
    }

    public class ApiResponseinfo
    {
        public ApiDetails Api { get; set; }
        public bool Status { get; set; }
        public string Reason { get; set; }
        public string Currency { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySign { get; set; }
    }

    public class ApiDetails
    {
        public int Version { get; set; }
        public int Tz { get; set; }
        public double Ts { get; set; }
        public string Lang { get; set; }
        public int Rid { get; set; }
        public string Module { get; set; }
        public string Method { get; set; }
    }

    public class CurrencyId
    {
        [JsonProperty("Base type")]
        public int BaseType { get; set; }
    }

    public class CurrencyCode
    {
        [JsonProperty("Base type")]
        public string BaseType { get; set; }
    }

    public class CurrencyInfo
    {
        public string sid { get; set; }
        public string cid { get; set; }
        public string user { get; set; }
        public string pass { get; set; }
        public CurrencyId currency_id { get; set; }
        public CurrencyCode currency_code { get; set; }
        public string Currency { get; set; }
    }

    public class UninetOutputDataAccess : IUninetOutputDataAccess
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IMongoCollection<BsonDocument> _ICountCollection;
        private readonly IMongoCollection<BsonDocument> _ICountDocInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountClientInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountCompaniesInfoCollection;
        private readonly IMongoCollection<BsonDocument> _MorningCompanisInfoCollection;
        private readonly IMongoCollection<BsonDocument> _MorningClientSuppliers;
        private readonly IMongoCollection<BsonDocument> _MorningExpenses;
        
        private readonly IMongoCollection<BsonDocument> _IcountClientSuppliers;
        private readonly IMongoCollection<BsonDocument> _IcountExpenses;
        private readonly IMongoCollection<BsonDocument> _IcountExpensesTypes;
        private readonly IMongoCollection<BsonDocument> _MorningExpensesTypes;
        
        private readonly IMongoCollection<BsonDocument> _IcountWebhookData;
        private readonly IMongoCollection<BsonDocument> _MorningWebHookData;
        private readonly IMongoCollection<BsonDocument> _Icount_BussinesPartner_Clients_Suplliers;
        private readonly IDataMailassist _dataMailassist;
        private readonly Dictionary<int, ExternalSystemConfig> _externalSystemConfig;
        

        //IcountWebhookData
        public UninetOutputDataAccess(IRepository<UninetContext> repository, IMongoClient client, IDataMailassist dataMailassist)//, IloginRepository loginRepository
        {
            var database = client.GetDatabase("Uninet");
            _ICountCollection = database.GetCollection<BsonDocument>("Icount");
            _ICountDocInfoCollection = database.GetCollection<BsonDocument>("IcountDocInfo");
            _IcountClientInfoCollection = database.GetCollection<BsonDocument>("icountClientInfo");
            _IcountCompaniesInfoCollection = database.GetCollection<BsonDocument>("IcountCompanisInfo");
            _MorningCompanisInfoCollection= database.GetCollection<BsonDocument>("MorningCompanisInfo");
            _IcountClientSuppliers = database.GetCollection<BsonDocument>("icountClientSuppliers");
            _IcountExpenses = database.GetCollection<BsonDocument>("IcountExpenses");
            _IcountExpensesTypes = database.GetCollection<BsonDocument>("IcountExpensesTypes");
            _IcountWebhookData = database.GetCollection<BsonDocument>("IcountWebhookData");
            _MorningWebHookData= database.GetCollection<BsonDocument>("MorningWebHookData");
            _MorningClientSuppliers= database.GetCollection<BsonDocument>("MorningClientSuppliers");
            _MorningExpensesTypes = database.GetCollection<BsonDocument>("MorningExpensesTypes");
            _MorningExpenses= database.GetCollection<BsonDocument>("MorningExpenses");
            _Icount_BussinesPartner_Clients_Suplliers = database.GetCollection<BsonDocument>("Icount_BussinesPartner_Clients_Suplliers");
            _repository = repository;
            _dataMailassist = dataMailassist;
            // Initialize External System Config
            _externalSystemConfig = new Dictionary<int, ExternalSystemConfig>
    {
        {
            2, // iCount System
            new ExternalSystemConfig
            {
                CompaniesInfoCollection = _IcountCompaniesInfoCollection,
                WebhookCollection = _IcountWebhookData,
                ClientSupplierCollection= _IcountClientSuppliers,
                CompanyFieldPath = "company_info.InternalCompanyId",
                SubCompanyFieldPath = "company_info.SubCompanyId",
                VatFieldPath = "company_info.vat_id",
                SupplierFieldPath = "company_info.businessName",
                UrlFieldPath = "doc_info.doc_url_copy" // Path for document URL
            }
        },
        {
            6, // Morning System
            new ExternalSystemConfig
            {
                CompaniesInfoCollection = _MorningCompanisInfoCollection,
                WebhookCollection = _MorningWebHookData,
                ClientSupplierCollection=_MorningClientSuppliers,
                CompanyFieldPath = "InternalCompanyId",
                SubCompanyFieldPath = "SubCompanyId",
                VatFieldPath = "taxId",
                SupplierFieldPath = "name",
                UrlFieldPath = "files.downloadLinks.processedUrl" // Path for document URL
            }
        }
        // Add more systems as needed
    };

        }
        private async Task<string> SendRequest(string endpointUrl, HttpMethod method, string postData = null)
        {
            string result = "";
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage request = new HttpRequestMessage(method, endpointUrl);

                // Check the HTTP method and set the appropriate request content if it's a POST request
                if (method == HttpMethod.Post)
                {

                    request.Content = new StringContent(postData, Encoding.UTF8, "application/json");
                }

                HttpResponseMessage response = await client.SendAsync(request);

                response.EnsureSuccessStatusCode(); // Throw an exception if the request is not successful

                string responseData = await response.Content.ReadAsStringAsync();
                result = responseData;
            }

            return result;
        }


        /// <summary>
        /// this function GetClientSUpplierLIst query icount  when the client login to uninet and click on approve digitaldocument  and look for all his suppliers
        /// </summary>
        /// <param name="cidvalue"></param>
        /// <param name="uservalue"></param>
        /// <param name="passvalue"></param>
        /// <returns></returns>
        //private async Task<List<SupplierItem>> GetClientSupplierList(int Userid)
        //{
        //    var InternalCompanyId = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == Userid);

        //    var filter = Builders<BsonDocument>.Filter.And(
        //        Builders<BsonDocument>.Filter.Eq("internalcompanid", InternalCompanyId.BusinessId),
        //        Builders<BsonDocument>.Filter.Eq("UserID", Userid)
        //    );

        //    var document = await _IcountClientSuppliers.Find(filter).FirstOrDefaultAsync();
        //    if (document != null)
        //    {
        //        var suppliersData = document["suppliers"].AsBsonDocument;
        //        var supplierList = new List<SupplierItem>();

        //        // Loop through the suppliers data and map it to SupplierItem objects
        //        foreach (var supplier in suppliersData)
        //        {
        //            if (supplier.Value is BsonDocument supplierObject)
        //            {
        //                var vatIdValue = supplierObject.TryGetValue("vat_id", out var vatId) ? vatId.AsString : null;

        //                var supplierItem = new SupplierItem
        //                {
        //                    supplier_id = Convert.ToInt32(supplier.Name),
        //                    vat_id = string.IsNullOrEmpty(vatIdValue) ? 0 : Convert.ToInt32(vatIdValue),
        //                    supplier_name = supplierObject.TryGetValue("supplier_name", out var supplierName) ? supplierName.AsString : null,
        //                    company_name = supplierObject.TryGetValue("company_name", out var companyName) ? companyName.AsString : null
        //                };

        //                supplierList.Add(supplierItem);
        //            }
        //        }

        //        return supplierList;
        //    }
        //    else
        //    {
        //        return null;
        //    }
        //}

        private async Task<List<SupplierItem>> GetClientSupplierList(string cidvalue, string uservalue, string passvalue)
        {
            var ClinetinfoEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);  ////api.icount.co.il/api/v3.php/supplier/get_list
            var endpointClinetinfo = ClinetinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
            HttpMethod methodclientinfo = HttpMethod.Get;
            var ReponsneClientInfo = await SendRequest(endpointClinetinfo, methodclientinfo);

            // Parse the JSON response
            var jsonDocument = JsonDocument.Parse(ReponsneClientInfo);
            var jsonData = jsonDocument.RootElement;
            var suppliersData = jsonData.GetProperty("suppliers");

            // Create a list to store SupplierItem objects
            var supplierList = new List<SupplierItem>();

            // Loop through the suppliers data and map it to SupplierItem objects
            foreach (var supplier in suppliersData.EnumerateObject())
            {
                var supplierItem = new SupplierItem
                {
                    supplier_id = supplier.Name,
                    vat_id = supplier.Value.GetProperty("vat_id").GetString()==""?0: Convert.ToInt32(supplier.Value.GetProperty("vat_id").GetString()),

                    // Handle double quotes in supplier_name and company_name
                    supplier_name = supplier.Value.GetProperty("supplier_name").GetString()?.Replace("\"", ""),
                    company_name = supplier.Value.GetProperty("company_name").GetString()?.Replace("\"", ""),
                };

                supplierList.Add(supplierItem);
            }

            return supplierList;
        }

        private async Task<(string ExpenseTypeId, string ExpenseTypeName)> GetRecentExpenseTypeInfo(BsonDocument expenseDocument, string supplierId)
        {
            // Parse the results_list
            var resultsList = expenseDocument["results_list"].AsBsonDocument;

            DateTime mostRecentDate = DateTime.MinValue;
            BsonDocument mostRecentExpense = null;

            foreach (var expense in resultsList.Elements)
            {
                var expenseDetails = expense.Value.AsBsonDocument;
                if (expenseDetails["supplier_id"].AsString == supplierId)
                {
                    var expenseDate = DateTime.Parse(expenseDetails["expense_add_date"].AsString);
                    if (expenseDate > mostRecentDate)
                    {
                        mostRecentDate = expenseDate;
                        mostRecentExpense = expenseDetails;
                    }
                }
            }

            if (mostRecentExpense != null)
            {
                // Extract expense_type_id and expense_type_name
                var expenseTypeId = mostRecentExpense["expense_type_id"].AsString;
                var expenseTypeName = mostRecentExpense["expense_type_name"].AsString;

                return (expenseTypeId, expenseTypeName);
            }

            return (null, null); // Or appropriate default values
        }
        private async Task<List<ExpenseType>> PostExpenseTypeListMorning(string endpoint, Dictionary<string, string> requestBody, int? expenseTypeId)
        {
            try
            {
                string postData = JsonConvert.SerializeObject(requestBody);
                string response = await SendRequestWithToken(endpoint, HttpMethod.Post, requestBody["token"], postData);

                // Parse response into a list of ExpenseType
                var expenseTypeList = JsonConvert.DeserializeObject<List<ExpenseType>>(response);

                if (expenseTypeId != null)
                {
                    foreach (var expenseType in expenseTypeList)
                    {
                        expenseType.IsDefault = expenseType.ExpenseTypeId == expenseTypeId;
                    }
                }

                return expenseTypeList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostExpenseTypeListMorning: {ex.Message}");
                return new List<ExpenseType>();
            }
        }

        private async Task<List<ExpenseType>> CreateExpenseCategorylistMorning(
            int userId,
            string supplierId = null,
            string businessVatId = null,
            
            bool? documentApprovedToUninet = false,
            int? expenseTypeId = 0
        )
        {
            List<ExpenseType> expenseTypeList = new List<ExpenseType>();
            string supplierIdFromApi = "";

            // Retrieve the business object and get the InternalCompanyId
            var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
            if (business != null)
            {
                int internalCompanyId = business.BusinessId;

                // Get a new token for Morning API
                string token = await GetNewToken(internalCompanyId, Convert.ToInt32(supplierId), userId, 6); // ExternalSystemId for Morning is 6

                if (documentApprovedToUninet == false || documentApprovedToUninet == null)
                {
                    // Find supplier ID based on VAT ID
                    var filter = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("UserID", userId),
                        Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId)
                    );

                    var clientSuppliersItem = await _MorningClientSuppliers.Find(filter).ToListAsync();
                    foreach (var item in clientSuppliersItem)
                    {
                        var suppliers = item["suppliers"].AsBsonDocument;
                        foreach (var supplierKey in suppliers.Names)
                        {
                            var supplierDetails = suppliers[supplierKey].AsBsonDocument;
                            if (supplierDetails["vat_id"].AsString == businessVatId)
                            {
                                supplierIdFromApi = supplierDetails["supplier_id"].AsString;
                                break;
                            }
                        }
                    }

                    // Fetch expense type information using Morning API
                    var expenseEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83); // Replace with correct endpoint ID for Morning
                    var expenseEndpoint = expenseEndpointObj.Endpoint;

                    // Prepare request for Morning API
                    var requestBody = new Dictionary<string, string>
                    {
                        { "supplier_id", supplierIdFromApi },
                        { "token", token }
                    };

                    // Fetch permanent properties set to true
                    requestBody["permanent_property"] = "true";
                    var expenseTypeListTrue = await PostExpenseTypeListMorning(expenseEndpoint, requestBody, expenseTypeId);

                    // Fetch permanent properties set to false
                    requestBody["permanent_property"] = "false";
                    var expenseTypeListFalse = await PostExpenseTypeListMorning(expenseEndpoint, requestBody, expenseTypeId);

                    // Sort both lists
                    expenseTypeListFalse = expenseTypeListFalse.OrderBy(et => et.ExpenseTypeDesc).ToList();
                    expenseTypeListTrue = expenseTypeListTrue.OrderBy(et => et.ExpenseTypeDesc).ToList();

                    // Merge lists: expenseTypeListFalse first, then expenseTypeListTrue
                    expenseTypeList = expenseTypeListFalse.Concat(expenseTypeListTrue).ToList();
                }

                if (documentApprovedToUninet == true)
                {
                    // Handle case where document is approved to Uninet
                    var expenseEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83); // Replace with correct endpoint ID for Morning
                    var expenseEndpoint = expenseEndpointObj.Endpoint;

                    var requestBody = new Dictionary<string, string>
                    {
                        { "supplier_id", supplierIdFromApi },
                        { "token", token }
                    };

                    expenseTypeList = await PostExpenseTypeListMorning(expenseEndpoint, requestBody, expenseTypeId);
                }
            }

            return expenseTypeList;
        }


        private async Task<List<ExpenseType>> CreateExpenseCategorylistIcount(int userId, string supplierId = null, string BusinessVatId = null, string cidvalue = null, string uservalue = null, string passvalue = null, bool? DocumentApprovedtoUninet = false, int? ExpenseTypeId = 0)
        {
            List<ExpenseType> ExpenseTypeList = new List<ExpenseType>();
            string SuplierId = "";
            // Retrieve the business object and get the InternalCompanyId
            var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
            if (business != null)
            {
                if (DocumentApprovedtoUninet == false || DocumentApprovedtoUninet == null)
                {
                    int internalCompanyId = business.BusinessId; // Assuming InternalCompanyId is a property of Businesses

                    // Create a filter to match the internalcompanid and UserID
                    var filter = Builders<BsonDocument>.Filter.Eq("UserID", userId) &
                                 Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId);


                    var ClientSuppliersItem = await _IcountClientSuppliers.Find(filter).ToListAsync();
                    foreach (var item in ClientSuppliersItem)
                    {
                        // Assuming suppliers are stored as a BsonDocument
                        BsonDocument suppliers = item["suppliers"].AsBsonDocument;

                        foreach (var supplierKey in suppliers.Names)
                        {
                            var supplierDetails = suppliers[supplierKey].AsBsonDocument;
                            if (supplierDetails["vat_id"].AsString == BusinessVatId)
                            {
                                // Return supplier_id if vat_id matches
                                SuplierId = supplierDetails["supplier_id"].AsString;
                                break;
                            }
                        }
                    }


                    var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
                            Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId),
                            Builders<BsonDocument>.Filter.Eq("UserID", userId)
                        );

                    var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();

                    var expenseInfo = await GetRecentExpenseTypeInfo(ExpenseexistingDocument, supplierId);



                    var ExpenseTypeListEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);
                    var ExpenseTypeListEndpointinfo = ExpenseTypeListEndpoint.Endpoint;

                    var requestBody = new Dictionary<string, string>
                        {
                            {"cid",cidvalue},
                            {"pass",passvalue },
                            {"user",uservalue }
                        };


                    // First call with permanent_property = true
                    requestBody["permanent_property"] = "true";
                    var expenseTypeListTrue = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, expenseInfo.ExpenseTypeId);

                    // Second call with permanent_property = false
                    requestBody["permanent_property"] = "false";
                    var expenseTypeListFalse = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, expenseInfo.ExpenseTypeId);

                    // Sort both lists
                    expenseTypeListFalse = expenseTypeListFalse.OrderBy(et => et.ExpenseTypeDesc).ToList();
                    expenseTypeListTrue = expenseTypeListTrue.OrderBy(et => et.ExpenseTypeDesc).ToList();

                    // Merge lists: expenseTypeListFalse first, then expenseTypeListTrue
                    ExpenseTypeList = expenseTypeListFalse.Concat(expenseTypeListTrue).ToList();


                }

                if (DocumentApprovedtoUninet == true)
                {
                    var ExpenseTypeListEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);
                    var ExpenseTypeListEndpointinfo = ExpenseTypeListEndpoint.Endpoint;

                    var requestBody = new Dictionary<string, string>
                        {
                            {"cid",cidvalue},
                            {"pass",passvalue },
                            {"user",uservalue }
                        };
                    ExpenseTypeList = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, ExpenseTypeId.ToString());
                }

            }




            return ExpenseTypeList;

        }
        public async Task<string> SendRequestCurrency(string url, HttpMethod method, string postData)
        {
            using (var client = new HttpClient())
            using (var request = new HttpRequestMessage(method, url))
            {
                request.Content = new StringContent(postData, Encoding.UTF8, "application/json");

                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
            }
        }
        private async Task<BsonDocument> FetchCompanyRowByVatId(ExternalSystemConfig config, string vatId)
        {
            var filter = Builders<BsonDocument>.Filter.Or(
                Builders<BsonDocument>.Filter.Eq(config.VatFieldPath, vatId),
                Builders<BsonDocument>.Filter.Eq(config.VatFieldPath, vatId.TrimStart('0'))
            );
            return await config.CompaniesInfoCollection.Find(filter).FirstOrDefaultAsync();
        }
        private T ExtractField<T>(BsonDocument document, string fieldPath)
        {
            try
            {
                var segments = fieldPath.Split('.');
                BsonValue value = document;

                foreach (var segment in segments)
                {
                    if (value == null || !value.AsBsonDocument.Contains(segment))
                        return default;

                    value = value[segment];
                }

                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default;
            }
        }

        private async Task<object> GetCredentials(int userId, int companyId, int subCompanyId, int externalSystemId)
        {
            var credentials = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                x => x.Companyid == companyId
                  && x.Userid == userId
                  && x.SubCompayId == subCompanyId
                  && x.ExternalSystemId == externalSystemId
            );

            switch (externalSystemId)
            {
                case 2: // iCount
                    string cid = null, user = null, pass = null;

                    foreach (var field in credentials)
                    {
                        if (field.FieldLabelName == "cid") cid = field.FieldLabelValue;
                        else if (field.FieldLabelName == "user") user = field.FieldLabelValue;
                        else if (field.FieldLabelName == "pass") pass = field.FieldLabelValue;
                    }

                    return new { Cid = cid, User = user, Pass = pass };

                case 6: // Morning
                    string apiToken = null, secretKey = null;

                    foreach (var field in credentials)
                    {
                        if (field.FieldLabelName == "ApiToken") apiToken = field.FieldLabelValue;
                        else if (field.FieldLabelName == "SecretKey") secretKey = field.FieldLabelValue;
                    }

                    return new { ApiToken = apiToken, SecretKey = secretKey };

                default:
                    throw new InvalidOperationException("Unsupported ExternalSystemId");
            }
        }

        private async Task<ExpensesDigitalDocumentProp> ProcessICountDocument(
            BusinessData data,
            DigitalDocumentDInputRequest request,
            (string cid, string user, string pass) clientCreds,
            ShowingDocsResults docsResults,
            int userId,
            int internalCompanyId,
            string currencyCode,
            decimal currencyRate)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(request.JsonDocumentid));
            var doc = await _IcountWebhookData.Find(filter).FirstOrDefaultAsync();

            if (doc != null)
            {
                return await MapDocumentToResponseIcount(
                    doc,
                    docsResults,
                    request,
                    clientCreds,
                    userId,
                    internalCompanyId,
                    currencyCode,
                    currencyRate
                );
            }

            return null;
        }

        private async Task<ExpensesDigitalDocumentProp> ProcessMorningDocument(
            BusinessData data,
            DigitalDocumentDInputRequest request,
            (string ApiToken, string SecretKey) morningClientCreds,
            ShowingDocsResults docsResults,
            int userId,
            int internalCompanyId,
            string currencyCode,
            decimal currencyRate,
            int Client_SubCompanyid)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(request.JsonDocumentid));
            var doc = await _MorningWebHookData.Find(filter).FirstOrDefaultAsync();

            if (doc != null)
            {
                return await MapDocumentToResponseMorning(
                    doc,
                    docsResults,
                    request,
                    morningClientCreds,
                    userId,
                    internalCompanyId,
                    currencyCode,
                    currencyRate,
                    Client_SubCompanyid
                );
            }

            return null;
        }
        private async Task<string> SendRequestWithHeaders(string url, HttpMethod method, Dictionary<string, string> headers, HttpContent content = null)
        {
            using var client = new HttpClient();
            var request = new HttpRequestMessage(method, url);

            // Add headers to the request
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            // Add the content if provided
            if (content != null)
            {
                request.Content = content;

                // Read the actual content body
                string requestBodyContent = await content.ReadAsStringAsync();
                Console.WriteLine($"Request Body: {requestBodyContent}");
            }

            // Send the request and get the response
            var response = await client.SendAsync(request);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStringAsync(); // Return the response body
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error: {response.StatusCode} - {response.ReasonPhrase}");
                Console.WriteLine($"Response Content: {errorContent}");
                return null; // Return null if the request fails
            }
        }





        private async Task<SupplierItemMorning> AddSupplierMorning(string token, string supplierVatId)
        {
            try
            {
                // Fetch the endpoint configuration for adding a new supplier
                var addSupplierEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 90); // Update with the correct ID
                var addSupplierEndpoint = addSupplierEndpointObj.Endpoint;

                // Set the HTTP method to POST
                HttpMethod method = HttpMethod.Post;

                // Prepare the request payload with supplier data
                var supplierData = new
                {
                    name = "New Supplier",           // Replace with actual supplier name
                    active = true,                   // Set to true for an active supplier
                    department = "Sales",            // Optional: Department
                    taxId = supplierVatId,           // VAT ID
                    accountingKey = "10202",         // Optional: Accounting key
                    paymentTerms = 0,                // Optional: Payment terms
                    bankName = "לאומי",             // Replace with actual bank name
                    bankBranch = "44",               // Replace with actual bank branch
                    bankAccount = "565656565656",    // Replace with actual bank account
                    address = "רחוב סוקולוב 15",    // Replace with actual address
                    city = "תל אביב-יפו",           // Replace with actual city
                    zip = "6291790",                 // Replace with actual zip code
                    country = "IL",                  // Country code
                    phone = "565656565656",          // Replace with actual phone
                    fax = "565656565656",            // Replace with actual fax
                    mobile = "565656565656",         // Replace with actual mobile
                    remarks = "Customer approved 2016 sales", // Optional remarks
                    contactPerson = "Ido",           // Replace with actual contact person
                    emails = new string[] { },       // Replace with actual email list
                    labels = new string[] { }        // Replace with actual label list
                };

                // Serialize the supplier data to JSON and create StringContent
                var requestBody = new StringContent(JsonConvert.SerializeObject(supplierData), Encoding.UTF8, "application/json");

                // Prepare headers for authorization
                var requestHeaders = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {token}" }
        };

                // Send the request to add the supplier
                string response = await SendRequestWithHeaders(addSupplierEndpoint, method, requestHeaders, requestBody);

                // Parse the response
                var jsonDocument = JsonDocument.Parse(response);
                var jsonData = jsonDocument.RootElement;

                // Map the response to SupplierItem
                var SupplierItemMorning = new SupplierItemMorning
                {
                    supplier_id = jsonData.GetProperty("id").GetString(),
                    vat_id = int.TryParse(jsonData.GetProperty("taxId").GetString(), out var vatId) ? vatId : 0,
                    supplier_name = jsonData.GetProperty("name").GetString(),
                    company_name = jsonData.TryGetProperty("bankName", out var bankNameElement) ? bankNameElement.GetString() : "Unknown"
                };

                return SupplierItemMorning;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddSupplierMorning: {ex.Message}");
                return null; // Return null if there's an error
            }
        }



        private async Task<List<SupplierItemMorning>> GetSupplierListMorning(string token)
        {
            try
            {
                // Fetch the endpoint configuration for Morning Suppliers
                var suppliersEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 89);
                var suppliersEndpoint = suppliersEndpointObj.Endpoint;

                // Set the HTTP method to POST for fetching the list of suppliers
                HttpMethod method = HttpMethod.Post;

                // Prepare headers for authorization
                var requestHeaders = new Dictionary<string, string>
        {
            { "Authorization", $"Bearer {token}" }
        };

                // Prepare the request body with page and pageSize
                var requestBodyContent = new
                {
                    page = 1,
                    pageSize = 20
                };

                var requestBody = new StringContent(
                    JsonConvert.SerializeObject(requestBodyContent),
                    Encoding.UTF8,
                    "application/json"
                );

                // Send the request to fetch the supplier list
                string response = await SendRequestWithHeaders(suppliersEndpoint, method, requestHeaders, requestBody);

                // Parse the JSON response
                var jsonDocument = JsonDocument.Parse(response);
                var items = jsonDocument.RootElement.GetProperty("items");

                // Create a list to store SupplierItem objects
                var supplierList = new List<SupplierItemMorning>();

                // Loop through the suppliers data and map it to SupplierItem objects
                foreach (var supplier in items.EnumerateArray())
                {
                    var supplierItem = new SupplierItemMorning
                    {
                        supplier_id = supplier.GetProperty("id").GetString(),
                        vat_id = int.TryParse(supplier.GetProperty("taxId").GetString(), out var vatId) ? vatId : 0,
                        supplier_name = supplier.GetProperty("name").GetString(),
                        company_name = supplier.TryGetProperty("bankName", out var bankNameElement) ? bankNameElement.GetString() : null
                    };

                    supplierList.Add(supplierItem);
                }

                return supplierList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetSupplierListMorning: {ex.Message}");
                return new List<SupplierItemMorning>(); // Return empty list if there's an error
            }
        }






        private async Task<ExpensesDigitalDocumentProp> MapDocumentToResponseMorning(
    BsonDocument doc,
    ShowingDocsResults docsResults,
    DigitalDocumentDInputRequest request,
    (string ApiToken, string SecretKey) morningClientCreds,
    int userId,
    int internalCompanyId,
    string currencyCode,
    decimal currencyRate,
    int Client_SubCompanyid)
        {
            try
            {
                // Get a new token for Morning
                string newToken = await GetNewToken(internalCompanyId, Client_SubCompanyid, userId, 6);

                // Fetch supplier details using Morning API
                string supplierVatId = request.BusinessVatId;
                var supplierList = await GetSupplierListMorning(newToken);
                var supplierItem = supplierList.FirstOrDefault(s => s.vat_id == Convert.ToInt32(supplierVatId));

                if (supplierItem == null)
                {
                    // Add supplier if not found
                    var addedSupplier = await AddSupplierMorning(newToken, supplierVatId);
                    if (addedSupplier != null)
                    {
                        supplierItem = new SupplierItemMorning
                        {
                            supplier_id = addedSupplier.supplier_id,
                            vat_id = Convert.ToInt32(addedSupplier.vat_id),
                            supplier_name = addedSupplier.supplier_name,
                            company_name = addedSupplier.company_name
                        };
                    }
                }

                // Parse fields from the Morning document JSON
                int docNumber = doc.GetValue("number", 0).ToInt32();
                string doctypename = doc.GetValue("typename", 0).AsString;
                
                
                string docDate = doc.GetValue("date", "").AsString;
                double amountAV = doc.GetValue("total", 0.0).ToDouble();
                double amountBeforeVat = doc.GetValue("subtotal", 0.0).ToDouble();
                double vat = 0.0;

                // Extract VAT value from the tax array
                if (doc.TryGetValue("tax", out var taxElement) && taxElement.IsBsonArray)
                {
                    var taxArray = taxElement.AsBsonArray;
                    var vatTax = taxArray.FirstOrDefault(t => t.AsBsonDocument.GetValue("name", "").AsString == "VAT");
                    if (vatTax != null)
                    {
                        vat = vatTax.AsBsonDocument.GetValue("total", 0.0).ToDouble();
                    }
                }

                return new ExpensesDigitalDocumentProp
                {
                    Supplier_name_Sender = supplierItem?.supplier_name ?? "Unknown",
                    Supplier_ID = supplierItem?.supplier_id ?? "0",
                    DocNumber = docNumber.ToString(),
                    Doctype = doctypename.ToString(),
                    DocDate = DateTime.Parse(docDate),
                    AmountAV = amountAV,
                    Vat = vat,
                    AmountBeforeVat = amountBeforeVat,
                    Jsondocumentid = request.JsonDocumentid,
                    currencyName = currencyCode,
                    CurrenctRateValue = currencyRate,
                    showingDocsResults = docsResults,
                    internalCompanyId = internalCompanyId,
                    TaxId = request.BusinessVatId
                };
            }
            catch (Exception ex)
            {
                await LogException("MapDocumentToResponseMorning", ex);
                return null;
            }
        }




        private async Task LogException(string taskName, Exception ex)
        {
            var UserParam = new
            {
                Taskid = 1,
                TaskDesc = taskName,
                text = ex.InnerException + ex.Message
            };
            await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam);
        }
        private async Task<(string supplierId, string supplierName)> FetchOrAddSupplier(string supplierVatId, (string cid, string user, string pass) clientCreds)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", supplierVatId);
            var companyRow = await _IcountCompaniesInfoCollection.Find(filter).FirstOrDefaultAsync();

            string businessName = companyRow?["company_info"]["businessName"].AsString ?? "New_Supplier";
            string newSupplierName = $"{businessName}_{DateTime.Now:dd-MM-yyyy}";

            var endpoint = (await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 76)).Endpoint;
            var requestBody = new Dictionary<string, string>
    {
        { "cid", clientCreds.cid },
        { "user", clientCreds.user },
        { "pass", clientCreds.pass },
        { "supplier_name", newSupplierName },
        { "vat_id", supplierVatId }
    };

            string supplierId = await PostAndGetSupplierId(endpoint, requestBody);
            return (supplierId, newSupplierName);
        }





        private async Task<ExpensesDigitalDocumentProp> MapDocumentToResponseIcount(
            BsonDocument doc,
            ShowingDocsResults docsResults,
            DigitalDocumentDInputRequest request,
            (string cid, string user, string pass) clientCreds,
            int userId,
            int internalCompanyId,
            string currencyCode,
            decimal currencyRate)
        {
            try
            {
                // Fetch supplier details
                string supplierVatId = request.BusinessVatId;
                var supplierList = await GetClientSupplierList(clientCreds.cid, clientCreds.user, clientCreds.pass);

                var supplierItem = GetSupplierItemByVatId(supplierList, Convert.ToInt32(supplierVatId));
                if (supplierItem == null)
                {
                    var supplierData = await FetchOrAddSupplier(supplierVatId, clientCreds);
                    supplierItem = new SupplierItem
                    {
                        supplier_id = supplierData.supplierId,
                        supplier_name = supplierData.supplierName,
                        vat_id = Convert.ToInt32(supplierVatId)
                    };
                }

                // Map document data to response
                return new ExpensesDigitalDocumentProp
                {
                    Supplier_name_Sender = supplierItem.supplier_name.Split('_')[0],
                    Supplier_ID = supplierItem.supplier_id,
                    DocNumber = doc["doc_info"]["docnum"].AsString,
                    Doctype = doc["doc_info"]["doctype"].AsString,
                    DocDate = DateTime.Parse(doc["doc_info"]["dateissued"].AsString),
                    AmountAV = doc["doc_info"]["total"].ToDouble(),
                    Vat = doc["doc_info"]["totalvat"].ToDouble(),
                    AmountBeforeVat = doc["doc_info"]["totalsum"].ToDouble(),
                    Jsondocumentid = request.JsonDocumentid,
                    currencyName = currencyCode,
                    CurrenctRateValue = currencyRate,
                    showingDocsResults = docsResults,
                    internalCompanyId = internalCompanyId,
                    TaxId = request.BusinessVatId
                };
            }
            catch (Exception ex)
            {
                await LogException("MapDocumentToResponseIcount", ex);
                return null;
            }
        }






        public async Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId)
        {
            try
            {
                int InternalCompanyId = 0;
                int SubCompanyId = 0;
                int SubCompanyid_clientRelated = 0;

                string vatId = expensesUserDoRequest.ClientVat_id.PadLeft(9, '0');
                string supplierVatId = expensesUserDoRequest.BusinessVatId;

                // Determine external system and get config
                int externalSystemId = await GetExternalSystemIdbyjsonId(userId, expensesUserDoRequest.JsonDocumentid);
                var config = _externalSystemConfig[externalSystemId];

                // Retrieve Client and Supplier Information
                var companyClientRow = await FetchCompanyRowByVatId(config, vatId);
                var companySupplierRow = await FetchCompanyRowByVatId(config, supplierVatId);

                if (companyClientRow != null)
                {
                    InternalCompanyId = ExtractField<int>(companyClientRow, config.CompanyFieldPath);
                    SubCompanyid_clientRelated = ExtractField<int>(companyClientRow, config.SubCompanyFieldPath);
                }

                if (companySupplierRow != null)
                {
                    SubCompanyId = ExtractField<int>(companySupplierRow, config.SubCompanyFieldPath);
                }

                // Fetch credentials
                var clientCredentials = await GetCredentials(userId, InternalCompanyId, SubCompanyid_clientRelated, externalSystemId);
                var supplierCredentials = await GetCredentials(userId, InternalCompanyId, SubCompanyId, externalSystemId);

                (string cid, string user, string pass) clientCreds = (null, null, null);
                (string apiToken, string secretKey) morningClientCreds = (null, null);

                if (externalSystemId == 2) // iCount
                {
                    var c = (dynamic)clientCredentials;
                    clientCreds = (c.Cid, c.User, c.Pass);
                }
                else if (externalSystemId == 6) // Morning
                {
                    var c = (dynamic)clientCredentials;
                    morningClientCreds = (c.ApiToken, c.SecretKey);
                }

                // Fetch Document Information
                ShowingDocsResults docsResults = new ShowingDocsResults();
                var RowBusinessData = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == expensesUserDoRequest.JsonDocumentid);

                if (RowBusinessData != null)
                {
                    if (externalSystemId == 2) // iCount
                    {
                        // Fetch Currency Information
                        string currencyName = "ILS";
                        decimal currencyRate = 1;

                        var currencyInfoObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 87);
                        var currencyRateObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 85);

                        var currencyInfo = new CurrencyInfo
                        {
                            sid = "",
                            cid = clientCreds.cid,
                            user = clientCreds.user,
                            pass = clientCreds.pass,
                            currency_id = new CurrencyId { BaseType = 0 },
                            currency_code = new CurrencyCode { BaseType = "ILS" },
                            Currency = ""
                        };

                        string postData = JsonConvert.SerializeObject(currencyInfo);
                        string result = await SendRequestCurrency(currencyInfoObj.Endpoint, HttpMethod.Post, postData);
                        var jsonResponse = JObject.Parse(result);

                        if (jsonResponse["status"]?.Value<bool>() != true)
                        {
                            // Handle error response
                            docsResults.Success = false;
                            docsResults.ErrSec = expensesUserDoRequest.Lang == 1
                                ? "your external system credentials are wrong, please try again"
                                : "פרטי ההתחברות למערכת החיצונית שלך שגויים אנא נסה שנית";

                            return new ExpensesDigitalDocumentProp
                            {
                                showingDocsResults = docsResults
                            };
                        }
                        else
                        {
                            // Set success result
                            docsResults.Success = true;
                            docsResults.ErrSec = "";

                            var apiResponse = JsonConvert.DeserializeObject<ApiResponseinfo>(result);
                            currencyName = apiResponse.Currency;

                            // Get currency rate
                            var currencyRateRequest = new CurrencyRateRequest
                            {
                                sid = "",
                                cid = clientCreds.cid,
                                user = clientCreds.user,
                                pass = clientCreds.pass
                            };

                            string postData1 = JsonConvert.SerializeObject(currencyRateRequest);
                            string result1 = await SendRequestCurrency(currencyRateObj.Endpoint, HttpMethod.Post, postData1);

                            var apiResponse1 = JsonConvert.DeserializeObject<ApiResponse>(result1);
                            if (apiResponse1.CurrencyRates.TryGetValue(currencyName, out decimal rate))
                            {
                                currencyRate = rate;
                            }
                        }

                        var resultDocument = await ProcessICountDocument(
                            RowBusinessData,
                            expensesUserDoRequest,
                            clientCreds,
                            docsResults,
                            userId,
                            InternalCompanyId,
                            currencyName,
                            currencyRate
                        );

                        // Add ExpenseTypeList if missing
                        if (resultDocument != null && resultDocument.ExpenseTypeList == null)
                        {
                            var expenseList = await CreateExpenseCategorylistIcount(
                                userId,
                                supplierVatId,
                                supplierVatId,
                                clientCreds.cid,
                                clientCreds.user,
                                clientCreds.pass,
                                RowBusinessData.DocumentApprovedtoUninet,
                                RowBusinessData.ExpenseTypeId
                            );

                            resultDocument.ExpenseTypeList = expenseList;
                        }

                        // Set docsResults
                        resultDocument.showingDocsResults = docsResults;

                        return resultDocument;
                    }
                    else if (externalSystemId == 6) // Morning
                    {
                        string currencyName = "ILS";
                        decimal currencyRate = 1;

                        // Process Morning Document
                        var resultDocument = await ProcessMorningDocument(
                            RowBusinessData,
                            expensesUserDoRequest,
                            morningClientCreds,
                            docsResults,
                            userId,
                            InternalCompanyId,
                            currencyName,
                            currencyRate,
                            SubCompanyid_clientRelated
                        );

                        if (resultDocument != null && resultDocument.ExpenseTypeList == null)
                        {
                            // Fetch ExpenseTypeList for Morning
                            var expenseList = await CreateExpenseCategorylistMorning(
                            userId,                                 // int
                            supplierVatId,                          // string
                            supplierVatId,                          // string
                            RowBusinessData.DocumentApprovedtoUninet, // bool?
                            RowBusinessData.ExpenseTypeId                  // int?
                        );

           


                            resultDocument.ExpenseTypeList = expenseList;
                        }

                        // Set docsResults
                        resultDocument.showingDocsResults = docsResults;

                        return resultDocument;
                    }

                }

                // No documents found
                docsResults.Success = false;
                docsResults.ErrSec = expensesUserDoRequest.Lang == 1
                    ? "no documents left in the inbox"
                    : "לא נותרו מסמכים בתיבת הדואר הניכנס";

                return new ExpensesDigitalDocumentProp
                {
                    showingDocsResults = docsResults,
                    ExpenseTypeList = new List<ExpenseType>()
                };
            }
            catch (Exception ex)
            {
                await LogException("ShowDigitalDocumentDetails", ex);
                return null;
            }
        }







        //public async Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId)
        //{
        //    try
        //    {



        //        Int32 InternalCompanyId = 0;
        //        int SubCompanyId = 0;
        //        int SubCompanyid_clientRelated = 0;
        //        string Email = "";
        //        string addressCity = "";
        //        string addressState = "";
        //        string addressStreet = "";
        //        string addressZip = "";
        //        string vatId = expensesUserDoRequest.ClientVat_id;
        //        vatId = vatId.PadLeft(9, '0');
        //        var FilterClientvatidCompanyInfo = Builders<BsonDocument>.Filter.Or(
        //            Builders<BsonDocument>.Filter.Eq("company_info.vat_id", vatId),
        //            Builders<BsonDocument>.Filter.Eq("company_info.vat_id", vatId.TrimStart('0'))
        //        );

        //        var companyClientRow = await _IcountCompaniesInfoCollection.Find(FilterClientvatidCompanyInfo).FirstOrDefaultAsync();
        //        string suppliervatid = expensesUserDoRequest.BusinessVatId;
        //        var FilterSuppliervatidCompanyInfo = Builders<BsonDocument>.Filter.Or(
        //            Builders<BsonDocument>.Filter.Eq("company_info.vat_id", suppliervatid),
        //            Builders<BsonDocument>.Filter.Eq("company_info.vat_id", suppliervatid.TrimStart('0'))
        //        );
        //        var companysuplierRow = await _IcountCompaniesInfoCollection.Find(FilterSuppliervatidCompanyInfo).FirstOrDefaultAsync();

        //        if (companysuplierRow != null)
        //        {
        //            var companysuplierInfo = companysuplierRow["company_info"].AsBsonDocument;
        //            if (companysuplierInfo.Contains("SubCompanyId"))
        //            {
        //                var subCompanyId = companysuplierInfo["SubCompanyId"].AsInt32;
        //                SubCompanyId = subCompanyId;
        //                // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //            }
        //        }

        //        string BussinessName = "";
        //        if (companyClientRow != null)
        //        {
        //            var companyInfo = companyClientRow["company_info"].AsBsonDocument;
        //            if (companyInfo.Contains("InternalCompanyId"))
        //            {
        //                var internalCompanyId = companyInfo["InternalCompanyId"].AsInt32;
        //                InternalCompanyId = internalCompanyId;
        //                // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //            }
        //            else
        //            {
        //                // Handle the case where 'InternalCompanyId' is not present in the document.
        //            }
        //            if (companyInfo.Contains("SubCompanyId"))
        //            {
        //                var subCompanyId = companyInfo["SubCompanyId"].AsInt32;
        //                SubCompanyid_clientRelated = subCompanyId;
        //                // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //            }
        //            else
        //            {
        //                // Handle the case where 'InternalCompanyId' is not present in the document.
        //            }

        //        }
        //        else
        //        {
        //            // Handle the case where no document matches the filter.
        //        }

        //        //we get all list of supliers for the user loged into uninet and get his suplierid and supliername
        //        List<UsersExternalSystemDynamicFields> UserexternalSystemDynamicFieldslist = null;
        //        List<UsersExternalSystemDynamicFields> UserexternalSystemDynamicFieldslistClient = null;
        //        var CheckUsermasterExist = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == userId);
        //        if (CheckUsermasterExist != null)
        //        {
        //            var SubCompanyidClientObj = await _repository.GetFirstObjectAsync<BusinessData>(x => x.UserId == userId && x.BusinessId == InternalCompanyId && x.ClientVat_id == Convert.ToInt32(expensesUserDoRequest.ClientVat_id));
        //            UserexternalSystemDynamicFieldslistClient = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == InternalCompanyId && x.Userid == userId && x.SubCompayId == SubCompanyid_clientRelated);

        //            UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == InternalCompanyId && x.Userid == userId && x.SubCompayId == SubCompanyId);
        //        }
        //        else
        //        {
        //            var GetRelatedMasterId = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == userId);
        //            if (GetRelatedMasterId != null)
        //            {
        //                UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == GetRelatedMasterId.CompanyId && x.Userid == GetRelatedMasterId.Userid && x.SubCompayId == SubCompanyId);
        //                userId = GetRelatedMasterId.Userid;
        //            }

        //            if (UserexternalSystemDynamicFieldslistClient == null)
        //            {
        //                UserexternalSystemDynamicFieldslistClient = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == InternalCompanyId && x.Userid == userId && x.SubCompayId == SubCompanyid_clientRelated);
        //            }

        //            if (UserexternalSystemDynamicFieldslist == null)
        //            {
        //                var GetsuplierUserId = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == expensesUserDoRequest.JsonDocumentid);
        //                UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == expensesUserDoRequest.sendingDigitalDocumentBusinessID && x.Userid == GetsuplierUserId.UserId && x.SubCompayId == SubCompanyId);
        //            }

        //        }
        //        //client credentials
        //        string cidvalueclient = null;
        //        string uservalueclient = null;
        //        string passvalueclient = null;
        //        foreach (var dynamicField in UserexternalSystemDynamicFieldslistClient)
        //        {
        //            string fieldLabelName = dynamicField.FieldLabelName;
        //            string fieldLabelValue = dynamicField.FieldLabelValue;

        //            if (fieldLabelName == "cid")
        //            {
        //                cidvalueclient = fieldLabelValue;
        //                // Use the cid value as needed
        //            }
        //            else if (fieldLabelName == "user")
        //            {
        //                uservalueclient = fieldLabelValue;
        //                // Use the user value as needed
        //            }
        //            else if (fieldLabelName == "pass")
        //            {
        //                passvalueclient = fieldLabelValue;
        //                // Use the pass value as needed
        //            }






        //        }


        //        /////suplier credentials
        //        string cidvalue = null;
        //        string uservalue = null;
        //        string passvalue = null;
        //        foreach (var dynamicField in UserexternalSystemDynamicFieldslist)
        //        {
        //            string fieldLabelName = dynamicField.FieldLabelName;
        //            string fieldLabelValue = dynamicField.FieldLabelValue;

        //            if (fieldLabelName == "cid")
        //            {
        //                cidvalue = fieldLabelValue;
        //                // Use the cid value as needed
        //            }
        //            else if (fieldLabelName == "user")
        //            {
        //                uservalue = fieldLabelValue;
        //                // Use the user value as needed
        //            }
        //            else if (fieldLabelName == "pass")
        //            {
        //                passvalue = fieldLabelValue;
        //                // Use the pass value as needed
        //            }






        //        }
        //        var resSUpplierLIst = await GetClientSupplierList(cidvalueclient, uservalueclient, passvalueclient);
        //        //var resSUpplierLIst = await GetClientSupplierList(userId);
        //        ShowingDocsResults docsResults = new ShowingDocsResults();
        //        //BusinessData
        //        var RowBusinessData = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == expensesUserDoRequest.JsonDocumentid);
        //        //extract doctype,DocDate,total from IcountDocInfo
        //        string currencyName = "";
        //        string currency = "";
        //        decimal currenctRateValue = 0;
        //        string rate = "";
        //        string docnum = "";
        //        string Doctype = "";
        //        string dateissuedstr = "";
        //        string total_before_nicui = "";
        //        DateTime DocDate = DateTime.MinValue;
        //        string totalstr = "";
        //        double total = 0;
        //        double AmountBeforeVat = 0;
        //        double total_before_nicuiDouble = 0;
        //        string AmountBeforeVatstr = "";
        //        double DoubleVatresult = 0;
        //        string DoubleVatresultstr = "";
        //        string TaxId = "";



        //        if (RowBusinessData != null)
        //        {


        //            if (RowBusinessData.DataSourceType == 1)
        //            {
        //                var Documentidfilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(expensesUserDoRequest.JsonDocumentid));
        //                var doctypeprojection = Builders<BsonDocument>.Projection.Include("doctype").Exclude("_id");
        //                var doctyperesult = _ICountDocInfoCollection.Find(Documentidfilter).Project(doctypeprojection).FirstOrDefault();

        //                var DocDateprojection = Builders<BsonDocument>.Projection.Include("doc_info.dateissued").Exclude("_id");
        //                var DocDateresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(DocDateprojection).FirstOrDefault();


        //                var totalprojection = Builders<BsonDocument>.Projection.Include("doc_info.total").Exclude("_id");
        //                var totalresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(totalprojection).FirstOrDefault();

        //                var docnumprojection = Builders<BsonDocument>.Projection.Include("doc_info.docnum").Exclude("_id");
        //                var docnumresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(docnumprojection).FirstOrDefault();

        //                ////////////////////////////////////////////////////////////////////////////////////////////////////
        //                ///
        //                /*
        //                  "TaxId": ---זה למעשה ח"פ מספר ישות של החברה 
        //                  "AmountBeforeVat"-- סכום לפני מיסוי 
        //                  "Vat"--מיסוי עצמו
        //                */





        //                //vat_percent




        //                var TaxIdprojection = Builders<BsonDocument>.Projection.Include("doc_info.vat_id").Exclude("_id");
        //                var Taxresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(TaxIdprojection).FirstOrDefault();


        //                if (Taxresult != null)
        //                {

        //                    TaxId = Taxresult["doc_info"]["vat_id"].AsString;
        //                }





        //                var AmountBeforeVatprojection = Builders<BsonDocument>.Projection.Include("doc_info.totalsum").Exclude("_id");
        //                var AmountBeforeVatresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(AmountBeforeVatprojection).FirstOrDefault();

        //                if (AmountBeforeVatresult != null)
        //                {
        //                    BsonValue totalValue = AmountBeforeVatresult["doc_info"]["totalsum"];
        //                    if (totalValue.IsString)
        //                    {
        //                        string totalString = totalValue.AsString;
        //                        if (double.TryParse(totalString, out double totalDouble))
        //                        {
        //                            AmountBeforeVat = totalDouble;
        //                        }
        //                        else
        //                        {
        //                            // Handle the case when the string cannot be parsed as a double
        //                        }
        //                    }
        //                    else if (totalValue.IsDouble)
        //                    {
        //                        AmountBeforeVat = totalValue.AsDouble;
        //                    }
        //                    else
        //                    {
        //                        // Handle other data types if necessary
        //                    }
        //                }


        //                var Vatprojection = Builders<BsonDocument>.Projection.Include("doc_info.totalvat").Exclude("_id");
        //                var Vatresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(Vatprojection).FirstOrDefault();

        //                //totalvat eyal to deploy 

        //                if (Vatresult != null)
        //                {
        //                    BsonValue totalValue = Vatresult["doc_info"]["totalvat"];
        //                    if (totalValue.IsString)
        //                    {
        //                        string totalString = totalValue.AsString;
        //                        if (double.TryParse(totalString, out double totalDouble))
        //                        {
        //                            DoubleVatresult = totalDouble;
        //                        }
        //                        else
        //                        {
        //                            // Handle the case when the string cannot be parsed as a double
        //                        }
        //                    }
        //                    else if (totalValue.IsDouble)
        //                    {
        //                        DoubleVatresult = totalValue.AsDouble;
        //                    }
        //                    else
        //                    {
        //                        // Handle other data types if necessary
        //                    }
        //                }






        //                if (totalresult != null)
        //                {
        //                    BsonValue totalValue = totalresult["doc_info"]["total"];
        //                    if (totalValue.IsString)
        //                    {
        //                        string totalString = totalValue.AsString;
        //                        if (double.TryParse(totalString, out double totalDouble))
        //                        {
        //                            total = totalDouble;
        //                        }
        //                        else
        //                        {
        //                            // Handle the case when the string cannot be parsed as a double
        //                        }
        //                    }
        //                    else if (totalValue.IsDouble)
        //                    {
        //                        total = totalValue.AsDouble;
        //                    }
        //                    else
        //                    {
        //                        // Handle other data types if necessary
        //                    }
        //                }








        //                if (DocDateresult != null)
        //                {
        //                    BsonValue dateValue = DocDateresult["doc_info"]["dateissued"];
        //                    if (dateValue.IsString)
        //                    {
        //                        string dateString = dateValue.AsString;
        //                        if (DateTime.TryParse(dateString, out DateTime parsedDate))
        //                        {
        //                            DocDate = parsedDate;
        //                        }
        //                        else
        //                        {
        //                            // Handle the case when the string cannot be parsed as a DateTime
        //                        }
        //                    }
        //                    else if (dateValue.IsDateTime)
        //                    {
        //                        DocDate = dateValue.AsDateTime;
        //                    }
        //                    else
        //                    {
        //                        // Handle other data types if necessary
        //                    }
        //                }




        //                if (doctyperesult != null)
        //                {

        //                    Doctype = doctyperesult["doctype"].AsString;
        //                }



        //                if (docnumresult != null)
        //                {
        //                    docnum = docnumresult["doc_info"]["docnum"].AsString;

        //                }
        //            }
        //            if (RowBusinessData.DataSourceType == 2)
        //            {
        //                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(expensesUserDoRequest.JsonDocumentid));
        //                var webhookdoc = _IcountWebhookData.Find(filter).FirstOrDefault();

        //                if (webhookdoc != null)
        //                {
        //                    if (webhookdoc["doc_info"].AsBsonDocument.Contains("items") && webhookdoc["doc_info"]["items"].IsBsonArray)
        //                    {
        //                        var itemsArray = webhookdoc["doc_info"]["items"].AsBsonArray;
        //                        if (itemsArray.Count > 0)
        //                        {
        //                            // Extract the first item in the items array
        //                            var firstItem = itemsArray[0].AsBsonDocument;

        //                            // Extract currency and rate
        //                            if (firstItem.Contains("currency") && firstItem.Contains("rate"))
        //                            {
        //                                currency = firstItem["currency"].AsString;
        //                                rate = firstItem["rate"].ToString();


        //                            }
        //                        }
        //                    }

        //                    var currencyInfoObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 87);
        //                    //string EndpointcurrencyInfo = currencyInfoObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //                    HttpMethod method = HttpMethod.Post;

        //                    var currencyInfo = new CurrencyInfo
        //                    {
        //                        sid = "",
        //                        cid = cidvalueclient,
        //                        user = uservalueclient,
        //                        pass = passvalueclient,
        //                        currency_id = new CurrencyId
        //                        {
        //                            BaseType = 0
        //                        },
        //                        currency_code = new CurrencyCode
        //                        {
        //                            BaseType = currency
        //                        },
        //                        Currency = ""
        //                    };

        //                    string postData = JsonConvert.SerializeObject(currencyInfo);
        //                    string result = await SendRequestCurrency(currencyInfoObj.Endpoint, method, postData);

        //                    var jsonResult = JObject.Parse(result);
        //                    bool status = jsonResult["status"].Value<bool>();


        //                    if (!status)
        //                    {
        //                        docsResults.Success = false;
        //                        string errorDescription = jsonResult["error_description"].Value<string>();
        //                        docsResults.ErrSec = expensesUserDoRequest.Lang == 1 ? "your external system credentials are wrong  please try again" : "פרטי ההתחברות למערכת החיצונית שלך שגויים אנא נסה שנית ";

        //                        var expensesDigitalDocumentProp = new ExpensesDigitalDocumentProp
        //                        {

        //                            showingDocsResults = docsResults

        //                        };


        //                        return expensesDigitalDocumentProp;


        //                    }
        //                    else
        //                    {
        //                        docsResults.Success = true;
        //                        docsResults.ErrSec = "";
        //                    }
        //                    // Deserialize the response to extract the "currency" field
        //                    var apiResponse = JsonConvert.DeserializeObject<ApiResponseinfo>(result);
        //                    currencyName = apiResponse.Currency;

        //                    //////////////////////////////////////////////////////////

        //                    var currencyRateObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 85);
        //                    //string EndpointcurrencyInfo = currencyInfoObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //                    HttpMethod method1 = HttpMethod.Post;
        //                    var currencyRateRequest = new CurrencyRateRequest
        //                    {
        //                        sid = "",
        //                        cid = cidvalueclient,
        //                        user = uservalueclient,
        //                        pass = passvalueclient
        //                    };

        //                    string postData1 = JsonConvert.SerializeObject(currencyRateRequest);

        //                    string result1 = await SendRequestCurrency(currencyRateObj.Endpoint, method1, postData1);

        //                    // Deserialize the response
        //                    var apiResponse1 = JsonConvert.DeserializeObject<ApiResponse>(result1);

        //                    // Example: Get the rate for USD

        //                    if (apiResponse1.CurrencyRates.TryGetValue(currencyName, out decimal rate1))
        //                    {
        //                        currenctRateValue = rate1;
        //                    }


        //                    docnum = webhookdoc["doc_info"]["docnum"].AsString;
        //                    Doctype = webhookdoc["doc_info"]["doctype"].AsString;


        //                    dateissuedstr = webhookdoc["doc_info"]["dateissued"].AsString;
        //                    DateTime date = DateTime.ParseExact(dateissuedstr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

        //                    DocDate = date;

        //                    totalstr = webhookdoc["doc_info"]["total"].AsString;
        //                    total = Convert.ToDouble(totalstr);

        //                    if (webhookdoc["doc_info"].AsBsonDocument.Contains("totalsum"))
        //                    {

        //                        AmountBeforeVatstr = webhookdoc["doc_info"]["totalsum"].AsString;
        //                        AmountBeforeVat = Convert.ToDouble(AmountBeforeVatstr);
        //                    }

        //                    if (webhookdoc["doc_info"].AsBsonDocument.Contains("total_before_nicui"))
        //                    {


        //                        total_before_nicui = webhookdoc["doc_info"]["total_before_nicui"].AsString;
        //                        total_before_nicuiDouble = Convert.ToDouble(total_before_nicui);
        //                    }

        //                    if (webhookdoc["doc_info"].AsBsonDocument.Contains("totalvat"))
        //                    {
        //                        DoubleVatresultstr = webhookdoc["doc_info"]["totalvat"].AsString;
        //                        DoubleVatresult = Convert.ToDouble(DoubleVatresultstr);

        //                    }



        //                    if (webhookdoc["doc_info"].AsBsonDocument.Contains("totalvat"))
        //                    {
        //                        DoubleVatresultstr = webhookdoc["doc_info"]["totalvat"].AsString;
        //                        DoubleVatresult = Convert.ToDouble(DoubleVatresultstr);

        //                    }





        //                    //JsonDocumentid  //BusinessData





        //                }
        //            }








        //            //now we should loop on the resSUpplierLIst
        //            //and find if the vatId exist in the suplier list
        //            var ItemFound = GetSupplierItemByVatId(resSUpplierLIst, Convert.ToInt32(expensesUserDoRequest.BusinessVatId));

        //            if (ItemFound != null)
        //            {
        //                //string cidvalue = null;
        //                //string uservalue = null;
        //                //string passvalue = null;

        //                //JsonDocumentid

        //                List<ExpenseType> res = await CreateExpenseCategorylist(userId, ItemFound.supplier_id.ToString(), expensesUserDoRequest.BusinessVatId, cidvalueclient, uservalueclient, passvalueclient, RowBusinessData.DocumentApprovedtoUninet, RowBusinessData.ExpenseTypeId);
        //var expensesDigitalDocumentProp = new ExpensesDigitalDocumentProp
        //{
        //    Supplier_name_Sender = ItemFound.supplier_name.Split('_')[0],
        //    Supplier_ID = ItemFound.supplier_id,
        //    DocNumber = docnum,
        //    Doctype = Doctype,
        //    DocDate = DocDate,
        //    AmountAV = total,
        //    currencyName = currencyName,
        //    CurrenctRateValue = currenctRateValue,
        //    ExpenseTypeList = res,
        //    internalCompanyId = InternalCompanyId,
        //    Jsondocumentid = expensesUserDoRequest.JsonDocumentid,
        //    TaxId = expensesUserDoRequest.BusinessVatId,
        //    AmountBeforeVat = AmountBeforeVat == 0 ? total_before_nicuiDouble : AmountBeforeVat,
        //    Vat = DoubleVatresult,
        //    showingDocsResults = docsResults



        //};
        //                return expensesDigitalDocumentProp;
        //            }
        //            else//if not found call  // https://api.icount.co.il/api/v3.php/supplier/add
        //            {


        //                var FilterClientvatidSenderCompanyInfo = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", expensesUserDoRequest.BusinessVatId);




        //                var SendercompanyClientRow = await _IcountCompaniesInfoCollection.Find(FilterClientvatidSenderCompanyInfo).FirstOrDefaultAsync();


        //                var sendercompanyInfo = SendercompanyClientRow["company_info"].AsBsonDocument;


        //                if (sendercompanyInfo.Contains("businessName"))
        //                {
        //                    var businessName = sendercompanyInfo["businessName"].AsString;
        //                    BussinessName = businessName;
        //                    // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //                }

        //                if (sendercompanyInfo.Contains("email"))
        //                {
        //                    var businessEmail = sendercompanyInfo["email"].AsString;
        //                    businessEmail = businessEmail;
        //                    // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //                }


        //                if (sendercompanyInfo.Contains("addressCity"))
        //                {
        //                    var businessaddressCity = sendercompanyInfo["addressCity"].AsString;
        //                    addressCity = businessaddressCity;
        //                    // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //                }

        //                if (sendercompanyInfo.Contains("addressState"))
        //                {
        //                    var businessaddressState = sendercompanyInfo["addressState"].AsString;
        //                    addressState = businessaddressState;
        //                    // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //                }

        //                if (sendercompanyInfo.Contains("addressStreet"))
        //                {
        //                    var businessaddressStreet = sendercompanyInfo["addressStreet"].AsString;
        //                    addressStreet = businessaddressStreet;
        //                    // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //                }

        //                if (sendercompanyInfo.Contains("addressZip"))
        //                {
        //                    var businessaddressZip = sendercompanyInfo["addressZip"].AsString;
        //                    addressZip = businessaddressZip;
        //                    // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
        //                }









        //                var ClinetinfoEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 76);
        //                var endpointClinetinfo = ClinetinfoEndpoint.Endpoint;

        //                //var FilterClientvatidCompanyInfo = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", expensesUserDoRequest.ClientVat_id);
        //                //var companyclientRow = await _IcountCompaniesInfoCollection.Find(FilterClientvatidCompanyInfo).FirstOrDefaultAsync();


        //                string NewBusinessName = BussinessName + "_" + DateTime.Now.ToString("dd-MM-yyyy");
        //                var requestBody = new Dictionary<string, string>
        //                {
        //                    { "cid",cidvalueclient},
        //                    {"pass",passvalueclient},
        //                    {"user",uservalueclient},
        //                    { "supplier_name", NewBusinessName},//to get the provider name from the document its on the pdf document for example uninetconnect
        //                    { "vat_id", expensesUserDoRequest.BusinessVatId },
        //                    { "fname", "" },
        //                    { "lname", "" },
        //                    { "email", Email },//remark eyal to add an eamil her fro, the pdf doc
        //                    { "phone", "" },
        //                    { "mobile", "" },
        //                    { "fax", "" },
        //                    { "bus_country", addressState },
        //                    { "bus_city", addressCity },
        //                    { "bus_zip", addressZip },
        //                    { "bus_street", addressStreet },//remark eyal to get this and city here 
        //                    { "bus_no", "" },
        //                    { "bank", "" },
        //                    { "branch", "" },
        //                    { "account", "" },
        //                    { "faccount", "" },
        //                    { "wht_percent", "" },
        //                    { "wht_validity", "" },
        //                    { "notes", "" }
        //                };

        //                /*
        //                    string Email = "";
        //                    string addressCity = "";
        //                    string addressState = "";
        //                    string addressStreet = "";
        //                    string addressZip = "";

        //                */
        //                var supplierId = await PostAndGetSupplierId(endpointClinetinfo, requestBody);


        //                var supplierItem = new SupplierItem
        //                {
        //                    company_name = BussinessName,
        //                    supplier_id = supplierId,
        //                    supplier_name = BussinessName,
        //                    vat_id = Convert.ToInt32(expensesUserDoRequest.BusinessVatId)
        //                };
        //                resSUpplierLIst.Add(supplierItem);


        //                //await CreateExpenseCategorylist(userId, ItemFound.supplier_id.ToString(), expensesUserDoRequest.BusinessVatId, cidvalue, uservalue, passvalue, RowBusinessData.DocumentApprovedtoUninet, RowBusinessData.ExpenseTypeId);
        //                List<ExpenseType> res = await CreateExpenseCategorylist(userId, supplierId.ToString(), expensesUserDoRequest.BusinessVatId, cidvalueclient, uservalueclient, passvalueclient, RowBusinessData.DocumentApprovedtoUninet, RowBusinessData.ExpenseTypeId);
        //                var SuplierItemFound = GetSupplierItemByVatId(resSUpplierLIst, Convert.ToInt32(expensesUserDoRequest.BusinessVatId));

        //                var expensesDigitalDocumentProp = new ExpensesDigitalDocumentProp
        //                {
        //                    Supplier_name_Sender = SuplierItemFound.supplier_name.Split('_')[0],
        //                    Supplier_ID = SuplierItemFound.supplier_id,
        //                    DocNumber = docnum,
        //                    Doctype = Doctype,
        //                    DocDate = DocDate,
        //                    AmountAV = total,
        //                    currencyName = currencyName,
        //                    CurrenctRateValue = currenctRateValue,
        //                    ExpenseTypeList = res,
        //                    internalCompanyId = InternalCompanyId,
        //                    Jsondocumentid = expensesUserDoRequest.JsonDocumentid,
        //                    TaxId = expensesUserDoRequest.BusinessVatId,
        //                    AmountBeforeVat = AmountBeforeVat,
        //                    Vat = DoubleVatresult,
        //                    showingDocsResults = docsResults

        //                };

        //                return expensesDigitalDocumentProp;

        //            }

        //        }
        //        else
        //        {
        //            docsResults.Success = false;

        //            docsResults.ErrSec = expensesUserDoRequest.Lang == 1 ? "no documents left in the inbox" : "לא נותרו מסמכים בתיבת הדואר הניכנס";
        //            var ZeroDocsResponse = new ExpensesDigitalDocumentProp
        //            {
        //                Supplier_name_Sender = null,
        //                Supplier_ID = 0,
        //                DocNumber = null,
        //                Doctype = null,
        //                DocDate = default(DateTime),
        //                AmountAV = 0.0,
        //                currencyName = null,
        //                CurrenctRateValue = 0,
        //                ExpenseTypeList = null,
        //                internalCompanyId = 0,
        //                Jsondocumentid = null,
        //                TaxId = null,
        //                AmountBeforeVat = 0.0,
        //                Vat = 0.0,
        //                showingDocsResults = docsResults
        //            };
        //            return ZeroDocsResponse;
        //        }


        //    }
        //    catch (Exception ex)
        //    {
        //        var UserParam0 = new
        //        {
        //            Taskid = 1,
        //            TaskDesc = "ShowDigitalDocumentDetails",
        //            text = ex.InnerException + ex.Message
        //        };
        //        var spresult0 = await _repository.ExecuteGetSPAsync<InsertdatatoJobbatchlogResult>(ConstUninetStoredprocedure.SP_InsertdatatoJobbatchlog, UserParam0);



        //        return null;
        //    }
        //}




        public async Task<responseTest> test(int UserID, string Typelist)
        {
            string text = "";
            try
            {

                string jsonResult = "";
                text = "UserID=" + UserID + "***";
                List<DigitalDocumentToApprove> List_DigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
                var ResListOfCompaniesRelatedToLogedinUser = await _repository.GetListOfObjectsAsync<Businesses>(x => x.AdminUserid == UserID);
                text = text + "result from " + System.Text.Json.JsonSerializer.Serialize(ResListOfCompaniesRelatedToLogedinUser);
                //loop on the list of Companies for each company attached to user we need to extract her vat_id from IcountCompanisInfo 
                foreach (var Company in ResListOfCompaniesRelatedToLogedinUser)
                {

                    text = text + "1 ";
                    var filter = Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", Company.BusinessId);
                    var projection = Builders<BsonDocument>.Projection.Include("company_info.vat_id").Exclude("_id");
                    text = text + "2 before filter looking for Company.BusinessId" + Company.BusinessId + "in IcountCompanisInfo colection";
                    var result = _IcountCompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();
                    text = text + "3 ";
                    jsonResult = System.Text.Json.JsonSerializer.Serialize(result.ToString());
                    text = text + "4 ";
                    return new responseTest { text = "eyal111   " + text };
                    //return new responseTest { text = jsonResult + "Company.BusinessId=" + Company.BusinessId };


                    //if (result != null)
                    //{
                    //    var vatId = result["company_info"]["vat_id"].AsString;




                    //    //now we go to BusinessData  table that has all digitaldocument sent to clients and check if client is there by his vat_id
                    //    //if its found we need to extract JsonDocumentid  and BusinessId (as the company that sent the document)
                    //    //and return a list of them to the client to show this waitingto approve list to insert as expenses
                    //    // Replace with your desired VAT ID

                    //    List<BusinessData> ResListOfClientCompaniesThatWasSentDigitalDocument = null;
                    //    switch (Typelist)
                    //    {

                    //        //remark eyal need to get 
                    //        //supplier_name_Sender
                    //        //docDate
                    //        //amountAV

                    //        case "notApproveOrRejected":
                    //            ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == null);
                    //            // ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) );
                    //            break;
                    //        case "Rejected":
                    //            ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == false);
                    //            break;
                    //        case "Approved":
                    //            ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == true);
                    //            break;
                    //    }

                    //    //  string jsonResult = System.Text.Json.JsonSerializer.Serialize(ResListOfClientCompaniesThatWasSentDigitalDocument);


                    //    foreach (var DigitalClientRow in ResListOfClientCompaniesThatWasSentDigitalDocument)
                    //    {
                    //        string JsonDocUrl = "";
                    //        var DocInfofilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(DigitalClientRow.JsonDocumentid));
                    //        var DocInfoprojection = Builders<BsonDocument>.Projection.Include("doc_info.doc_url").Exclude("_id");
                    //        var DocInforesult = _ICountDocInfoCollection.Find(DocInfofilter).Project(DocInfoprojection).FirstOrDefault();
                    //        if (DocInforesult != null)
                    //        {
                    //            JsonDocUrl = DocInforesult["doc_info"]["doc_url"].AsString;
                    //        }
                    //        List_DigitalDocumentToApprove.Add(new DigitalDocumentToApprove() { JsonDocumentid = DigitalClientRow.JsonDocumentid, ClientVat_id = Convert.ToInt32(DigitalClientRow.ClientVat_id), SendingDigitalDocumentBusinessID = DigitalClientRow.BusinessId, BusinessVatId = DigitalClientRow.BusinessVatId, DocInfoUrl = JsonDocUrl, supplier_name_Sender = DigitalClientRow.supplier_name_Sender, docDate = DigitalClientRow.docDate, amountAV = DigitalClientRow.amountAV });

                    //    }
                    //}

                }

                return new responseTest { text = "eyal222" };
            }
            catch (Exception ex)
            {
                return new responseTest { text = ex.Message + ex.InnerException + text };
            }

            //try 
            //{
            //    List<DigitalDocumentToApprove> List_DigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
            //    var ResListOfCompaniesRelatedToLogedinUser = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == UserID);
            //    string jsonResult = System.Text.Json.JsonSerializer.Serialize(ResListOfCompaniesRelatedToLogedinUser);
            //    return new responseTest { text= jsonResult };
            //}
            //catch(Exception ex)
            //{
            //    return null;
            //}
        }

        public async Task<string> GetCompanyName(int subCompanyId, int MainCompanyId)
        {
            var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", MainCompanyId),
                    Builders<BsonDocument>.Filter.Eq("company_info.SubCompanyId", subCompanyId)
                );

            var projection = Builders<BsonDocument>.Projection.Include("company_info.businessName").Exclude("_id");

            var result = _IcountCompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();
            if (result != null)
            {
                var businessName = result["company_info"]["businessName"].AsString;

                return businessName;
            }
            else return "";
        }


        // Method to populate ListOfSubCompaniesandNames
        public async Task<List<CompanyNameRelatedToUser>> PopulateListOfSubCompaniesandNames(
      List<MainSubCopmaniesMasters> ResListOfCompaniesRelatedToLogedinUser,
      int UserID,
      int ExternalSystemId)
        {
            List<CompanyNameRelatedToUser> ListOfSubCompaniesandNames = new List<CompanyNameRelatedToUser>();

            // Retrieve the external system configuration dynamically
            var config = _externalSystemConfig[ExternalSystemId];

            // Find the most recent LastTimeDataShowed
            DateTime mostRecentTime = ResListOfCompaniesRelatedToLogedinUser.Max(c => c.LastTimeDataShowed);

            foreach (var company in ResListOfCompaniesRelatedToLogedinUser)
            {
                int totalDocs = 0;
                int DocsAccepted = 0;
                int totalDocsRejected = 0;

                string vatIdValue = "";

                // Query to dynamically access the correct MongoDB collection
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq(config.CompanyFieldPath, company.MainCompanyId),
                    Builders<BsonDocument>.Filter.Eq(config.SubCompanyFieldPath, company.SubCopmanyId)
                );

                var companyRow = await config.CompaniesInfoCollection.Find(filter).FirstOrDefaultAsync();
                if (companyRow != null)
                {
                    // Retrieve the VAT ID dynamically based on the system configuration
                    var vatIdPathSegments = config.VatFieldPath.Split('.');
                    BsonValue fieldValue = companyRow;

                    foreach (var segment in vatIdPathSegments)
                    {
                        if (fieldValue.AsBsonDocument.Contains(segment))
                        {
                            fieldValue = fieldValue[segment];
                        }
                        else
                        {
                            fieldValue = null;
                            break;
                        }
                    }

                    if (fieldValue != null)
                    {
                        vatIdValue = fieldValue.AsString;
                    }
                }

                if (vatIdValue != "")
                {
                    var totalCountObj = await _repository.GetListOfObjectsAsync<BusinessData>(
                        x => x.ClientVat_id == Convert.ToUInt32(vatIdValue) && x.DocumentApprovedtoUninet == null);
                    totalDocs = totalCountObj.Count();

                    var DocsAcceptedObj = await _repository.GetListOfObjectsAsync<BusinessData>(
                        x => x.ClientVat_id == Convert.ToUInt32(vatIdValue) && x.DocumentApprovedtoUninet == true);
                    DocsAccepted = DocsAcceptedObj.Count();

                    var DocsRejectedObj = await _repository.GetListOfObjectsAsync<BusinessData>(
                        x => x.ClientVat_id == Convert.ToUInt32(vatIdValue) && x.DocumentApprovedtoUninet == false);
                    totalDocsRejected = DocsRejectedObj.Count();
                }
                string companyName = await GetCompanyName(company.SubCopmanyId, company.MainCompanyId);

                // Check First-Time Console Logic
                var FirstTimeConsoleIndicationObj = await _repository.GetFirstObjectAsync<FirstTimeConsoleIndication>(
                    x => x.Userid == UserID && x.Mainorganization == company.MainCompanyId && x.Subcompanyid == company.SubCopmanyId);

                if (FirstTimeConsoleIndicationObj == null)
                {
                    var objrowFirstTimeConsoleIndication = new FirstTimeConsoleIndication
                    {
                        Userid = UserID,
                        Mainorganization = company.MainCompanyId,
                        Subcompanyid = company.SubCopmanyId,
                        FirsttimeOnConsoleForEntity = true
                    };
                    await _repository.CreateAsync(objrowFirstTimeConsoleIndication);
                }

                var UserCreditCardHolderObj = await _repository.GetFirstObjectAsync<UserCreditCardHolder>(
                    x => x.UserId == UserID && x.BusinessId == company.MainCompanyId && x.SubCompanyId == company.SubCopmanyId && x.ExternalSystemId == ExternalSystemId);

                if (companyName != null)
                {
                    bool isDefault = company.LastTimeDataShowed == mostRecentTime;

                    ListOfSubCompaniesandNames.Add(new CompanyNameRelatedToUser
                    {
                        SubCopmanyId = company.SubCopmanyId,
                        CompanyName = $"{companyName}_{vatIdValue}",
                        IsDefault = isDefault,
                        TotalDocs = totalDocs,
                        TotalDocsAccepted = DocsAccepted,
                        TotalDocsRejected = totalDocsRejected,
                        MainCompanyId = company.MainCompanyId,
                        ShowFirstTimeMessage = (FirstTimeConsoleIndicationObj?.UserClicksonContinueFree == null),
                        ShowExceedsMessage = (DocsAccepted >= 15 && UserCreditCardHolderObj == null),
                        ClickedButtonToInviteBusinessPartnersSubCompany = company.ClickedButtonToInviteBusinessPartnersSubCompany
                    });
                }
            }

            return ListOfSubCompaniesandNames;
        }




        public List<MainSubCopmaniesMasters> GetMainSubCompanies(int userId, int companyId,int SubCompanyid)
        {
            List<MainSubCopmaniesMasters> ResListOfCompaniesRelatedToLogedinUser = new List<MainSubCopmaniesMasters>();

            // Get SubUserCredentials based on userId
            var subUserCredentials = _repository.GetListOfObjects<SubUserCredentials>(x => x.SubUserId == userId && x.SubCompanyId== SubCompanyid);

            // Get MainSubCopmaniesMasters
            var mainSubCompaniesMasters = _repository.GetListOfObjects<MainSubCopmaniesMasters>(x => x.MainCompanyId == companyId);

            // Loop through SubUserCredentials
            if (subUserCredentials.Count > 0)
            {
                foreach (var sub in subUserCredentials)
                {
                    // Find matching record in MainSubCopmaniesMasters
                    var matchingRecord = mainSubCompaniesMasters.FirstOrDefault(main =>
                        main.MainCompanyId == sub.CompanyId && main.SubCopmanyId == sub.SubCompanyId);

                    // If a matching record is found, add it to the result list
                    if (matchingRecord != null)
                    {
                        ResListOfCompaniesRelatedToLogedinUser.Add(matchingRecord);
                    }
                }
            }
            else
            {
                // Add all records from mainSubCompaniesMasters to the result list
                ResListOfCompaniesRelatedToLogedinUser.AddRange(mainSubCompaniesMasters);
            }

            return ResListOfCompaniesRelatedToLogedinUser;
        }

        private async Task<List<BusinessData>> FetchPaginatedDigitalDocuments(int UserID, string Typelist, int? subCompanyId, string vatId, int currentPage, int itemsPerPage, bool? documentApprovedToUninet)
        {
            try
            {
                // Calculate startIndex based on currentPage and itemsPerPage
                int startIndex = (currentPage - 1) * itemsPerPage;

                // Fetch the paginated list of digital documents using the updated repository method
                var digitalDocuments = _repository.GetListOfObjectsPaging<BusinessData>(
                    x => x.ClientVat_id == Convert.ToUInt32(vatId) &&
                         (x.DocumentApprovedtoUninet == documentApprovedToUninet),
                    startIndex,
                    itemsPerPage
                );

                return digitalDocuments;
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                return new List<BusinessData>();
            }
        }



        public async Task<int> GetDocAmountSupplier(string vatId, int UserID, int subCopmanyId, int MainCompanyId, string cidvalue, string uservalue, string passvalue, string Supplier_id)
        {
            try
            {
                var DocAmountSupplierEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 77);  ////https://api.icount.co.il/api/v3.php/expense/search
                var endpointDocAmountSupplier = DocAmountSupplierEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue + "&start_ts=01/01/2000" + "&end_ts=" + DateTime.Now + "&supplier_id=" + Supplier_id;
                HttpMethod methodDocAmountSupplier = HttpMethod.Get;
                var ReponsneDocAmountSupplier = await SendRequest(endpointDocAmountSupplier, methodDocAmountSupplier);
                /*
                https://api.icount.co.il/api/v3.php/expense/search?cid=clienticount&user=larexsons2010&pass=45F$t123&start_ts=01/01/2000 08:07:38&end_ts=19/04/2024 17:31:55&supplier_id=1
                */
                // Parse the JSON response
                var jsonDocument = JsonDocument.Parse(ReponsneDocAmountSupplier);
                var jsonData = jsonDocument.RootElement;
                var results_count = jsonData.GetProperty("results_count");
                int count = results_count.GetInt32();

                return count;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return 0;
            }
        }
        public async Task<int> GetDocAmountclient(string vatId, int UserID, int subCopmanyId, int MainCompanyId, string cidvalue, string uservalue, string passvalue)
        {
            try
            {
                var DocAmountclientEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 71);  ////https://api.icount.co.il/api/v3.php/doc/search
                var endpointDocAmountclient = DocAmountclientEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue + "&vat_id=" + vatId + "&max_results=1";
                HttpMethod methodDocAmountclient = HttpMethod.Get;
                var ReponsneDocAmountclient = await SendRequest(endpointDocAmountclient, methodDocAmountclient);

                // Parse the JSON response
                var jsonDocument = JsonDocument.Parse(ReponsneDocAmountclient);
                var jsonData = jsonDocument.RootElement;
                var results_count = jsonData.GetProperty("results_count");
                int count = results_count.GetInt32();
                // Create a list to store SupplierItem objects


                return count;
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return 0;
            }
        }
        private async Task<bool> GetStatus(string vatId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", vatId);
            var existingDocument = await _IcountCompaniesInfoCollection.Find(filter).FirstOrDefaultAsync();
            if (existingDocument != null)
            {
                return true;
            }
            else
            {
                // Implement logic to determine other status values based on your requirements
                // For example:
                // if (someCondition) return "Missing email details";
                // else if (anotherCondition) return "Waiting for invitation";
                // else return "Status not determined";
                return false;
            }
        }

        //private async Task<TotalBusinessPartnerProp> GetSuppliers(
        // string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId,
        // int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId, int pageNumber, int pageSize,
        // bool ShortVersion, int Lang)
        //{
        //    var suppliersList = new List<BusinessPartnerProp>();
        //    string UIStatus = "";
        //    string actionLabel = "";
        //    string typeLabel = Lang == 1 ? "Supplier" : "ספק"; // Set the default Type based on the language

        //    // Check if the document exists in the collection
        //    var filterBuilder = Builders<BsonDocument>.Filter;
        //    var filter = filterBuilder.Eq("userId", userId) &
        //                 filterBuilder.Eq("subCompanyId", subCompanyId) &
        //                 filterBuilder.Eq("MainCompanyId", MainCompanyId) &
        //                 filterBuilder.Eq("ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId) &
        //                 filterBuilder.Exists("suppliers", true);
        //    var existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();
        //    DateTime lastInsertDate = new DateTime();

        //    if (existingDocument != null)
        //    {
        //        var dateString = existingDocument["LastInsertDate"].AsString;

        //        if (DateTime.TryParse(dateString, out lastInsertDate))
        //        {
        //            lastInsertDate = lastInsertDate.ToUniversalTime();
        //        }

        //        TimeSpan differenceyear = DateTime.UtcNow - lastInsertDate;

        //        if (differenceyear.TotalDays < 365)
        //        {
        //            var suppliersData = existingDocument.GetValue("suppliers");
        //            foreach (var supplier in suppliersData.AsBsonDocument)
        //            {
        //                var supplierData = supplier.Value.AsBsonDocument;
        //                string businesspartnerName = supplierData.GetValue("supplier_name").AsString;
        //                string vatId = supplierData.GetValue("vat_id").AsString;
        //                string Supplier_id = supplierData.GetValue("supplier_id").AsString;
        //                string SupplierEmail = supplierData.GetValue("email").AsString;
        //                int docAmount = await GetDocAmountSupplier(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue, Supplier_id);
        //                bool status = await GetStatus(vatId);
        //                DateTime? lastInvitationDate = null;
        //                ActionItem action = null;

        //                var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(
        //                    x =>  x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId
        //                );

        //                if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue)
        //                {
        //                    if (status)
        //                    {
        //                        UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                        actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                        action = new ActionItem(actionLabel, ActionType.String);
        //                    }
        //                    else
        //                    {
        //                        UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
        //                        if (BusinessPartnersObj.EmailSent.Value)
        //                        {
        //                            var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                            if (difference.TotalDays < 365)
        //                            {
        //                                action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                            }
        //                            else
        //                            {
        //                                actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                                action = new ActionItem(actionLabel, ActionType.Button);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
        //                            actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
        //                            action = new ActionItem(actionLabel, ActionType.Button);
        //                        }
        //                    }
        //                }
        //                else if (status)
        //                {
        //                    UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                    actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                    action = new ActionItem(actionLabel, ActionType.String);
        //                }
        //                else
        //                {
        //                    UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
        //                    actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                    action = new ActionItem(actionLabel, ActionType.Button);
        //                }

        //                suppliersList.Add(new BusinessPartnerProp
        //                {
        //                    BusinesspartnerName = businesspartnerName,
        //                    VatId = vatId,
        //                    supplier_id = Supplier_id,
        //                    DocAmount = docAmount,
        //                    Email = SupplierEmail,
        //                    SubCompanyId = subCompanyId,
        //                    Type = typeLabel,
        //                    Status = UIStatus,
        //                    LastInvitationDate = BusinessPartnersObj?.LastDateSent,
        //                    Actions = action
        //                });
        //            }
        //        }
        //        else
        //        {
        //            await _Icount_BussinesPartner_Clients_Suplliers.DeleteOneAsync(filter);
        //            var supplierget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);
        //            var endpointsupplierget_list = supplierget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //            HttpMethod methodsupplierget_list = HttpMethod.Get;
        //            var Reponsnesupplierget_list = await SendRequest(endpointsupplierget_list, methodsupplierget_list);

        //            var modifiedJson = JsonConvert.DeserializeObject<JObject>(Reponsnesupplierget_list);
        //            modifiedJson["userId"] = userId;
        //            modifiedJson["subCompanyId"] = subCompanyId;
        //            modifiedJson["MainCompanyId"] = MainCompanyId;
        //            modifiedJson["ExtrnalsystemIdOfsubCopmanyId"] = ExtrnalsystemIdOfsubCopmanyId;
        //            modifiedJson["LastInsertDate"] = DateTime.UtcNow;

        //            await _Icount_BussinesPartner_Clients_Suplliers.InsertOneAsync(BsonDocument.Parse(modifiedJson.ToString()));
        //            var jsonDocument = JsonDocument.Parse(Reponsnesupplierget_list);
        //            var jsonData = jsonDocument.RootElement;
        //            var suppliersData = jsonData.GetProperty("suppliers");

        //            foreach (var supplier in suppliersData.EnumerateObject())
        //            {
        //                var supplierData = supplier.Value;
        //                string businesspartnerName = supplierData.GetProperty("supplier_name").GetString();
        //                string vatId = supplierData.GetProperty("vat_id").GetString();
        //                string Supplier_id = supplierData.GetProperty("supplier_id").GetString();
        //                string SupplierEmail = supplierData.GetProperty("email").GetString();
        //                int docAmount = await GetDocAmountSupplier(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue, Supplier_id);
        //                bool status = await GetStatus(vatId);
        //                DateTime? lastInvitationDate = null;
        //                ActionItem action = null;

        //                var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(
        //                    x => x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId
        //                );

        //                if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue)
        //                {
        //                    if (status)
        //                    {
        //                        UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                        actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                        action = new ActionItem(actionLabel, ActionType.String);
        //                    }
        //                    else
        //                    {
        //                        UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
        //                        if (BusinessPartnersObj.EmailSent.Value)
        //                        {
        //                            var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                            if (difference.TotalDays < 365)
        //                            {
        //                                action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                            }
        //                            else
        //                            {
        //                                actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                                action = new ActionItem(actionLabel, ActionType.Button);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
        //                            actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
        //                            action = new ActionItem(actionLabel, ActionType.Button);
        //                        }
        //                    }
        //                }
        //                else if (status)
        //                {
        //                    UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                    actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                    action = new ActionItem(actionLabel, ActionType.String);
        //                }
        //                else
        //                {
        //                    UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
        //                    actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                    action = new ActionItem(actionLabel, ActionType.Button);
        //                }

        //                suppliersList.Add(new BusinessPartnerProp
        //                {
        //                    BusinesspartnerName = businesspartnerName,
        //                    VatId = vatId,
        //                    supplier_id = Supplier_id,
        //                    DocAmount = docAmount,
        //                    Email = SupplierEmail,
        //                    SubCompanyId = subCompanyId,
        //                    Type = typeLabel,
        //                    Status = UIStatus,
        //                    LastInvitationDate = lastInvitationDate,
        //                    Actions = action
        //                });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        // Document does not exist, fetch data from external source
        //        var supplierget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);
        //        var endpointsupplierget_list = supplierget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //        HttpMethod methodsupplierget_list = HttpMethod.Get;
        //        var Reponsnesupplierget_list = await SendRequest(endpointsupplierget_list, methodsupplierget_list);

        //        var modifiedJson = JsonConvert.DeserializeObject<JObject>(Reponsnesupplierget_list);
        //        modifiedJson["userId"] = userId;
        //        modifiedJson["subCompanyId"] = subCompanyId;
        //        modifiedJson["MainCompanyId"] = MainCompanyId;
        //        modifiedJson["ExtrnalsystemIdOfsubCopmanyId"] = ExtrnalsystemIdOfsubCopmanyId;
        //        modifiedJson["LastInsertDate"] = DateTime.UtcNow;

        //        await _Icount_BussinesPartner_Clients_Suplliers.InsertOneAsync(BsonDocument.Parse(modifiedJson.ToString()));
        //        var jsonDocument = JsonDocument.Parse(Reponsnesupplierget_list);
        //        var jsonData = jsonDocument.RootElement;
        //        var suppliersData = jsonData.GetProperty("suppliers");

        //        foreach (var supplier in suppliersData.EnumerateObject())
        //        {
        //            var supplierData = supplier.Value;
        //            string businesspartnerName = supplierData.GetProperty("supplier_name").GetString();
        //            string vatId = supplierData.GetProperty("vat_id").GetString();
        //            string Supplier_id = supplierData.GetProperty("supplier_id").GetString();
        //            string SupplierEmail = supplierData.GetProperty("email").GetString();
        //            int docAmount = await GetDocAmountSupplier(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue, Supplier_id);
        //            bool status = await GetStatus(vatId);
        //            DateTime? lastInvitationDate = null;
        //            ActionItem action = null;

        //            var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(
        //                x =>  x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId
        //            );

        //            if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue)
        //            {
        //                if (status)
        //                {
        //                    UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                    actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                    action = new ActionItem(actionLabel, ActionType.String);
        //                }
        //                else
        //                {
        //                    UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
        //                    if (BusinessPartnersObj.EmailSent.Value)
        //                    {
        //                        var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                        if (difference.TotalDays < 365)
        //                        {
        //                            action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                        }
        //                        else
        //                        {
        //                            actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                            action = new ActionItem(actionLabel, ActionType.Button);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
        //                        actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
        //                        action = new ActionItem(actionLabel, ActionType.Button);
        //                    }
        //                }
        //            }
        //            else if (status)
        //            {
        //                UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                action = new ActionItem(actionLabel, ActionType.String);
        //            }
        //            else
        //            {
        //                UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
        //                actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                action = new ActionItem(actionLabel, ActionType.Button);
        //            }

        //            suppliersList.Add(new BusinessPartnerProp
        //            {
        //                BusinesspartnerName = businesspartnerName,
        //                VatId = vatId,
        //                supplier_id = Supplier_id,
        //                DocAmount = docAmount,
        //                Email = SupplierEmail,
        //                SubCompanyId = subCompanyId,
        //                Type = typeLabel,
        //                Status = UIStatus,
        //                LastInvitationDate = lastInvitationDate,
        //                Actions = action
        //            });
        //        }
        //    }

        //    if (!ShortVersion)
        //    {
        //        // Sort the list by DocAmount in descending order
        //        suppliersList = suppliersList.OrderByDescending(s => s.DocAmount).ToList();
        //        int totalResults = suppliersList.Count;
        //        // Apply pagination
        //        suppliersList = suppliersList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //        var TotalBusinessPartnerPropObj = new TotalBusinessPartnerProp
        //        {
        //            BusinessPartnerProp = suppliersList,
        //            TotalResults = totalResults
        //        };
        //        return TotalBusinessPartnerPropObj;
        //    }
        //    else
        //    {
        //        // Order the list by DocAmount in descending order
        //        suppliersList = suppliersList.OrderByDescending(s => s.DocAmount).ToList();

        //        // Get the total number of results before applying the top 10 filter
        //        int totalResults = suppliersList.Count;

        //        // Get only the top 10 suppliers
        //        suppliersList = suppliersList.Take(10).ToList();

        //        var TotalBusinessPartnerPropObj = new TotalBusinessPartnerProp
        //        {
        //            BusinessPartnerProp = suppliersList,
        //            TotalResults = totalResults
        //        };
        //        return TotalBusinessPartnerPropObj;
        //    }
        //}



        //private async Task<List<BusinessPartnerProp>> GetSuppliers(string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId, int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId, int pageNumber, int pageSize)
        //{


        //    var suppliersList = new List<BusinessPartnerProp>();

        //    // Check if the document exists in the collection
        //    var filterBuilder = Builders<BsonDocument>.Filter;
        //    var filter = filterBuilder.Eq("userId", userId) &
        //                 filterBuilder.Eq("subCompanyId", subCompanyId) &
        //                 filterBuilder.Eq("MainCompanyId", MainCompanyId) &
        //                 filterBuilder.Eq("ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId) &
        //                 filterBuilder.Exists("suppliers", true);
        //    var existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();
        //    DateTime lastInsertDate = new DateTime();
        //    // If the document exists, extract "suppliers" data from it
        //    if (existingDocument != null)
        //    {
        //        var dateString = existingDocument["LastInsertDate"].AsString;

        //        if (DateTime.TryParse(dateString, out lastInsertDate))
        //        {
        //            // Now you can convert the DateTime to universal time
        //            lastInsertDate = lastInsertDate.ToUniversalTime();
        //        }
        //        else
        //        {
        //            // Handle the case where the date string cannot be parsed
        //            // This could be due to invalid date format or missing/incorrect data
        //        }

        //        // Calculate the difference between lastInsertDate and current date
        //        TimeSpan differenceyear = DateTime.UtcNow - lastInsertDate;

        //        // Check if the difference is more than one year
        //        if (differenceyear.TotalDays < 365)
        //        {

        //            var suppliersData = existingDocument.GetValue("suppliers");
        //            foreach (var supplier in suppliersData.AsBsonDocument)
        //            {
        //                var supplierData = supplier.Value.AsBsonDocument;
        //                string businesspartnerName = supplierData.GetValue("supplier_name").AsString;
        //                string vatId = supplierData.GetValue("vat_id").AsString;
        //                string Supplier_id = supplierData.GetValue("supplier_id").AsString;
        //                string SupplierEmail = supplierData.GetValue("email").AsString;
        //                int docAmount = await GetDocAmountSupplier(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue, Supplier_id);
        //                string status = await GetStatus(vatId);
        //                DateTime? lastInvitationDate = null;
        //                ActionItem action = null;

        //                var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(x => x.VatId == Convert.ToInt32(vatId) && x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId);

        //                if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue)
        //                {
        //                    if (status == "Connected to Uninet")
        //                    {
        //                        action = new ActionItem("Connected", ActionType.String);
        //                    }
        //                    else if (status == "Still not connected")
        //                    {
        //                        if (BusinessPartnersObj.EmailSent.Value)
        //                        {
        //                            var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                            if (difference.TotalDays < 365)
        //                            {
        //                                action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                            }
        //                            else
        //                            {
        //                                action = new ActionItem("Invite", ActionType.Button);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            status = "Missing email details";
        //                            action = new ActionItem("Complete details", ActionType.Button);
        //                        }
        //                    }
        //                }
        //                else if (status == "Connected to Uninet")
        //                {
        //                    action = new ActionItem("Connected", ActionType.String);
        //                }
        //                else
        //                {
        //                    status = "Waiting for invitation";
        //                    action = new ActionItem("Invite", ActionType.Button);
        //                }

        //                suppliersList.Add(new BusinessPartnerProp
        //                {
        //                    BusinesspartnerName = businesspartnerName,
        //                    VatId = vatId,
        //                    supplier_id = Supplier_id,
        //                    DocAmount = docAmount,
        //                    Email = SupplierEmail,
        //                    SubCompanyId = subCompanyId,
        //                    Type = "Supplier",
        //                    Status = status,
        //                    LastInvitationDate = BusinessPartnersObj?.LastDateSent,
        //                    Actions = action
        //                });
        //            }
        //        }
        //        else
        //        {
        //            await _Icount_BussinesPartner_Clients_Suplliers.DeleteOneAsync(filter);
        //            var supplierget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);
        //            var endpointsupplierget_list = supplierget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //            HttpMethod methodsupplierget_list = HttpMethod.Get;
        //            var Reponsnesupplierget_list = await SendRequest(endpointsupplierget_list, methodsupplierget_list);

        //            // Modify the JSON response by adding the required fields
        //            var modifiedJson = JsonConvert.DeserializeObject<JObject>(Reponsnesupplierget_list);
        //            modifiedJson["userId"] = userId;
        //            modifiedJson["subCompanyId"] = subCompanyId;
        //            modifiedJson["MainCompanyId"] = MainCompanyId;
        //            modifiedJson["ExtrnalsystemIdOfsubCopmanyId"] = ExtrnalsystemIdOfsubCopmanyId;
        //            modifiedJson["LastInsertDate"] = DateTime.UtcNow;

        //            // Insert the modified JSON into the collection
        //            await _Icount_BussinesPartner_Clients_Suplliers.InsertOneAsync(BsonDocument.Parse(modifiedJson.ToString()));
        //            var jsonDocument = JsonDocument.Parse(Reponsnesupplierget_list);
        //            var jsonData = jsonDocument.RootElement;
        //            var suppliersData = jsonData.GetProperty("suppliers");

        //            foreach (var supplier in suppliersData.EnumerateObject())
        //            {
        //                var supplierData = supplier.Value;
        //                string businesspartnerName = supplierData.GetProperty("supplier_name").GetString();
        //                string vatId = supplierData.GetProperty("vat_id").GetString();
        //                string Supplier_id = supplierData.GetProperty("supplier_id").GetString();
        //                string SupplierEmail = supplierData.GetProperty("email").GetString();
        //                int docAmount = await GetDocAmountSupplier(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue, Supplier_id);
        //                string status = await GetStatus(vatId);
        //                DateTime? lastInvitationDate = null;
        //                ActionItem action = null;


        //                var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(x => x.VatId == Convert.ToInt32(vatId) && x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId);

        //                if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue)
        //                {
        //                    if (status == "Connected to Uninet")
        //                    {
        //                        action = new ActionItem("Connected", ActionType.String);
        //                    }
        //                    else if (status == "Still not connected")
        //                    {
        //                        if (BusinessPartnersObj.EmailSent.Value)
        //                        {
        //                            var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                            if (difference.TotalDays < 365)
        //                            {
        //                                action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                            }
        //                            else
        //                            {
        //                                action = new ActionItem("Invite", ActionType.Button);
        //                            }
        //                        }
        //                        else
        //                        {
        //                            status = "Missing email details";
        //                            action = new ActionItem("Complete details", ActionType.Button);
        //                        }
        //                    }
        //                }
        //                else if (status == "Connected to Uninet")
        //                {
        //                    action = new ActionItem("Connected", ActionType.String);
        //                }
        //                else
        //                {
        //                    status = "Waiting for invitation";
        //                    action = new ActionItem("Invite", ActionType.Button);
        //                }

        //                suppliersList.Add(new BusinessPartnerProp
        //                {
        //                    BusinesspartnerName = businesspartnerName,
        //                    VatId = vatId,
        //                    supplier_id = Supplier_id,
        //                    DocAmount = docAmount,
        //                    Email = SupplierEmail,
        //                    SubCompanyId = subCompanyId,
        //                    Type = "Supplier",
        //                    Status = status,
        //                    LastInvitationDate = lastInvitationDate,
        //                    Actions = action
        //                });
        //            }
        //        }

        //    }
        //    else
        //    {
        //        // Document does not exist, fetch data from external source

        //        var supplierget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);
        //        var endpointsupplierget_list = supplierget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //        HttpMethod methodsupplierget_list = HttpMethod.Get;
        //        var Reponsnesupplierget_list = await SendRequest(endpointsupplierget_list, methodsupplierget_list);

        //        // Modify the JSON response by adding the required fields
        //        var modifiedJson = JsonConvert.DeserializeObject<JObject>(Reponsnesupplierget_list);
        //        modifiedJson["userId"] = userId;
        //        modifiedJson["subCompanyId"] = subCompanyId;
        //        modifiedJson["MainCompanyId"] = MainCompanyId;
        //        modifiedJson["ExtrnalsystemIdOfsubCopmanyId"] = ExtrnalsystemIdOfsubCopmanyId;
        //        modifiedJson["LastInsertDate"] = DateTime.UtcNow;

        //        // Insert the modified JSON into the collection
        //        await _Icount_BussinesPartner_Clients_Suplliers.InsertOneAsync(BsonDocument.Parse(modifiedJson.ToString()));


        //        var jsonDocument = JsonDocument.Parse(Reponsnesupplierget_list);
        //        var jsonData = jsonDocument.RootElement;
        //        var suppliersData = jsonData.GetProperty("suppliers");

        //        foreach (var supplier in suppliersData.EnumerateObject())
        //        {
        //            var supplierData = supplier.Value;
        //            string businesspartnerName = supplierData.GetProperty("supplier_name").GetString();
        //            string vatId = supplierData.GetProperty("vat_id").GetString();
        //            string Supplier_id = supplierData.GetProperty("supplier_id").GetString();
        //            string SupplierEmail = supplierData.GetProperty("email").GetString();
        //            int docAmount = await GetDocAmountSupplier(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue, Supplier_id);
        //            string status = await GetStatus(vatId);
        //            DateTime? lastInvitationDate = null;
        //            ActionItem action = null;


        //            var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(x => x.VatId == Convert.ToInt32(vatId) && x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId);

        //            if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue)
        //            {
        //                if (status == "Connected to Uninet")
        //                {
        //                    action = new ActionItem("Connected", ActionType.String);
        //                }
        //                else if (status == "Still not connected")
        //                {
        //                    if (BusinessPartnersObj.EmailSent.Value)
        //                    {
        //                        var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                        if (difference.TotalDays < 365)
        //                        {
        //                            action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                        }
        //                        else
        //                        {
        //                            action = new ActionItem("Invite", ActionType.Button);
        //                        }
        //                    }
        //                    else
        //                    {
        //                        status = "Missing email details";
        //                        action = new ActionItem("Complete details", ActionType.Button);
        //                    }
        //                }
        //            }
        //            else if (status == "Connected to Uninet")
        //            {
        //                action = new ActionItem("Connected", ActionType.String);
        //            }
        //            else
        //            {
        //                status = "Waiting for invitation";
        //                action = new ActionItem("Invite", ActionType.Button);
        //            }

        //            suppliersList.Add(new BusinessPartnerProp
        //            {
        //                BusinesspartnerName = businesspartnerName,
        //                VatId = vatId,
        //                supplier_id = Supplier_id,
        //                DocAmount = docAmount,
        //                Email = SupplierEmail,
        //                SubCompanyId = subCompanyId,
        //                Type = "Supplier",
        //                Status = status,
        //                LastInvitationDate = lastInvitationDate,
        //                Actions = action
        //            });
        //        }
        //    }





        //    return suppliersList;
        //}



        private async Task<TotalBusinessPartnerProp> GetSuppliers(
            string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId,
            int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId, int pageNumber, int pageSize,
            bool ShortVersion, int Lang)
        {
            try
            {
                var suppliersList = new List<BusinessPartnerProp>();
                string typeLabel = Lang == 1 ? "Supplier" : "ספק"; // Set the default Type based on the language

                // Define the filter to find the document in the collection
                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("userId", userId) &
                             filterBuilder.Eq("subCompanyId", subCompanyId) &
                             filterBuilder.Eq("MainCompanyId", MainCompanyId) &
                             filterBuilder.Eq("ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId) &
                             filterBuilder.Exists("suppliers", true);

                // Attempt to retrieve the existing document
                var existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();

                if (existingDocument != null)
                {
                    DateTime lastInsertDate;
                    // Parse the LastInsertDate safely
                    if (DateTime.TryParse(existingDocument["LastInsertDate"].AsString, out lastInsertDate))
                    {
                        lastInsertDate = DateTime.SpecifyKind(lastInsertDate, DateTimeKind.Utc); // Ensure UTC
                        TimeSpan differenceYear = DateTime.UtcNow - lastInsertDate;

                        // Refresh data if the last insert is outdated
                        if (differenceYear.TotalDays > 0)
                        {
                            await RefreshSupplierData(cidvalue, uservalue, passvalue, userId, subCompanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);

                            // Re-fetch the updated document after refresh
                            existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();
                        }
                    }
                    else
                    {
                        // Handle the case where LastInsertDate cannot be parsed
                        await RefreshSupplierData(cidvalue, uservalue, passvalue, userId, subCompanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);
                        existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();
                    }

                    if (existingDocument != null)
                    {
                        // Process the document
                        var suppliersData = existingDocument["suppliers"].AsBsonDocument;
                        var vatIds = suppliersData.Names.Select(name => suppliersData[name]["vat_id"].AsString).ToList();
                        var supplierEmails = suppliersData.Names.Select(name => suppliersData[name]["email"].AsString).ToList();

                        // Query the status for all VAT IDs at once
                        var statuses = await GetSuplierStatuses(vatIds);

                        // Batch query for all relevant BusinessPartnersEmails records
                        var businessPartnersObjs = await _repository.GetListOfObjectsAsync<BusinessPartnersEmails>(
                            x => x.UserId == userId &&
                                 x.SubCompanyId == subCompanyId &&
                                 x.OrganizationId == MainCompanyId &&
                                 supplierEmails.Contains(x.Email)
                        );

                        var businessPartnersDict = businessPartnersObjs.ToDictionary(x => x.Email);

                        foreach (var supplierName in suppliersData.Names)
                        {
                            var supplierData = suppliersData[supplierName].AsBsonDocument;
                            string businesspartnerName = supplierData["supplier_name"].AsString;
                            string vatId = supplierData["vat_id"].AsString;
                            string supplier_id = supplierData["supplier_id"].AsString;
                            string supplierEmail = supplierData["email"].AsString;

                            var status = statuses.Contains(vatId);
                            businessPartnersDict.TryGetValue(supplierEmail, out var businessPartnersObj);

                            string UIStatus;
                            string actionLabel;
                            ActionItem action = null;

                            if (status)
                            {
                                UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
                                actionLabel = Lang == 1 ? "Connected" : "מחובר";
                                action = new ActionItem(actionLabel, ActionType.String);
                            }
                            else
                            {
                                UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
                                if (businessPartnersObj != null && businessPartnersObj.EmailSent.HasValue && businessPartnersObj.EmailSent.Value)
                                {
                                    var difference = DateTime.Now - businessPartnersObj.LastDateSent.Value;
                                    if (difference.TotalDays < 365)
                                    {
                                        action = new ActionItem(businessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
                                    }
                                    else
                                    {
                                        actionLabel = Lang == 1 ? "Invite" : "הזמן";
                                        action = new ActionItem(actionLabel, ActionType.Button);
                                    }
                                }
                                else if (businessPartnersObj != null && businessPartnersObj.Email == string.Empty)
                                {
                                    UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
                                    actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
                                    action = new ActionItem(actionLabel, ActionType.Button);
                                }
                                else if ((businessPartnersObj != null && businessPartnersObj.EmailSent.HasValue && businessPartnersObj.EmailSent.Value == false && businessPartnersObj.Email != string.Empty) || (businessPartnersObj == null))
                                {
                                    UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
                                    actionLabel = Lang == 1 ? "Invite" : "הזמן";
                                    action = new ActionItem(actionLabel, ActionType.Button);
                                }
                            }

                            suppliersList.Add(new BusinessPartnerProp
                            {
                                BusinesspartnerName = businesspartnerName,
                                VatId = vatId,
                                supplier_id = supplier_id,
                                DocAmount = 0, // Initially set to 0, you can populate it later
                                Email = supplierEmail,
                                SubCompanyId = subCompanyId,
                                Type = typeLabel,
                                Status = UIStatus,
                                LastInvitationDate = businessPartnersObj?.LastDateSent,
                                Actions = action
                            });
                        }
                    }
                }
                else
                {
                    // No existing document found, refresh data
                    await RefreshSupplierData(cidvalue, uservalue, passvalue, userId, subCompanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);

                    // Re-fetch and process after refresh
                    existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();

                    if (existingDocument != null)
                    {
                        var suppliersData = existingDocument["suppliers"].AsBsonDocument;
                        var vatIds = suppliersData.Names.Select(name => suppliersData[name]["vat_id"].AsString).ToList();
                        var supplierEmails = suppliersData.Names.Select(name => suppliersData[name]["email"].AsString).ToList();

                        // Query the status for all VAT IDs at once
                        var statuses = await GetSuplierStatuses(vatIds);

                        // Batch query for all relevant BusinessPartnersEmails records
                        var businessPartnersObjs = await _repository.GetListOfObjectsAsync<BusinessPartnersEmails>(
                            x => x.UserId == userId &&
                                 x.SubCompanyId == subCompanyId &&
                                 x.OrganizationId == MainCompanyId &&
                                 supplierEmails.Contains(x.Email)
                        );

                        var businessPartnersDict = businessPartnersObjs.ToDictionary(x => x.Email);

                        foreach (var supplierName in suppliersData.Names)
                        {
                            var supplierData = suppliersData[supplierName].AsBsonDocument;
                            string businesspartnerName = supplierData["supplier_name"].AsString;
                            string vatId = supplierData["vat_id"].AsString;
                            string supplier_id = supplierData["supplier_id"].AsString;
                            string supplierEmail = supplierData["email"].AsString;

                            var status = statuses.Contains(vatId);
                            businessPartnersDict.TryGetValue(supplierEmail, out var businessPartnersObj);

                            string UIStatus;
                            string actionLabel;
                            ActionItem action = null;

                            if (status)
                            {
                                UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
                                actionLabel = Lang == 1 ? "Connected" : "מחובר";
                                action = new ActionItem(actionLabel, ActionType.String);
                            }
                            else
                            {
                                UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
                                if (businessPartnersObj != null && businessPartnersObj.EmailSent.HasValue && businessPartnersObj.EmailSent.Value)
                                {
                                    var difference = DateTime.Now - businessPartnersObj.LastDateSent.Value;
                                    if (difference.TotalDays < 365)
                                    {
                                        action = new ActionItem(businessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
                                    }
                                    else
                                    {
                                        actionLabel = Lang == 1 ? "Invite" : "הזמן";
                                        action = new ActionItem(actionLabel, ActionType.Button);
                                    }
                                }
                                else if (businessPartnersObj != null && businessPartnersObj.Email == string.Empty)
                                {
                                    UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
                                    actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
                                    action = new ActionItem(actionLabel, ActionType.Button);
                                }
                                else if ((businessPartnersObj != null && businessPartnersObj.EmailSent.HasValue && businessPartnersObj.EmailSent.Value == false && businessPartnersObj.Email != string.Empty) || (businessPartnersObj == null))
                                {
                                    UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
                                    actionLabel = Lang == 1 ? "Invite" : "הזמן";
                                    action = new ActionItem(actionLabel, ActionType.Button);
                                }
                            }

                            suppliersList.Add(new BusinessPartnerProp
                            {
                                BusinesspartnerName = businesspartnerName,
                                VatId = vatId,
                                supplier_id = supplier_id,
                                DocAmount = 0, // Initially set to 0, you can populate it later
                                Email = supplierEmail,
                                SubCompanyId = subCompanyId,
                                Type = typeLabel,
                                Status = UIStatus,
                                LastInvitationDate = businessPartnersObj?.LastDateSent,
                                Actions = action
                            });
                        }
                    }
                }

                // Finalize the response
                if (!ShortVersion)
                {
                    // Sort the list by DocAmount in descending order
                    suppliersList = suppliersList.OrderByDescending(s => s.DocAmount).ToList();

                    int totalResults = suppliersList.Count;

                    // Apply pagination
                    suppliersList = suppliersList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                    return new TotalBusinessPartnerProp
                    {
                        BusinessPartnerProp = suppliersList,
                        TotalResults = totalResults
                    };
                }
                else
                {
                    // Get only the top 10 suppliers
                    suppliersList = suppliersList.OrderByDescending(s => s.DocAmount).Take(10).ToList();

                    return new TotalBusinessPartnerProp
                    {
                        BusinessPartnerProp = suppliersList,
                        TotalResults = suppliersList.Count
                    };
                }
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                
                return null;
            }
        }



        private async Task<HashSet<string>> GetSuplierStatuses(List<string> vatIds)
        {
            // Assuming MongoDB or similar NoSQL
            var filter = Builders<BsonDocument>.Filter.In("company_info.vat_id", vatIds);
            var documents = await _IcountCompaniesInfoCollection.Find(filter).ToListAsync();
            var statusVatIds = new HashSet<string>(documents.Select(doc => doc["company_info"]["vat_id"].AsString));
            return statusVatIds;
        }

        private async System.Threading.Tasks.Task RefreshSupplierData(string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId, int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId)
        {
            // Get the endpoint
            var supplierget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);
            var endpointsupplierget_list = supplierget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
            HttpMethod methodsupplierget_list = HttpMethod.Get;
            var Reponsnesupplierget_list = await SendRequest(endpointsupplierget_list, methodsupplierget_list);

            // Parse the JSON response using JsonDocument
            using var jsonDocument = JsonDocument.Parse(Reponsnesupplierget_list);
            var jsonData = jsonDocument.RootElement;

            // Get Israel time zone
            TimeZoneInfo israelTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Israel Standard Time");
            DateTime israelTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, israelTimeZone);

            // Manually construct a new BsonDocument with additional fields
            var newBsonDocument = new BsonDocument
    {
        { "userId", userId },
        { "subCompanyId", subCompanyId },
        { "MainCompanyId", MainCompanyId },
        { "ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId },
        { "LastInsertDate", israelTime.ToString("o") } // Set LastInsertDate as ISO 8601 string in Israel time
    };

            // Add the existing JSON data to the BsonDocument
            foreach (var property in jsonData.EnumerateObject())
            {
                // Handle the "suppliers" field specifically to ensure it's a BsonDocument
                if (property.Name == "suppliers" && property.Value.ValueKind == JsonValueKind.Object)
                {
                    var suppliersBson = BsonDocument.Parse(property.Value.GetRawText());
                    newBsonDocument.Add("suppliers", suppliersBson);
                }
                else
                {
                    newBsonDocument[property.Name] = BsonValue.Create(property.Value.ToString());
                }
            }

            // Define the filter for finding the existing document with "suppliers"
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Eq("userId", userId) &
                         filterBuilder.Eq("subCompanyId", subCompanyId) &
                         filterBuilder.Eq("MainCompanyId", MainCompanyId) &
                         filterBuilder.Eq("ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId) &
                         filterBuilder.Exists("suppliers", true);  // Ensure the document has a "suppliers" field

            // Retrieve the existing document
            var existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();

            if (existingDocument != null)
            {
                // Ensure "suppliers" is a BsonDocument
                var existingSuppliers = existingDocument.Contains("suppliers") ? existingDocument["suppliers"].AsBsonDocument : new BsonDocument();
                var newSuppliers = newBsonDocument.Contains("suppliers") ? newBsonDocument["suppliers"].AsBsonDocument : new BsonDocument();

                foreach (var newSupplier in newSuppliers.Elements)
                {
                    var supplierId = newSupplier.Name;
                    var newSupplierData = newSupplier.Value.AsBsonDocument;

                    if (existingSuppliers.Contains(supplierId))
                    {
                        // If the existing supplier has a non-empty email, retain it
                        var existingEmail = existingSuppliers[supplierId]["email"].AsString;
                        if (!string.IsNullOrEmpty(existingEmail))
                        {
                            newSupplierData["email"] = existingEmail;
                        }
                    }

                    // Update the supplier data in the existing document
                    existingSuppliers[supplierId] = newSupplierData;
                }

                // Update the existing document with merged suppliers and other updated fields
                var update = Builders<BsonDocument>.Update
                    .Set("suppliers", existingSuppliers)
                    .Set("LastInsertDate", israelTime.ToString("o")); // Convert to ISO 8601 string in Israel time

                await _Icount_BussinesPartner_Clients_Suplliers.UpdateOneAsync(filter, update);
            }
            else
            {
                // If no existing document is found, insert the new document
                await _Icount_BussinesPartner_Clients_Suplliers.InsertOneAsync(newBsonDocument);
            }
        }



        private async Task<TotalBusinessPartnerProp> GetClients(string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId, int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId, int pageNumber, int pageSize, int Lang)
        {
            try
            {
                var clientsList = new List<BusinessPartnerProp>();
                string typeLabel = Lang == 1 ? "Client" : "לקוח"; // Set the default Type based on the language

                // Check if the document exists in the collection
                var filterBuilder = Builders<BsonDocument>.Filter;
                var filter = filterBuilder.Eq("userId", userId) &
                             filterBuilder.Eq("subCompanyId", subCompanyId) &
                             filterBuilder.Eq("MainCompanyId", MainCompanyId) &
                             filterBuilder.Eq("ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId) &
                             filterBuilder.Exists("clients", true);
                var existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();

                if (existingDocument != null)
                {
                    DateTime lastInsertDate;
                    if (DateTime.TryParse(existingDocument["LastInsertDate"].AsString, out lastInsertDate))
                    {
                        lastInsertDate = lastInsertDate.ToUniversalTime();
                        TimeSpan differenceyear = DateTime.UtcNow - lastInsertDate;

                        if (differenceyear.TotalDays > 0 )
                        {
                            await RefreshClientData(cidvalue, uservalue, passvalue, userId, subCompanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);

                            // Re-fetch the updated document after refresh
                            existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();
                        }

                        // Now proceed with processing the (updated) document
                        if (existingDocument != null)
                        {
                            var clientData = existingDocument.GetValue("clients").AsBsonDocument;
                            var vatIds = clientData.Names.Select(name => clientData[name]["vat_id"].AsString).ToList();
                            var clientEmails = clientData.Names.Select(name => clientData[name]["email"].AsString).ToList();

                            // Query the status for all VAT IDs at once
                            var statuses = await GetClientStatuses(vatIds);

                            // Batch query for all relevant BusinessPartnersEmails records
                            var businessPartnersObjs = await _repository.GetListOfObjectsAsync<BusinessPartnersEmails>(
                                x => x.UserId == userId &&
                                     x.SubCompanyId == subCompanyId &&
                                     x.OrganizationId == MainCompanyId &&
                                     clientEmails.Contains(x.Email)
                            );

                            var businessPartnersDict = businessPartnersObjs.ToDictionary(x => x.Email);

                            foreach (var clientName in clientData.Names)
                            {
                                var clientInfo = clientData[clientName].AsBsonDocument;
                                string businesspartnerName = clientInfo.GetValue("client_name").AsString;
                                string vatId = clientInfo.GetValue("vat_id").AsString;
                                string clientEmail = clientInfo.GetValue("email").AsString;

                                var status = statuses.Contains(vatId);
                                businessPartnersDict.TryGetValue(clientEmail, out var businessPartnersObj);

                                string UIStatus;
                                string actionLabel;
                                ActionItem action = null;

                                if (status)
                                {
                                    UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
                                    actionLabel = Lang == 1 ? "Connected" : "מחובר";
                                    action = new ActionItem(actionLabel, ActionType.String);
                                }
                                else
                                {
                                    UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
                                    if (businessPartnersObj != null && businessPartnersObj.EmailSent.HasValue && businessPartnersObj.EmailSent.Value)
                                    {
                                        var difference = DateTime.Now - businessPartnersObj.LastDateSent.Value;
                                        if (difference.TotalDays < 365)
                                        {
                                            action = new ActionItem(businessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
                                        }
                                        else
                                        {
                                            actionLabel = Lang == 1 ? "Invite" : "הזמן";
                                            action = new ActionItem(actionLabel, ActionType.Button);
                                        }
                                    }
                                    else if (businessPartnersObj != null && businessPartnersObj.Email==string.Empty)
                                    {
                                        UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
                                        actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
                                        action = new ActionItem(actionLabel, ActionType.Button);
                                    }
                                    else if ((businessPartnersObj != null && businessPartnersObj.EmailSent.HasValue && businessPartnersObj.EmailSent.Value==false && businessPartnersObj.Email!=string.Empty) || (businessPartnersObj == null) )
                                    {
                                        UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
                                        actionLabel = Lang == 1 ? "Invite" : "הזמן";
                                        action = new ActionItem(actionLabel, ActionType.Button);
                                    }
                                    
                                }

                                clientsList.Add(new BusinessPartnerProp
                                {
                                    BusinesspartnerName = businesspartnerName,
                                    VatId = vatId,
                                    DocAmount = 0,
                                    Email = clientEmail,
                                    SubCompanyId = subCompanyId,
                                    Type = typeLabel,
                                    Status = UIStatus,
                                    LastInvitationDate = businessPartnersObj?.LastDateSent,
                                    Actions = action
                                });
                            }
                        }
                    }
                }
                else
                {
                    await RefreshClientData(cidvalue, uservalue, passvalue, userId, subCompanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);
                    // Re-fetch and process after refresh
                    existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();

                    if (existingDocument != null)
                    {
                        var clientData = existingDocument.GetValue("clients").AsBsonDocument;
                        var vatIds = clientData.Names.Select(name => clientData[name]["vat_id"].AsString).ToList();
                        var clientEmails = clientData.Names.Select(name => clientData[name]["email"].AsString).ToList();

                        // Query the status for all VAT IDs at once
                        var statuses = await GetClientStatuses(vatIds);

                        // Batch query for all relevant BusinessPartnersEmails records
                        var businessPartnersObjs = await _repository.GetListOfObjectsAsync<BusinessPartnersEmails>(
                            x => x.UserId == userId &&
                                 x.SubCompanyId == subCompanyId &&
                                 x.OrganizationId == MainCompanyId &&
                                 clientEmails.Contains(x.Email)
                        );

                        var businessPartnersDict = businessPartnersObjs.ToDictionary(x => x.Email);

                        foreach (var clientName in clientData.Names)
                        {
                            var clientInfo = clientData[clientName].AsBsonDocument;
                            string businesspartnerName = clientInfo.GetValue("client_name").AsString;
                            string vatId = clientInfo.GetValue("vat_id").AsString;
                            string clientEmail = clientInfo.GetValue("email").AsString;

                            var status = statuses.Contains(vatId);
                            businessPartnersDict.TryGetValue(clientEmail, out var businessPartnersObj);

                            string UIStatus;
                            string actionLabel;
                            ActionItem action = null;

                            if (status)
                            {
                                UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
                                actionLabel = Lang == 1 ? "Connected" : "מחובר";
                                action = new ActionItem(actionLabel, ActionType.String);
                            }
                            else
                            {
                                UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
                                if (businessPartnersObj != null && businessPartnersObj.EmailSent.HasValue && businessPartnersObj.EmailSent.Value)
                                {
                                    var difference = DateTime.Now - businessPartnersObj.LastDateSent.Value;
                                    if (difference.TotalDays < 365)
                                    {
                                        action = new ActionItem(businessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
                                    }
                                    else
                                    {
                                        actionLabel = Lang == 1 ? "Invite" : "הזמן";
                                        action = new ActionItem(actionLabel, ActionType.Button);
                                    }
                                }
                                else if (businessPartnersObj != null && businessPartnersObj.Email == string.Empty)
                                {
                                    UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
                                    actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
                                    action = new ActionItem(actionLabel, ActionType.Button);
                                }
                                else if ((businessPartnersObj != null && businessPartnersObj.EmailSent.HasValue && businessPartnersObj.EmailSent.Value == false && businessPartnersObj.Email != string.Empty) || (businessPartnersObj == null))
                                {
                                    UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
                                    actionLabel = Lang == 1 ? "Invite" : "הזמן";
                                    action = new ActionItem(actionLabel, ActionType.Button);
                                }
                            }

                            clientsList.Add(new BusinessPartnerProp
                            {
                                BusinesspartnerName = businesspartnerName,
                                VatId = vatId,
                                DocAmount = 0,
                                Email = clientEmail,
                                SubCompanyId = subCompanyId,
                                Type = typeLabel,
                                Status = UIStatus,
                                LastInvitationDate = businessPartnersObj?.LastDateSent,
                                Actions = action
                            });
                        }
                    }
                }

                // Sort the list by DocAmount in descending order
                clientsList = clientsList.OrderByDescending(s => s.DocAmount).ToList();

                int totalResults = clientsList.Count;

                clientsList = clientsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
                var totalBusinessPartnerPropObj = new TotalBusinessPartnerProp
                {
                    BusinessPartnerProp = clientsList,
                    TotalResults = totalResults
                };
                return totalBusinessPartnerPropObj;
            }
            catch (Exception ex)
            {
                // Handle the exception appropriately
                return null;
            }
        }

        private async Task<HashSet<string>> GetClientStatuses(List<string> vatIds)
        {
            var filter = Builders<BsonDocument>.Filter.In("company_info.vat_id", vatIds);
            var documents = await _IcountCompaniesInfoCollection.Find(filter).ToListAsync();
            var statusVatIds = new HashSet<string>(documents.Select(doc => doc["company_info"]["vat_id"].AsString));
            return statusVatIds;
        }

        private async System.Threading.Tasks.Task RefreshClientData(string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId, int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId)
        {
            // Get the endpoint
            var clientget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 84);
            var endpointclientget_list = clientget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
            HttpMethod methodclientget_list = HttpMethod.Get;
            var Reponsneclientget_list = await SendRequest(endpointclientget_list, methodclientget_list);

            // Parse the JSON response using JsonDocument
            using var jsonDocument = JsonDocument.Parse(Reponsneclientget_list);
            var jsonData = jsonDocument.RootElement;

            // Manually construct a new BsonDocument with additional fields
            var newBsonDocument = new BsonDocument
    {
        { "userId", userId },
        { "subCompanyId", subCompanyId },
        { "MainCompanyId", MainCompanyId },
        { "ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId },
        { "LastInsertDate", DateTime.UtcNow }
    };

            // Add the existing JSON data to the BsonDocument
            foreach (var property in jsonData.EnumerateObject())
            {
                // Handle the "clients" field specifically to ensure it's a BsonDocument
                if (property.Name == "clients" && property.Value.ValueKind == JsonValueKind.Object)
                {
                    var clientsBson = BsonDocument.Parse(property.Value.GetRawText());
                    newBsonDocument.Add("clients", clientsBson);
                }
                else
                {
                    newBsonDocument[property.Name] = BsonValue.Create(property.Value.ToString());
                }
            }

            // Define the filter for finding the existing document with "clients"
            var filterBuilder = Builders<BsonDocument>.Filter;
            var filter = filterBuilder.Eq("userId", userId) &
                         filterBuilder.Eq("subCompanyId", subCompanyId) &
                         filterBuilder.Eq("MainCompanyId", MainCompanyId) &
                         filterBuilder.Eq("ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId) &
                         filterBuilder.Exists("clients", true);  // Ensure the document has a "clients" field

            // Retrieve the existing document
            var existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();

            if (existingDocument != null)
            {
                // Ensure "clients" is a BsonDocument
                var existingClients = existingDocument.Contains("clients") ? existingDocument["clients"].AsBsonDocument : new BsonDocument();
                var newClients = newBsonDocument.Contains("clients") ? newBsonDocument["clients"].AsBsonDocument : new BsonDocument();

                foreach (var newClient in newClients.Elements)
                {
                    var clientId = newClient.Name;
                    var newClientData = newClient.Value.AsBsonDocument;

                    if (existingClients.Contains(clientId))
                    {
                        // If the existing client has a non-empty email, retain it
                        var existingEmail = existingClients[clientId]["email"].AsString;
                        if (!string.IsNullOrEmpty(existingEmail))
                        {
                            newClientData["email"] = existingEmail;
                        }
                    }

                    // Update the client data in the existing document
                    existingClients[clientId] = newClientData;
                }

                // Update the existing document with merged clients and other updated fields
                var update = Builders<BsonDocument>.Update
                    .Set("clients", existingClients)
                    .Set("LastInsertDate", DateTime.UtcNow.ToString("o")); // "o" is for ISO 8601 format

                await _Icount_BussinesPartner_Clients_Suplliers.UpdateOneAsync(filter, update);
            }
            else
            {
                // If no existing document is found, insert the new document
                await _Icount_BussinesPartner_Clients_Suplliers.InsertOneAsync(newBsonDocument);
            }
        }




        // Function to handle case 2 logic
        //private async Task<TotalBusinessPartnerProp> GetClients(string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId, int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId, int pageNumber, int pageSize, int Lang)
        //{
        //    var clientsList = new List<BusinessPartnerProp>();
        //    string UIStatus = "";
        //    string actionLabel = "";
        //    string typeLabel = Lang == 1 ? "Client" : "לקוח"; // Set the default Type based on the language

        //    // Check if the document exists in the collection
        //    var filterBuilder = Builders<BsonDocument>.Filter;
        //    var filter = filterBuilder.Eq("userId", userId) &
        //                 filterBuilder.Eq("subCompanyId", subCompanyId) &
        //                 filterBuilder.Eq("MainCompanyId", MainCompanyId) &
        //                 filterBuilder.Eq("ExtrnalsystemIdOfsubCopmanyId", ExtrnalsystemIdOfsubCopmanyId) &
        //                 filterBuilder.Exists("clients", true);
        //    var existingDocument = await _Icount_BussinesPartner_Clients_Suplliers.Find(filter).FirstOrDefaultAsync();
        //    DateTime lastInsertDate = new DateTime();

        //    if (existingDocument != null)
        //    {
        //        var dateString = existingDocument["LastInsertDate"].AsString;

        //        if (DateTime.TryParse(dateString, out lastInsertDate))
        //        {
        //            lastInsertDate = lastInsertDate.ToUniversalTime();
        //        }

        //        TimeSpan differenceyear = DateTime.UtcNow - lastInsertDate;

        //        if (differenceyear.TotalDays < 365)
        //        {
        //            var clientData = existingDocument.GetValue("clients");
        //            foreach (var client in clientData.AsBsonDocument)
        //            {
        //                var clientInfo = client.Value.AsBsonDocument;
        //                string businesspartnerName = clientInfo.GetValue("client_name").AsString;
        //                string vatId = clientInfo.GetValue("vat_id").AsString;
        //                string ClientEmail = clientInfo.GetValue("email").AsString;
        //                int docAmount = await GetDocAmountclient(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue);
        //                bool status = await GetStatus(vatId);

        //                var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(x =>x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId);

        //                ActionItem action = null;

        //                if (status)
        //                {
        //                    UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                    actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                    action = new ActionItem(actionLabel, ActionType.String);
        //                }
        //                else
        //                {
        //                    UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
        //                    if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue && BusinessPartnersObj.EmailSent.Value)
        //                    {
        //                        var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                        if (difference.TotalDays < 365)
        //                        {
        //                            action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                        }
        //                        else
        //                        {
        //                            actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                            action = new ActionItem(actionLabel, ActionType.Button);
        //                        }
        //                    }
        //                    else if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue && !BusinessPartnersObj.EmailSent.Value)
        //                    {
        //                        UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
        //                        actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
        //                        action = new ActionItem(actionLabel, ActionType.Button);
        //                    }
        //                    else
        //                    {
        //                        UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
        //                        actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                        action = new ActionItem(actionLabel, ActionType.Button);
        //                    }
        //                }

        //                clientsList.Add(new BusinessPartnerProp
        //                {
        //                    BusinesspartnerName = businesspartnerName,
        //                    VatId = vatId,
        //                    DocAmount = docAmount,
        //                    Email = ClientEmail,
        //                    SubCompanyId = subCompanyId,
        //                    Type = typeLabel,
        //                    Status = UIStatus,
        //                    LastInvitationDate = BusinessPartnersObj?.LastDateSent,
        //                    Actions = action
        //                });
        //            }
        //        }
        //        else
        //        {
        //            await _Icount_BussinesPartner_Clients_Suplliers.DeleteOneAsync(filter);
        //            var clientget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 84);
        //            var endpointclientget_list = clientget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //            HttpMethod methodclientget_list = HttpMethod.Get;
        //            var Reponsneclientget_list = await SendRequest(endpointclientget_list, methodclientget_list);

        //            var modifiedJson = JsonConvert.DeserializeObject<JObject>(Reponsneclientget_list);
        //            modifiedJson["userId"] = userId;
        //            modifiedJson["subCompanyId"] = subCompanyId;
        //            modifiedJson["MainCompanyId"] = MainCompanyId;
        //            modifiedJson["ExtrnalsystemIdOfsubCopmanyId"] = ExtrnalsystemIdOfsubCopmanyId;
        //            modifiedJson["LastInsertDate"] = DateTime.UtcNow;

        //            await _Icount_BussinesPartner_Clients_Suplliers.InsertOneAsync(BsonDocument.Parse(modifiedJson.ToString()));

        //            var jsonDocument = JsonDocument.Parse(Reponsneclientget_list);
        //            var jsonData = jsonDocument.RootElement;
        //            var clientsData = jsonData.GetProperty("clients");

        //            foreach (var client in clientsData.EnumerateObject())
        //            {
        //                var clientData = client.Value;
        //                string businesspartnerName = clientData.GetProperty("client_name").GetString();
        //                string vatId = clientData.GetProperty("vat_id").GetString();
        //                string ClientEmail = clientData.GetProperty("email").GetString();
        //                int docAmount = await GetDocAmountclient(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue);
        //                bool status = await GetStatus(vatId);

        //                var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(x => x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId);
        //                ActionItem action = null;

        //                if (status)
        //                {
        //                    UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                    actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                    action = new ActionItem(actionLabel, ActionType.String);
        //                }
        //                else
        //                {
        //                    UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";
        //                    if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue && BusinessPartnersObj.EmailSent.Value)
        //                    {
        //                        var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                        if (difference.TotalDays < 365)
        //                        {
        //                            action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                        }
        //                        else
        //                        {
        //                            actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                            action = new ActionItem(actionLabel, ActionType.Button);
        //                        }
        //                    }
        //                    else if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue && !BusinessPartnersObj.EmailSent.Value)
        //                    {
        //                        UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
        //                        actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
        //                        action = new ActionItem(actionLabel, ActionType.Button);
        //                    }
        //                    else
        //                    {
        //                        UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
        //                        actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                        action = new ActionItem(actionLabel, ActionType.Button);
        //                    }
        //                }

        //                clientsList.Add(new BusinessPartnerProp
        //                {
        //                    BusinesspartnerName = businesspartnerName,
        //                    VatId = vatId,
        //                    DocAmount = docAmount,
        //                    Email = ClientEmail,
        //                    SubCompanyId = subCompanyId,
        //                    Type = typeLabel,
        //                    Status = UIStatus,
        //                    LastInvitationDate = BusinessPartnersObj?.LastDateSent,
        //                    Actions = action
        //                });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        var clientget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 84);
        //        var endpointclientget_list = clientget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //        HttpMethod methodclientget_list = HttpMethod.Get;
        //        var Reponsneclientget_list = await SendRequest(endpointclientget_list, methodclientget_list);

        //        var modifiedJson = JsonConvert.DeserializeObject<JObject>(Reponsneclientget_list);
        //        modifiedJson["userId"] = userId;
        //        modifiedJson["subCompanyId"] = subCompanyId;
        //        modifiedJson["MainCompanyId"] = MainCompanyId;
        //        modifiedJson["ExtrnalsystemIdOfsubCopmanyId"] = ExtrnalsystemIdOfsubCopmanyId;
        //        modifiedJson["LastInsertDate"] = DateTime.UtcNow;

        //        await _Icount_BussinesPartner_Clients_Suplliers.InsertOneAsync(BsonDocument.Parse(modifiedJson.ToString()));

        //        var jsonDocument = JsonDocument.Parse(Reponsneclientget_list);
        //        var jsonData = jsonDocument.RootElement;
        //        var clientsData = jsonData.GetProperty("clients");

        //        foreach (var client in clientsData.EnumerateObject())
        //        {
        //            var clientData = client.Value;
        //            string businesspartnerName = clientData.GetProperty("client_name").GetString();
        //            string vatId = clientData.GetProperty("vat_id").GetString();
        //            string ClientEmail = clientData.GetProperty("email").GetString();
        //            int docAmount = await GetDocAmountclient(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue);
        //            bool status = await GetStatus(vatId);

        //            var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(x => x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId);
        //            ActionItem action = null;

        //            if (status)
        //            {
        //                UIStatus = Lang == 1 ? "Connected to Uninet" : "מחובר ליונינט";
        //                actionLabel = Lang == 1 ? "Connected" : "מחובר";
        //                action = new ActionItem(actionLabel, ActionType.String);
        //            }
        //            else
        //            {
        //                UIStatus = Lang == 1 ? "Still not connected" : "עדיין לא מחובר ליונינט";

        //                if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue && BusinessPartnersObj.EmailSent.Value)
        //                {
        //                    var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
        //                    if (difference.TotalDays < 365)
        //                    {
        //                        action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
        //                    }
        //                    else
        //                    {
        //                        actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                        action = new ActionItem(actionLabel, ActionType.Button);
        //                    }
        //                }
        //                else if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue && !BusinessPartnersObj.EmailSent.Value)
        //                {
        //                    UIStatus = Lang == 1 ? "Missing email details" : "פרטיי אי מייל חסרים";
        //                    actionLabel = Lang == 1 ? "Complete details" : "השלם פרטים";
        //                    action = new ActionItem(actionLabel, ActionType.Button);
        //                }
        //                else
        //                {
        //                    UIStatus = Lang == 1 ? "Waiting for invitation" : "מחכה להזמנה";
        //                    actionLabel = Lang == 1 ? "Invite" : "הזמן";
        //                    action = new ActionItem(actionLabel, ActionType.Button);
        //                }
        //            }

        //            clientsList.Add(new BusinessPartnerProp
        //            {
        //                BusinesspartnerName = businesspartnerName,
        //                VatId = vatId,
        //                DocAmount = docAmount,
        //                Email = ClientEmail,
        //                SubCompanyId = subCompanyId,
        //                Type = typeLabel,
        //                Status = UIStatus,
        //                LastInvitationDate = BusinessPartnersObj?.LastDateSent,
        //                Actions = action
        //            });
        //        }
        //    }

        //    // Sort the list by DocAmount in descending order
        //    clientsList = clientsList.OrderByDescending(s => s.DocAmount).ToList();

        //    int totalResults = clientsList.Count;

        //    clientsList = clientsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        //    var TotalBusinessPartnerPropObj = new TotalBusinessPartnerProp
        //    {
        //        BusinessPartnerProp = clientsList,
        //        TotalResults = totalResults
        //    };
        //    return TotalBusinessPartnerPropObj;
        //}



        public async Task<TotalBusinessPartnerProp> GetBusinessPartnersByFilter(int filterType, int userId, int subCopmanyId, int pageNumber, int pageSize, int Lang)
        {
            try
            {
                /*
                  Supplier = 1,
                  Client = 2,
                  Both = 3
                */
                //get  the companyid related to the user 
                var MainCompanyIdObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
                int MainCompanyId = MainCompanyIdObj.BusinessId;

                ///get credentials of userexternalsystem 
                var UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == MainCompanyId && x.Userid == userId && x.SubCompayId == subCopmanyId);
                int ExtrnalsystemIdOfsubCopmanyId = UserexternalSystemDynamicFieldslist[0].ExternalSystemId;
                string cidvalue = null;
                string uservalue = null;
                string passvalue = null;

                //from here 
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
                TotalBusinessPartnerProp businessPartnersSupliers = new TotalBusinessPartnerProp();
                TotalBusinessPartnerProp businessPartnersClients = new TotalBusinessPartnerProp();
                switch (filterType)
                {
                    case 1:
                        //get suppliers
                        businessPartnersSupliers = await GetSuppliers(cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId,  pageNumber,  pageSize,false, Lang);
                        return businessPartnersSupliers;



                        break;
                    case 2:
                        businessPartnersClients = await GetClients(cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId, pageNumber, pageSize, Lang);
                        return businessPartnersClients;





                        break;
                    case 3:
                        // Retrieve and sort suppliers by DocAmount in descending order
                        var PartnersSuppliers = await GetSuppliers(
                            cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId, 1, int.MaxValue,false, Lang
                        ); // Retrieve all suppliers

                        // Retrieve and sort clients by DocAmount in descending order
                        var PartnersClients = await GetClients(
                            cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId, 1, int.MaxValue, Lang
                        ); // Retrieve all clients


                        // Safely handle null lists
                        var suppliersList = PartnersSuppliers?.BusinessPartnerProp ?? new List<BusinessPartnerProp>();
                        var clientsList = PartnersClients?.BusinessPartnerProp ?? new List<BusinessPartnerProp>();

                        /// Combine and sort
                        var mergedList = suppliersList
                            .Concat(clientsList)
                            .OrderByDescending(p => p.DocAmount)
                            .ToList();

                        int MergedlistCounter = mergedList.Count;

                        // Apply pagination
                        var paginatedMergedList = mergedList
                            .Skip((pageNumber - 1) * pageSize)
                            .Take(pageSize)
                            .ToList();

                        return new TotalBusinessPartnerProp
                        {
                            BusinessPartnerProp = paginatedMergedList,
                            TotalResults = MergedlistCounter
                        };

                        break;

                    case 4:
                        //get suppliers
                        businessPartnersSupliers = await GetSuppliers(cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId, pageNumber, pageSize,true,Lang);
                        return businessPartnersSupliers;
                }



                return null;
                // Add more rows as needed...


            }
            catch (Exception ex) { return null; }
        }




        private async Task<List<MainSubCopmaniesMasters>> GetRelatedCompanies(int MainCompanyId, int UserID)
        {
            return await _repository.GetListOfObjectsAsync<MainSubCopmaniesMasters>(x => x.MainCompanyId == MainCompanyId);
        }
        private int? ResolveSubCompanyId(List<MainSubCopmaniesMasters> companies, int MainCompanyId)
        {
            if (companies.Count > 1)
            {
                return companies
                    .OrderByDescending(x => x.LastTimeDataShowed)
                    .FirstOrDefault()?.SubCopmanyId;
            }
            return companies.FirstOrDefault()?.SubCopmanyId;
        }


        private async Task<int> ResolveMainCompanyId(int UserID)
        {
            var CheckUsermasterExist = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == UserID);

            if (CheckUsermasterExist != null) // User is a master
            {
                var MainCompanyIdObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);
                return MainCompanyIdObj.BusinessId;
            }
            else // User is not a master
            {
                var CompanyIdObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);
                var subCompanyIdObj = await _repository.GetFirstObjectAsync<UsersExternalSystemDynamicFields>(
                    x => x.Userid == UserID && x.Companyid == CompanyIdObj.BusinessId
                );
                var MainCompanyIdObj = await _repository.GetFirstObjectAsync<SubUserCredentials>(
                    x => x.SubUserId == UserID && x.SubCompanyId == subCompanyIdObj.SubCompayId
                );

                return MainCompanyIdObj?.CompanyId ?? CompanyIdObj.BusinessId;
            }
        }


        private ExternalSystemConfig GetConfigByExternalSystemId(int externalSystemId)
        {
            if (!_externalSystemConfig.ContainsKey(externalSystemId))
                throw new Exception($"Configuration for ExternalSystemId {externalSystemId} not found.");

            return _externalSystemConfig[externalSystemId];
        }


        
        private async Task<int> GetExternalSystemIdbyjsonId(int UserID, string JsonDocumentid)
        {
            var businessDataResult = await _repository.GetFirstObjectAsync<BusinessData>(
                b => b.JsonDocumentid == JsonDocumentid
            );

            if (businessDataResult != null)
            {
                // Extract values from the BusinessData result
                int subCompanyId = businessDataResult.SubCompanyId;
                int userId = businessDataResult.UserId;

                // Step 2: Retrieve ExternalSystemId from UsersExternalSystemDynamicFields table
                var externalSystem = await _repository.GetFirstObjectAsync<UsersExternalSystemDynamicFields>(
                    u => u.Userid == userId && u.SubCompayId == subCompanyId
                );

                if (externalSystem != null)
                {
                    // Successfully retrieved ExternalSystemId
                    int externalSystemId = externalSystem.ExternalSystemId;

                    return externalSystemId;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private async Task<int> GetExternalSystemId(int UserID, int subCompanyId)
        {
            var externalSystem = await _repository.GetFirstObjectAsync<UsersExternalSystemDynamicFields>(
                x => x.Userid == UserID && x.SubCompayId == subCompanyId
            );

            if (externalSystem == null)
                throw new Exception($"No ExternalSystemId found for UserID {UserID} and SubCompanyId {subCompanyId}.");

            return externalSystem.ExternalSystemId;
        }

        private FilterDefinition<BsonDocument> BuildFilter(int companyId, int subCompanyId, string companyFieldPath, string subCompanyFieldPath)
        {
            return Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq(companyFieldPath, companyId),
                Builders<BsonDocument>.Filter.Eq(subCompanyFieldPath, subCompanyId)
            );
        }
        private async Task<string> GetVatId(int MainCompanyId, int subCompanyId, ExternalSystemConfig config)
        {
            var filter = BuildFilter(MainCompanyId, subCompanyId, config.CompanyFieldPath, config.SubCompanyFieldPath);

            var projection = Builders<BsonDocument>.Projection.Include(config.VatFieldPath).Exclude("_id");
            var result = config.CompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();

            string[] pathSegments = config.VatFieldPath.Split('.');
            BsonValue fieldValue = result;

            foreach (var segment in pathSegments)
            {
                if (fieldValue == null || !fieldValue.AsBsonDocument.Contains(segment))
                    return string.Empty;

                fieldValue = fieldValue[segment];
            }

            return fieldValue?.AsString ?? string.Empty;
        }
        private async Task<string> FetchSupplierName(int businessId, int subCompanyId, ExternalSystemConfig config)
        {
            var filter = BuildFilter(businessId, subCompanyId, config.CompanyFieldPath, config.SubCompanyFieldPath);

            var projection = Builders<BsonDocument>.Projection.Include(config.SupplierFieldPath).Exclude("_id");
            var result = config.CompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();

            string[] pathSegments = config.SupplierFieldPath.Split('.');
            BsonValue fieldValue = result;

            foreach (var segment in pathSegments)
            {
                if (fieldValue == null || !fieldValue.AsBsonDocument.Contains(segment))
                    return string.Empty;

                fieldValue = fieldValue[segment];
            }

            return fieldValue?.AsString ?? string.Empty;
        }
        private async Task<string> FetchJsonDocUrl(string jsonDocumentId, ExternalSystemConfig config)
        {
            try
            {
                // Query filter for the specific JSON document ID
                var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(jsonDocumentId));

                // Set the projection to include the top-level field containing the URL
                string topLevelField = config.UrlFieldPath.Split('.')[0];
                var projection = Builders<BsonDocument>.Projection.Include(topLevelField).Exclude("_id");

                // Execute the query on the WebhookCollection
                var result = await config.WebhookCollection.Find(filter).Project(projection).FirstOrDefaultAsync();

                // Traverse the result to access the nested field
                if (result != null)
                {
                    var urlFieldParts = config.UrlFieldPath.Split('.');
                    BsonValue field = result;

                    foreach (var part in urlFieldParts)
                    {
                        if (field != null && field.IsBsonDocument && field.AsBsonDocument.Contains(part))
                        {
                            field = field[part];
                        }
                        else
                        {
                            return string.Empty; // Return empty if field not found
                        }
                    }

                    return field?.AsString ?? string.Empty;
                }

                return string.Empty;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in FetchJsonDocUrl: {ex.Message}");
                return string.Empty;
            }
        }



        public async Task<DigitalDocumentToApproveObj> GetDigitalDocumentToApproveListByUser(int UserID, string Typelist, int? subCompanyId, int pageNumber, int pageSize)
        {
            try
            {
                // Initialize result object
                DigitalDocumentToApproveObj resultObj = new DigitalDocumentToApproveObj
                {
                    listDigitalDocumentToApprove = new List<DigitalDocumentToApprove>()
                };

                int MainCompanyId = await ResolveMainCompanyId(UserID);
                List<MainSubCopmaniesMasters> companies = await GetRelatedCompanies(MainCompanyId, UserID);

                // Resolve subCompanyId if not provided
                subCompanyId ??= ResolveSubCompanyId(companies, MainCompanyId);

                // Determine ExternalSystemId
                int externalSystemId = await GetExternalSystemId(UserID, subCompanyId.Value);

                // Fetch configuration
                var config = GetConfigByExternalSystemId(externalSystemId);

                // Get necessary credentials or token
                string apiToken = null, secretKey = null, cidvalue = null, uservalue = null, passvalue = null;

                if (externalSystemId == 6) // Morning API
                {
                    apiToken = await GetNewToken(MainCompanyId, subCompanyId.Value, UserID, externalSystemId);
                   
                    

                }
                else if (externalSystemId == 2) // iCount API
                {
                    var UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                        x => x.Companyid == MainCompanyId && x.Userid == UserID && x.SubCompayId == subCompanyId);

                    foreach (var dynamicField in UserexternalSystemDynamicFieldslist)
                    {
                        switch (dynamicField.FieldLabelName)
                        {
                            case "cid":
                                cidvalue = dynamicField.FieldLabelValue;
                                break;
                            case "user":
                                uservalue = dynamicField.FieldLabelValue;
                                break;
                            case "pass":
                                passvalue = dynamicField.FieldLabelValue;
                                break;
                        }
                    }
                }

                // Resolve VAT ID
                string vatId = await GetVatId(MainCompanyId, subCompanyId.Value, config);

                // Fetch paginated documents
                List<BusinessData> ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
                    UserID, Typelist, subCompanyId, vatId, pageNumber, pageSize, null
                );

                // Process each document
                foreach (var doc in ResListOfClientCompaniesThatWasSentDigitalDocument)
                {
                    string JsonDocUrl = string.Empty;
                    string supplierNameSender = string.Empty;

                    if (doc.DataSourceEnum == externalSystemId && doc.DataSourceType == 2)
                    {
                        JsonDocUrl = await FetchJsonDocUrl(doc.JsonDocumentid, config);
                       
                           
                  
                        supplierNameSender = await FetchSupplierName(doc.BusinessId, doc.SubCompanyId, config);
                    }

                    resultObj.listDigitalDocumentToApprove.Add(new DigitalDocumentToApprove
                    {
                        JsonDocumentid = doc.JsonDocumentid,
                        ClientVat_id = doc.ClientVat_id,
                        SendingDigitalDocumentBusinessID = doc.BusinessId,
                        BusinessVatId = doc.BusinessVatId,
                        DocInfoUrl = JsonDocUrl,
                        supplier_name_Sender = supplierNameSender,
                        docDate = doc.docDate,
                        amountAV = doc.amountAV,
                        currency_code = doc.currency_code
                    });
                }

                // Populate additional details
                resultObj.fullname = await GetUserFullName(UserID);
                resultObj.ListOfSubCompaniesandNames = await PopulateListOfSubCompaniesandNames(companies, UserID, externalSystemId);

                // Process specific actions for iCount API
                if (externalSystemId == 2)
                {
                    await ProcessICountClientInfo(MainCompanyId, UserID, cidvalue, uservalue, passvalue);//הערה אייל גם זה רק ספקים
                    await ProcessICountExpenses(MainCompanyId, UserID, cidvalue, uservalue, passvalue);
                    await ProcessICountExpensesTypes(MainCompanyId, UserID, cidvalue, uservalue, passvalue);
                }


                if (externalSystemId == 6) // Morning API
                {
                    string token = await GetNewToken(MainCompanyId, subCompanyId.Value, UserID, externalSystemId);
                    await ProcessMorningSupplierInfo(MainCompanyId, UserID, token);//הערה אייל זה רק ספקים 
                    await ProcessMorningExpensesTypes(MainCompanyId, UserID, token);
                    await ProcessMorningExpenses(MainCompanyId, UserID, token);
                    

                    
                }




                return resultObj;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
        private async Task ProcessMorningSupplierInfo(int MainCompanyId, int UserID, string token)
        {
            try
            {
                // Fetch the endpoint configuration for Morning Client Info
                var clientInfoEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 56); // Replace with the correct ID
                var clientInfoEndpoint = clientInfoEndpointObj.Endpoint; 
                HttpMethod method = HttpMethod.Post;

                // Send the request to the Morning API
                string response = await SendRequestWithToken(clientInfoEndpoint, method, token);

                // Parse the JSON response
                var jsonDocument = JsonDocument.Parse(response);
                var jsonData = jsonDocument.RootElement;

                // Convert the JSON response to a BsonDocument
                var bsonDocument = BsonDocument.Parse(jsonData.ToString());
                bsonDocument.Add("internalCompanyId", MainCompanyId);
                bsonDocument.Add("UserID", UserID);

                // Define the filter to check for existing documents
                var filterClientSuppliers = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("internalCompanyId", MainCompanyId),
                    Builders<BsonDocument>.Filter.Eq("UserID", UserID)
                );

                // Check for existing document and delete if necessary
                var existingDocument = await _MorningClientSuppliers.Find(filterClientSuppliers).FirstOrDefaultAsync();
                if (existingDocument != null)
                {
                    await _MorningClientSuppliers.DeleteOneAsync(filterClientSuppliers);
                }

                // Insert the new document
                await _MorningClientSuppliers.InsertOneAsync(bsonDocument);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessMorningClientInfo: {ex.Message}");
            }
        }

        private async Task ProcessMorningExpenses(int MainCompanyId, int UserID, string token)
        {
            try
            {
                // Fetch the endpoint configuration for Morning Expenses
                var expenseSearchObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 46); // Update with correct ID
                var expenseSearchEndpoint = expenseSearchObj.Endpoint; // Example: https://sandbox.d.greeninvoice.co.il/api/v1/expenses/search
                HttpMethod method = HttpMethod.Post;

                // Define the request payload for fetching expenses
                var payload = new
                {
                    page = 1,
                    pageSize = 100 // Adjust page size as necessary
                };
                string jsonPayload = JsonConvert.SerializeObject(payload);

                // Send the request to the Morning Expenses endpoint
                string response = await SendRequestWithToken(expenseSearchEndpoint, method, token, jsonPayload);

                // Parse the JSON response
                var jsonDocument = JsonDocument.Parse(response);
                if (jsonDocument.RootElement.TryGetProperty("items", out var itemsArray) && itemsArray.ValueKind == JsonValueKind.Array)
                {
                    // Define filter to check if the document for `internalCompanyId` and `UserID` exists
                    var filter = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", MainCompanyId),
                        Builders<BsonDocument>.Filter.Eq("UserID", UserID)
                    );

                    var existingDocument = await _MorningExpenses.Find(filter).FirstOrDefaultAsync();

                    if (existingDocument == null)
                    {
                        // Create a new document with all items under `results_list`
                        var newDocument = new BsonDocument
                {
                    { "internalCompanyId", MainCompanyId },
                    { "UserID", UserID },
                    { "status", true },
                    { "results_list", new BsonArray(itemsArray.EnumerateArray().Select(item => BsonDocument.Parse(item.ToString()))) }
                };

                        await _MorningExpenses.InsertOneAsync(newDocument);
                    }
                    else
                    {
                        // Check if any new expenses exist in the response
                        var existingExpenseIds = existingDocument["results_list"]
                            .AsBsonArray
                            .Select(doc => doc["id"].AsString)
                            .ToHashSet();

                        var newExpenses = itemsArray
                            .EnumerateArray()
                            .Where(item => !existingExpenseIds.Contains(item.GetProperty("id").GetString()))
                            .Select(item => BsonDocument.Parse(item.ToString()))
                            .ToList();

                        if (newExpenses.Any())
                        {
                            // Add new expenses to the `results_list`
                            var updatedResultsList = existingDocument["results_list"].AsBsonArray;
                            foreach (var newExpense in newExpenses)
                            {
                                updatedResultsList.Add(newExpense);
                            }

                            // Update the existing document
                            existingDocument["results_list"] = updatedResultsList;
                            await _MorningExpenses.ReplaceOneAsync(filter, existingDocument);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessMorningExpenses: {ex.Message}");
            }
        }


        private async Task ProcessMorningExpensesTypes(int MainCompanyId, int UserID, string token)
        {
            try
            {
                // Fetch the endpoint configuration for Morning Expenses Types
                var allExpensesTypeObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 47); // Update with correct ID
                var allExpensesEndpoint = allExpensesTypeObj.Endpoint; // Example: https://sandbox.d.greeninvoice.co.il/api/v1/expenses/statuses
                HttpMethod method = HttpMethod.Get;

                // Send a GET request with the token
                string response = await SendRequestWithToken(allExpensesEndpoint, method, token);

                // Parse the JSON response
                var expenseTypesResponse = JsonDocument.Parse(response).RootElement.EnumerateArray().Select(expenseType =>
                    new { Id = expenseType.GetProperty("id").GetInt32(), Name = expenseType.GetProperty("name").GetString() }).ToList();

                // Define filter to check if the document exists for internalCompanyId and UserID
                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("internalCompanyId", MainCompanyId),
                    Builders<BsonDocument>.Filter.Eq("UserID", UserID)
                );

                var existingDocument = await _MorningExpensesTypes.Find(filter).FirstOrDefaultAsync();

                if (existingDocument != null)
                {
                    // Check if all `id` values in the response exist under `expense_types` node
                    var existingExpenseTypes = existingDocument["expense_types"].AsBsonArray.Select(x => x["id"].AsInt32).ToList();
                    var missingExpenseTypes = expenseTypesResponse.Where(x => !existingExpenseTypes.Contains(x.Id)).ToList();

                    // Add missing expense types
                    foreach (var missing in missingExpenseTypes)
                    {
                        existingDocument["expense_types"].AsBsonArray.Add(new BsonDocument
                {
                    { "id", missing.Id },
                    { "name", missing.Name }
                });
                    }

                    // Update the document if changes were made
                    if (missingExpenseTypes.Any())
                    {
                        await _MorningExpensesTypes.ReplaceOneAsync(filter, existingDocument);
                    }
                }
                else
                {
                    // Create a new document if `internalCompanyId` and `UserID` do not exist
                    var newDocument = new BsonDocument
            {
                { "internalCompanyId", MainCompanyId },
                { "UserID", UserID },
                { "status", true },
                { "expense_types", new BsonArray(expenseTypesResponse.Select(x => new BsonDocument
                {
                    { "id", x.Id },
                    { "name", x.Name }
                })) }
            };

                    await _MorningExpensesTypes.InsertOneAsync(newDocument);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessMorningExpensesTypes: {ex.Message}");
            }
        }


        private async Task<string> SendRequestWithToken(string endpoint, HttpMethod method, string token, string payload = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response;
                if (method == HttpMethod.Post && payload != null)
                {
                    var content = new StringContent(payload, Encoding.UTF8, "application/json");
                    response = await client.PostAsync(endpoint, content);
                }
                else
                {
                    response = await client.GetAsync(endpoint);
                }

                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
        }

        private async Task ProcessICountClientInfo(int MainCompanyId, int UserID, string cid, string user, string pass)//this name is wrong it gets only supliers
        {
            try
            {
                var ClinetinfoEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);//https://api.icount.co.il/api/v3.php/supplier/get_list
                var endpointClinetinfo = $"{ClinetinfoEndpoint.Endpoint}?cid={cid}&user={user}&pass={pass}";
                HttpMethod methodclientinfo = HttpMethod.Get;
                var ReponsneClientInfo = await SendRequest(endpointClinetinfo, methodclientinfo);

                // Parse the JSON response
                var jsonDocument = JsonDocument.Parse(ReponsneClientInfo);
                var jsonData = jsonDocument.RootElement;
                var bsonDocument = BsonDocument.Parse(jsonData.ToString());
                bsonDocument.Add("internalcompanid", MainCompanyId);
                bsonDocument.Add("UserID", UserID);


                var filterClientSuppliers = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                    Builders<BsonDocument>.Filter.Eq("UserID", UserID)
                );

                var existingDocument = await _IcountClientSuppliers.Find(filterClientSuppliers).FirstOrDefaultAsync();
                if (existingDocument != null)
                {
                    _IcountClientSuppliers.DeleteOne(filterClientSuppliers);
                }
                _IcountClientSuppliers.InsertOne(bsonDocument);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in ProcessICountClientInfo: {ex.Message}");
            }
        }

        private async Task ProcessICountExpenses(int MainCompanyId, int UserID, string cid, string user, string pass)
        {
            var ExpenseSearchObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 77);
            var ExpenseSearchEndpoint = $"{ExpenseSearchObj.Endpoint}?cid={cid}&user={user}&pass={pass}";
            HttpMethod methodexpenseserach = HttpMethod.Get;
            var ReponsneExpenseSearch = await SendRequest(ExpenseSearchEndpoint, methodexpenseserach);

            var jsonDocumentReponsneExpenseSearch = JsonDocument.Parse(ReponsneExpenseSearch);
            var jsonDataExpenseSearch = jsonDocumentReponsneExpenseSearch.RootElement;
            var bsonDocumentExpenseSearch = BsonDocument.Parse(jsonDataExpenseSearch.ToString());
            bsonDocumentExpenseSearch.Add("internalcompanid", MainCompanyId);
            bsonDocumentExpenseSearch.Add("UserID", UserID);

            var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                Builders<BsonDocument>.Filter.Eq("UserID", UserID)
            );

            var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();
            if (ExpenseexistingDocument != null)
            {
                _IcountExpenses.DeleteOne(filterExpenseSearch);
            }
            _IcountExpenses.InsertOne(bsonDocumentExpenseSearch);
        }

        private async Task ProcessICountExpensesTypes(int MainCompanyId, int UserID, string cid, string user, string pass)
        {
            var AllExpensesTypeObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 78);
            var AllExpensesEndpoint = $"{AllExpensesTypeObj.Endpoint}?cid={cid}&user={user}&pass={pass}";
            HttpMethod AllExpensesMethod = HttpMethod.Get;
            var ReponsneAllExpensesTypes = await SendRequest(AllExpensesEndpoint, AllExpensesMethod);

            var ReponsneExpenseTypesjsonDocument = JsonDocument.Parse(ReponsneAllExpensesTypes);
            var jsonDataExpensetypes = ReponsneExpenseTypesjsonDocument.RootElement;
            var bsonDocumentExpensetypes = BsonDocument.Parse(jsonDataExpensetypes.ToString());
            bsonDocumentExpensetypes.Add("internalcompanid", MainCompanyId);
            bsonDocumentExpensetypes.Add("UserID", UserID);

            var filterExpensetypes = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                Builders<BsonDocument>.Filter.Eq("UserID", UserID)
            );

            var ExpensetypesexistingDocument = await _IcountExpensesTypes.Find(filterExpensetypes).FirstOrDefaultAsync();
            if (ExpensetypesexistingDocument != null)
            {
                _IcountExpensesTypes.DeleteOne(filterExpensetypes);
            }
            _IcountExpensesTypes.InsertOne(bsonDocumentExpensetypes);
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

        private async Task<string> GetUserFullName(int userId)
        {
            // Fetch the business entity where the AdminUserid matches the userId
            var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);

            // Ensure that the business entity exists
            if (business == null)
            {
                throw new Exception($"No business found for user ID {userId}.");
            }

            // Combine first and last names from the business entity
            return $"{business.FirstName} {business.LastName}";
        }


        // Helper function to check if the token is expired
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



        // Helper function to fetch and update the token
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
                var responseCompanyInfo = await SendRequest(tokenCompanyInfoEndpoint.Endpoint, HttpMethod.Post, jsonPayload);

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
       


        //public async Task<DigitalDocumentToApproveObj> GetDigitalDocumentToApproveListByUser(int UserID, string Typelist, int? subCompanyId, int pageNumber, int pageSize)
        //{
        //    try
        //    {
        //        // List<DigitalDocumentToApprove> List_DigitalDocumentToApprove = new List<DigitalDocumentToApprove>();

        //        string FullName = "";
        //        List<MainSubCopmaniesMasters> ResListOfCompaniesRelatedToLogedinUser = null;
        //        DigitalDocumentToApproveObj resObj = new DigitalDocumentToApproveObj();
        //        resObj.listDigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
        //        var CheckUsermasterExist = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == UserID);///if user id exist in column userid in table MainSubCopmaniesMasters than he is a master
        //        int MainCompanyId = 0;
        //        if (CheckUsermasterExist != null)//the user is master
        //        {
        //            var MainCompanyIdObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);
        //            MainCompanyId = MainCompanyIdObj.BusinessId;
        //            ResListOfCompaniesRelatedToLogedinUser = await _repository.GetListOfObjectsAsync<MainSubCopmaniesMasters>(x => x.MainCompanyId == MainCompanyId);
        //        }
        //        else
        //        {
        //            var Companyid = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);
        //            var subCompanyIdObj = await _repository.GetFirstObjectAsync<UsersExternalSystemDynamicFields>(x => x.Userid == UserID && x.Companyid == Companyid.BusinessId);
        //            var MainCompanyIdObj = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == UserID && x.SubCompanyId == subCompanyIdObj.SubCompayId);
        //            if (MainCompanyIdObj != null)
        //            {
        //                MainCompanyId = MainCompanyIdObj.CompanyId;
        //            }
        //            else
        //            {
        //                var MainCompanyIdObj1 = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);
        //                MainCompanyId = MainCompanyIdObj1.BusinessId;
        //            }




        //            ResListOfCompaniesRelatedToLogedinUser = GetMainSubCompanies(UserID, MainCompanyId, subCompanyIdObj.SubCompayId);
        //        }



        //        if (subCompanyId == null)
        //        {

        //            if (ResListOfCompaniesRelatedToLogedinUser.Count > 1)
        //            {


        //                var subCompanyIdList = await _repository.GetAllAsync<MainSubCopmaniesMasters>();
        //                subCompanyId = subCompanyIdList
        //                   .Where(x => x.MainCompanyId == MainCompanyId)
        //                   .OrderByDescending(x => x.LastTimeDataShowed)
        //                   .Select(x => x.SubCopmanyId)
        //                   .FirstOrDefault();
        //            }
        //            else
        //            {
        //                subCompanyId = ResListOfCompaniesRelatedToLogedinUser[0].SubCopmanyId;
        //            }



        //            if (subCompanyId != default)
        //            {

        //                var SubCopmaniesMastersRow = await _repository.GetFirstObjectAsync<MainSubCopmaniesMasters>(x => x.SubCopmanyId == subCompanyId && x.MainCompanyId == MainCompanyId);
        //                SubCopmaniesMastersRow.LastTimeDataShowed = DateTime.Now; // Current DateTime

        //                await _repository.UpdateAsync(SubCopmaniesMastersRow);
        //            }
        //            else
        //            {
        //                // Handle case where subCompanyId is not found
        //            }

        //        }
        //        else
        //        {
        //            var SubCopmaniesMastersRow = await _repository.GetFirstObjectAsync<MainSubCopmaniesMasters>(x => x.SubCopmanyId == subCompanyId && x.MainCompanyId == MainCompanyId);
        //            SubCopmaniesMastersRow.LastTimeDataShowed = DateTime.Now; // Current DateTime

        //            await _repository.UpdateAsync(SubCopmaniesMastersRow);

        //        }




        //        //loop on the list of Companies for each company attached to user we need to extract her vat_id from IcountCompanisInfo 
        //        //foreach (var Company in ResListOfCompaniesRelatedToLogedinUser)
        //        //{

        //        var filter = Builders<BsonDocument>.Filter.And(
        //            Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", MainCompanyId),
        //            Builders<BsonDocument>.Filter.Eq("company_info.SubCompanyId", subCompanyId)
        //        );

        //        var projection = Builders<BsonDocument>.Projection.Include("company_info.vat_id").Exclude("_id");

        //        var result = _IcountCompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();

        //        int totalCount = 0;
        //        if (result != null)
        //        {
        //            var vatId = result["company_info"]["vat_id"].AsString;



        //            //now we go to BusinessData  table that has all digitaldocument sent to clients and check if client is there by his vat_id
        //            //if its found we need to extract JsonDocumentid  and BusinessId (as the company that sent the document)
        //            //and return a list of them to the client to show this waitingto approve list to insert as expenses
        //            // Replace with your desired VAT ID

        //            List<BusinessData> ResListOfClientCompaniesThatWasSentDigitalDocument = null;




        //            switch (Typelist)
        //            {
        //                case "notApproveOrRejected":
        //                    //var totalCountObj = await _repository.GetListOfObjectsAsync<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == null);
        //                    //totalCount= totalCountObj.Count();
        //                    ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
        //                        UserID, Typelist, subCompanyId, vatId, pageNumber, pageSize, null);
        //                    break;
        //                case "Rejected":
        //                    //var totalCountObj1 = await _repository.GetListOfObjectsAsync<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == false);
        //                    //totalCount = totalCountObj1.Count();
        //                    ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
        //                        UserID, Typelist, subCompanyId, vatId, pageNumber, pageSize, false);
        //                    break;
        //                case "Approved":
        //                    //var totalCountObj2 = await _repository.GetListOfObjectsAsync<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == true);
        //                    //totalCount = totalCountObj2.Count();
        //                    ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
        //                        UserID, Typelist, subCompanyId, vatId, pageNumber, pageSize, true);
        //                    break;
        //                default:
        //                    // Handle invalid Typelist value
        //                    break;
        //            }
        //            //  string jsonResult = System.Text.Json.JsonSerializer.Serialize(ResListOfClientCompaniesThatWasSentDigitalDocument);




        //            foreach (var DigitalClientRow in ResListOfClientCompaniesThatWasSentDigitalDocument)
        //            {

        //                if (DigitalClientRow.DataSourceType == 1)
        //                {
        //                    string JsonDocUrl = "";
        //                    var DocInfofilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(DigitalClientRow.JsonDocumentid));
        //                    var DocInfoprojection = Builders<BsonDocument>.Projection.Include("doc_info.doc_url_copy").Exclude("_id");
        //                    var DocInforesult = _ICountDocInfoCollection.Find(DocInfofilter).Project(DocInfoprojection).FirstOrDefault();
        //                    if (DocInforesult != null)
        //                    {
        //                        JsonDocUrl = DocInforesult["doc_info"]["doc_url_copy"].AsString;
        //                    }
        //                    resObj.listDigitalDocumentToApprove.Add(new DigitalDocumentToApprove() { JsonDocumentid = DigitalClientRow.JsonDocumentid, ClientVat_id = Convert.ToInt32(DigitalClientRow.ClientVat_id), SendingDigitalDocumentBusinessID = DigitalClientRow.BusinessId, BusinessVatId = DigitalClientRow.BusinessVatId, DocInfoUrl = JsonDocUrl, supplier_name_Sender = DigitalClientRow.supplier_name_Sender, docDate = DigitalClientRow.docDate, amountAV = DigitalClientRow.amountAV, currency_code = DigitalClientRow.currency_code });
        //                }
        //                if (DigitalClientRow.DataSourceType == 2)//webhook data collection
        //                {
        //                    string JsonDocUrl = "";
        //                    var DocInfofilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(DigitalClientRow.JsonDocumentid));
        //                    var DocInfoprojection = Builders<BsonDocument>.Projection.Include("doc_info.doc_url_copy").Exclude("_id");
        //                    var DocInforesult = _IcountWebhookData.Find(DocInfofilter).Project(DocInfoprojection).FirstOrDefault();
        //                    if (DocInforesult != null)
        //                    {
        //                        JsonDocUrl = DocInforesult["doc_info"]["doc_url_copy"].AsString;
        //                    }

        //                    var filtercompany = Builders<BsonDocument>.Filter.And(
        //                         Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", DigitalClientRow.BusinessId),
        //                         Builders<BsonDocument>.Filter.Eq("company_info.SubCompanyId", DigitalClientRow.SubCompanyId)
        //                        );
        //                    var projectioncompany = Builders<BsonDocument>.Projection.Include("company_info.businessName").Exclude("_id");

        //                    var resultcompany = _IcountCompaniesInfoCollection.Find(filtercompany).Project(projectioncompany).FirstOrDefault();
        //                    string supplierNameSender = "";
        //                    if (resultcompany != null)
        //                    {
        //                        supplierNameSender = resultcompany["company_info"]["businessName"].AsString;
        //                    }
        //                    resObj.listDigitalDocumentToApprove.Add(new DigitalDocumentToApprove() { JsonDocumentid = DigitalClientRow.JsonDocumentid, ClientVat_id = Convert.ToInt32(DigitalClientRow.ClientVat_id), SendingDigitalDocumentBusinessID = DigitalClientRow.BusinessId, BusinessVatId = DigitalClientRow.BusinessVatId, DocInfoUrl = JsonDocUrl, supplier_name_Sender = supplierNameSender, docDate = DigitalClientRow.docDate, amountAV = DigitalClientRow.amountAV, currency_code = DigitalClientRow.currency_code });
        //                }
        //            }
        //        }

        //        //}

        //        var UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == MainCompanyId && x.Userid == UserID && x.SubCompayId == subCompanyId);

        //        string cidvalue = null;
        //        string uservalue = null;
        //        string passvalue = null;

        //        //from here 
        //        foreach (var dynamicField in UserexternalSystemDynamicFieldslist)
        //        {
        //            string fieldLabelName = dynamicField.FieldLabelName;
        //            string fieldLabelValue = dynamicField.FieldLabelValue;

        //            if (fieldLabelName == "cid")
        //            {
        //                cidvalue = fieldLabelValue;
        //                // Use the cid value as needed
        //            }
        //            else if (fieldLabelName == "user")
        //            {
        //                uservalue = fieldLabelValue;
        //                // Use the user value as needed
        //            }
        //            else if (fieldLabelName == "pass")
        //            {
        //                passvalue = fieldLabelValue;
        //                // Use the pass value as needed
        //            }
        //        }
        //        var ClinetinfoEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);  ////api.icount.co.il/api/v3.php/supplier/get_list
        //        var endpointClinetinfo = ClinetinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //        HttpMethod methodclientinfo = HttpMethod.Get;
        //        var ReponsneClientInfo = await SendRequest(endpointClinetinfo, methodclientinfo);

        //        // Parse the JSON response
        //        var jsonDocument = JsonDocument.Parse(ReponsneClientInfo);
        //        var jsonData = jsonDocument.RootElement;



        //        var bsonDocument = BsonDocument.Parse(jsonData.ToString());
        //        // Add "internalcompanid" and "UserID" properties
        //        bsonDocument.Add("internalcompanid", MainCompanyId);
        //        bsonDocument.Add("UserID", UserID);


        //        // Define the query to find and delete the existing document
        //        var filterClientSuppliers = Builders<BsonDocument>.Filter.And(
        //            Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
        //            Builders<BsonDocument>.Filter.Eq("UserID", UserID)
        //        );

        //        // Check if a document with the specified internalcompanid and UserID exists
        //        var existingDocument = await _IcountClientSuppliers.Find(filterClientSuppliers).FirstOrDefaultAsync();

        //        if (existingDocument != null)
        //        {
        //            // If the document exists, delete it
        //            _IcountClientSuppliers.DeleteOne(filterClientSuppliers);
        //            _IcountClientSuppliers.InsertOne(bsonDocument);
        //        }
        //        else
        //        {
        //            _IcountClientSuppliers.InsertOne(bsonDocument);
        //        }




        //        //added logic eyal to add json to expensesmongodb
        //        var ExpenseSearchObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 77);//api.icount.co.il/api/v3.php/expense/search
        //        var ExpenseSearchEndpoint = ExpenseSearchObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue; //+ "&supplier_id=" + supplierId.ToString();
        //        HttpMethod methodexpenseserach = HttpMethod.Get;
        //        var ReponsneExpenseSearch = await SendRequest(ExpenseSearchEndpoint, methodexpenseserach);
        //        var jsonDocumentReponsneExpenseSearch = JsonDocument.Parse(ReponsneExpenseSearch);
        //        var jsonDataExpenseSearch = jsonDocumentReponsneExpenseSearch.RootElement;
        //        var bsonDocumentExpenseSearch = BsonDocument.Parse(jsonDataExpenseSearch.ToString());
        //        // Add "internalcompanid" and "UserID" properties
        //        bsonDocumentExpenseSearch.Add("internalcompanid", MainCompanyId);
        //        bsonDocumentExpenseSearch.Add("UserID", UserID);


        //        // Define the query to find and delete the existing document
        //        var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
        //            Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
        //            Builders<BsonDocument>.Filter.Eq("UserID", UserID)
        //        );

        //        var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();
        //        if (ExpenseexistingDocument != null)
        //        {
        //            // If the document exists, delete it
        //            _IcountExpenses.DeleteOne(filterExpenseSearch);
        //            _IcountExpenses.InsertOne(bsonDocumentExpenseSearch);
        //        }
        //        else
        //        {
        //            _IcountExpenses.InsertOne(bsonDocumentExpenseSearch);
        //        }

        //        /////end added logic eyal to add json to expensesmongodb



        //        //start add logic add json to _IcountExpensesTypes
        //        var AllExpensesTypeObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 78);//api.icount.co.il/api/v3.php/expense/types
        //        var AllExpensesEndpoint = AllExpensesTypeObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
        //        HttpMethod AllExpensesMethod = HttpMethod.Get;
        //        var ReponsneAllExpensesTypes = await SendRequest(AllExpensesEndpoint, AllExpensesMethod);
        //        var ReponsneExpenseTypesjsonDocument = JsonDocument.Parse(ReponsneAllExpensesTypes);
        //        var jsonDataExpensetypes = ReponsneExpenseTypesjsonDocument.RootElement;
        //        var bsonDocumentExpensetypes = BsonDocument.Parse(jsonDataExpensetypes.ToString());
        //        bsonDocumentExpensetypes.Add("internalcompanid", MainCompanyId);
        //        bsonDocumentExpensetypes.Add("UserID", UserID);

        //        // Define the query to find and delete the existing document
        //        var filterExpensetypes = Builders<BsonDocument>.Filter.And(
        //            Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
        //            Builders<BsonDocument>.Filter.Eq("UserID", UserID)
        //        );

        //        var ExpensetypesexistingDocument = await _IcountExpensesTypes.Find(filterExpensetypes).FirstOrDefaultAsync();
        //        if (ExpensetypesexistingDocument != null)
        //        {
        //            // If the document exists, delete it
        //            _IcountExpensesTypes.DeleteOne(filterExpensetypes);
        //            _IcountExpensesTypes.InsertOne(bsonDocumentExpensetypes);
        //        }
        //        else
        //        {
        //            _IcountExpensesTypes.InsertOne(bsonDocumentExpensetypes);
        //        }
        //        //end add logic add json to _IcountExpensesTypes





        //        var firstnameObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);
        //        var lastnameObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);

        //        FullName = firstnameObj.FirstName + " " + lastnameObj.LastName;
        //        //}
        //        ///eyal add logic to show alert window with biling details first time or billing wlert window when excceeds the number of files

        //        //var FirstTimeConsoleIndicationObj = await _repository.GetFirstObjectAsync<FirstTimeConsoleIndication>(x => x.Userid == UserID && x.Mainorganization == MainCompanyId && x.Subcompanyid == subCompanyId);

        //        //if (FirstTimeConsoleIndicationObj != null) //the user already visited the page 
        //        //{
        //        //    if (FirstTimeConsoleIndicationObj.FirsttimeOnConsoleForEntity == true)
        //        //    {
        //        //        FirstTimeConsoleIndicationObj.FirsttimeOnConsoleForEntity = false;
        //        //        await _repository.UpdateAsync(FirstTimeConsoleIndicationObj);
        //        //    }


        //        //}
        //        //else
        //        //{//he user first time on the console  page 
        //        //    var objrowFirstTimeConsoleIndication = new FirstTimeConsoleIndication
        //        //    {
        //        //        Userid = UserID,
        //        //        Mainorganization = MainCompanyId,
        //        //        Subcompanyid = subCompanyId,
        //        //        FirsttimeOnConsoleForEntity = true
        //        //    };
        //        //    await _repository.CreateAsync(objrowFirstTimeConsoleIndication);
        //        //    FirstTimeConsoleIndicationObj = await _repository.GetFirstObjectAsync<FirstTimeConsoleIndication>(x => x.Userid == UserID && x.Mainorganization == MainCompanyId && x.Subcompanyid == subCompanyId);
        //        //}
        //        //var UserCreditCardHolderObj = await _repository.GetFirstObjectAsync<UserCreditCardHolder>(x => x.UserId == UserID && x.BusinessId == MainCompanyId && x.SubCompanyId == subCompanyId && x.ExternalSystemId == UserexternalSystemDynamicFieldslist[0].ExternalSystemId);


        //        var BusinessVatId = await _repository.GetFirstObjectAsync<BusinessData>(x => x.UserId == UserID && x.BusinessId == MainCompanyId && x.SubCompanyId == subCompanyId);
        //        IEnumerable<BusinessData> totalCountObj = Enumerable.Empty<BusinessData>();

        //        if (BusinessVatId != null)
        //        {
        //            var Clientvatid = await _repository.GetFirstObjectAsync<BusinessData>(x => x.BusinessVatId == BusinessVatId.BusinessVatId);
        //            if (Clientvatid != null)
        //            {
        //                totalCountObj = await _repository.GetListOfObjectsAsync<BusinessData>(x => x.ClientVat_id == Clientvatid.ClientVat_id);
        //            }
        //        }

        //        var Objres = new DigitalDocumentToApproveObj
        //        {
        //            listDigitalDocumentToApprove = resObj.listDigitalDocumentToApprove,
        //            //TotallistDigitalDocumentToApprove = totalCount,
        //            ListOfSubCompaniesandNames = await PopulateListOfSubCompaniesandNames(ResListOfCompaniesRelatedToLogedinUser, UserID, UserexternalSystemDynamicFieldslist[0].ExternalSystemId),
        //            fullname = FullName

        //        };

        //        return Objres;

        //    }
        //    catch (Exception ex) { return null; }
        //}


        public SupplierItem GetSupplierItemByVatId(List<SupplierItem> supplierList, int vatId)
        {
            return supplierList.FirstOrDefault(supplier => supplier.vat_id == vatId);
        }

        public async Task<RejectDocumenResponse> RejectDocument(RequestRejectDocument requestRejectDocument)
        {
            try
            {
                var businessDatarow = await _repository.GetFirstObjectAsync<BusinessData>(x => x.BusinessVatId == requestRejectDocument.BusinessVatId && x.ClientVat_id == requestRejectDocument.ClientVat_id && x.JsonDocumentid == requestRejectDocument.Jsondocumentid);
                if (businessDatarow != null)
                {
                    businessDatarow.DocumentApprovedtoUninet = false;
                    //businessDatarow.ExpenseTypeId = requestRejectDocument.expense_type_id;
                    await _repository.UpdateAsync(businessDatarow);
                    var res = new RejectDocumenResponse
                    {
                        Success = true,
                        textResponse = requestRejectDocument.Lang == 1 ? "Document rejected Successfully" : "המסמך נדחה בהצלחה"
                    };
                    return res;

                }
                else
                {
                    var res = new RejectDocumenResponse
                    {
                        Success = true,
                        textResponse = requestRejectDocument.Lang == 1 ? "An error occurred, the document was not rejected. " : "אירעה שגיאה המסמך לא נדחה "
                    };
                    return res;
                }



            }
            catch (Exception ex)
            {
                var res = new RejectDocumenResponse
                {
                    Success = false,
                    textResponse = requestRejectDocument.Lang == 1 ? "An error occurred, the document was not rejected. " : "אירעה שגיאה המסמך לא נדחה "
                };
                return res;
            }
        }

        //public bool IsVatIdExists(List<SupplierItem> supplierList, int vatId)
        //{
        //    return supplierList.Any(supplier => supplier.vat_id == vatId);
        //}

        //public async Task<bool> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest insertUserDigitalDocRequest, int userId)
        //{
        //    try
        //    {

        //        var UserexternalSystemDynamicFieldslist = _repository.GetListOfObjects<UsersExternalSystemDynamicFields>(x => x.Companyid == insertUserDigitalDocRequest.internalCompanyId && x.Userid == userId);

        //        string cidvalue = null;
        //        string uservalue = null;
        //        string passvalue = null;

        //        //from here 
        //        foreach (var dynamicField in UserexternalSystemDynamicFieldslist)
        //        {
        //            string fieldLabelName = dynamicField.FieldLabelName;
        //            string fieldLabelValue = dynamicField.FieldLabelValue;

        //            if (fieldLabelName == "cid")
        //            {
        //                cidvalue = fieldLabelValue;
        //                // Use the cid value as needed
        //            }
        //            else if (fieldLabelName == "user")
        //            {
        //                uservalue = fieldLabelValue;
        //                // Use the user value as needed
        //            }
        //            else if (fieldLabelName == "pass")
        //            {
        //                passvalue = fieldLabelValue;
        //                // Use the pass value as needed
        //            }
        //        }
        //        var ExpenseCreateEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 79);
        //        var endpointExpenseCreate = ExpenseCreateEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;



        //        //supplier_id
        //        //expense_type_id
        //        //expense_doctype
        //        //expense_docnum
        //        //expense_sum

        //        //https://api.icount.co.il/api/v3.php/expense/create





        //        return true;
        //    }
        //    catch (Exception ex) { return false; }
        //}

        public async Task<AddGenericexpenseTypeResponse> AddexpenseType(AddexpenseTypeRequest addexpenseTypeRequest, int userId)
        {
            try
            {

                //get ClientVat_id 540270165 from table BusinessData by sending JsonDocumentid 660184ea1e021dddd7445485
                // var ClientVat_id = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == insertUserDigitalDocRequest.Jsondocumentid).ClientVat_id;


                //now get SubCompanyId from  _IcountCompaniesInfoCollection
                var FilterClientvatidCompanyInfo = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", addexpenseTypeRequest.tax_id.ToString());

                var companyClientRow = await _IcountCompaniesInfoCollection.Find(FilterClientvatidCompanyInfo).FirstOrDefaultAsync();
                int SubCompanyId = 0;
                int InternalCompanyId = 0;

                if (companyClientRow != null)
                {
                    var companyInfo = companyClientRow["company_info"].AsBsonDocument;
                    if (companyInfo.Contains("SubCompanyId"))
                    {
                        var subCompanyId = companyInfo["SubCompanyId"].AsInt32;
                        SubCompanyId = subCompanyId;

                    }
                    if (companyInfo.Contains("InternalCompanyId"))
                    {
                        var internalCompanyId = companyInfo["InternalCompanyId"].AsInt32;
                        InternalCompanyId = internalCompanyId;

                    }



                }



                List<UsersExternalSystemDynamicFields> UserexternalSystemDynamicFieldslist = null;
                var CheckUsermasterExist = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == userId);///if user id exist in column userid in table MainSubCopmaniesMasters than he is a master
                if (CheckUsermasterExist != null)
                {
                    UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == InternalCompanyId && x.Userid == userId && x.SubCompayId == SubCompanyId);
                }
                else
                {
                    var GetRelatedMasterId = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == userId);
                    if (GetRelatedMasterId!=null)
                    {
                        UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == GetRelatedMasterId.CompanyId && x.Userid == GetRelatedMasterId.Userid && x.SubCompayId == SubCompanyId);
                        userId = GetRelatedMasterId.Userid;
                    }
                    else
                    {
                        UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == InternalCompanyId && x.Userid == userId && x.SubCompayId == SubCompanyId);
                    }
                    
                }

                //var InternalCompanyId = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);


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
                var AddexpenseTypeEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 80);
                string EndpointAddexpenseType = AddexpenseTypeEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                HttpMethod method = HttpMethod.Post; // Change to HttpMethod.Get for a GET request






                // Set your POST data if needed
                //string postData = "{\"vat_to_expense\": " + addexpenseTypeRequest.vat_to_expense + ", \"expense_type_name\": " + addexpenseTypeRequest.expense_type_name + ", \"deductable_vat\": \"" + addexpenseTypeRequest.deductable_vat + "\", \"deductable_expense\": \"" + addexpenseTypeRequest.deductable_expense + "}";
                //string postData = Newtonsoft.Json.JsonConvert.SerializeObject(addexpenseTypeRequest);

                string postData = "{"
                 + "\"vat_to_expense\": " + addexpenseTypeRequest.vat_to_expense.ToString().ToLower() + ", "
                 + "\"expense_type_name\": \"" + addexpenseTypeRequest.expense_type_name.Replace("\"", "\\\"") + "\", "
                 + "\"deductable_vat\": " + addexpenseTypeRequest.deductable_vat + ", "
                 + "\"deductable_expense\": " + addexpenseTypeRequest.deductable_expense + ", "
                 + "\"permanent_property\": " + addexpenseTypeRequest.permanent_property.ToString().ToLower() + ", "
                 + "\"no_vat\": " + addexpenseTypeRequest.no_vat.ToString().ToLower()
                 + "}";


                string result = await SendRequest(EndpointAddexpenseType, method, postData);
                AddexpenseTypeResponse response = JsonConvert.DeserializeObject<AddexpenseTypeResponse>(result);

                // Handle the result as needed
                List<ExpenseType> res = new List<ExpenseType>();

                if (response.status)
                {
                    // When returning from creating the new expense type, we need to call icount again to retrieve the new list of expense types
                    res = await CreateExpenseCategorylistIcount(userId, addexpenseTypeRequest.supplier_ID, addexpenseTypeRequest.tax_id.ToString(), cidvalue, uservalue, passvalue);

                    var res2 = new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1 ? "The expense type was added successfully" : "סוג הוצאה התווסף בהצלחה",
                        ExpenseTypeList = res
                    };
                    return res2;
                }
                else
                {
                    // Extract the error detail for Hebrew response
                    string errorDetail = response.error_details != null && response.error_details.Length > 0
                        ? response.error_details[0]
                        : "נכשל ביצירת הוצאה"; // Fallback message if error_details is empty or null

                    var res2 = new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1 ? errorDetail : errorDetail,
                        ExpenseTypeList = res
                    };
                    return res2;
                }


            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public async Task<createExpenseApiResponse> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest insertUserDigitalDocRequest, int userId)
        {
            try
            {
                List<UsersExternalSystemDynamicFields> UserexternalSystemDynamicFieldslist = null;

                //get ClientVat_id 540270165 from table BusinessData by sending JsonDocumentid 660184ea1e021dddd7445485
                var businessData = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == insertUserDigitalDocRequest.Jsondocumentid);
                var ClientVat_id = businessData?.ClientVat_id;

                //now get SubCompanyId from  _IcountCompaniesInfoCollection
                var FilterClientvatidCompanyInfo = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", ClientVat_id.ToString());

                var companyClientRow = await _IcountCompaniesInfoCollection.Find(FilterClientvatidCompanyInfo).FirstOrDefaultAsync();
                int SubCompanyId = 0;


                if (companyClientRow != null)
                {
                    var companyInfo = companyClientRow["company_info"].AsBsonDocument;
                    if (companyInfo.Contains("SubCompanyId"))
                    {
                        var subCompanyId = companyInfo["SubCompanyId"].AsInt32;
                        SubCompanyId = subCompanyId;

                    }
                }


                var CheckUsermasterExist = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == userId);///if user id exist in column userid in table MainSubCopmaniesMasters than he is a master






                if (CheckUsermasterExist != null) // user is a master
                {
                    UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                        x => x.Companyid == insertUserDigitalDocRequest.internalCompanyId &&
                             x.Userid == userId &&
                             x.SubCompayId == SubCompanyId
                    );
                }
                else
                {
                    var GetRelatedMasterId = await _repository.GetFirstObjectAsync<SubUserCredentials>(
                        x => x.SubUserId == userId
                    );

                    if (GetRelatedMasterId != null)
                    {
                        UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                            x => x.Companyid == GetRelatedMasterId.CompanyId &&
                                 x.Userid == GetRelatedMasterId.Userid &&
                                 x.SubCompayId == SubCompanyId
                        );
                        userId = GetRelatedMasterId.Userid;
                    }
                    else
                    {
                        UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                            x => x.Companyid == insertUserDigitalDocRequest.internalCompanyId &&
                                 x.Userid == userId &&
                                 x.SubCompayId == SubCompanyId
                        );
                    }
                }




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
                var ExpenseCreateEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 79);//https://api.icount.co.il/api/v3.php/expense/create
                string endpointExpenseCreate = ExpenseCreateEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                HttpMethod method = HttpMethod.Post; // Change to HttpMethod.Get for a GET request

                /////need to add logic search supplier in the 
                /////
                ////we get all list of supliers for the user loged into uninet and get his suplierid and supliername
                //var resSUpplierLIst = await GetClientSupplierList(cidvalue, uservalue, passvalue);
                ////now we should loop on the resSUpplierLIst
                ////and find if the vatId exist in the suplier list
                //var BusinessVatId = await _repository.GetFirstObjectAsync<BusinessData>(x => x.BusinessId == insertUserDigitalDocRequest.internalCompanyId);
                //var ItemFound = GetSupplierItemByVatId(resSUpplierLIst, Convert.ToInt32(BusinessVatId));
                //receiptDate   "2024-01-17"
                string dateissued = "";
                string doc_url_copy = "";
                bool payed = true;
                string postData = "";
                string result = "";
                string result_endpointExpenseCreate = "";
                var filter = Builders<BsonDocument>.Filter.Eq("docnum", insertUserDigitalDocRequest.expense_docnum);
                var webhookdoc = _IcountWebhookData.Find(filter).FirstOrDefault();

                if (webhookdoc != null)
                {

                    dateissued = webhookdoc["doc_info"]["dateissued"].AsString;
                    doc_url_copy = webhookdoc["doc_info"]["doc_url_copy"].AsString;
                }

                if (insertUserDigitalDocRequest.expense_doctype == "invrec" || insertUserDigitalDocRequest.expense_doctype == "receipt")
                {



                    //postData = "{\"supplier_id\": " + insertUserDigitalDocRequest.supplier_id + ", \"expense_type_id\": " + insertUserDigitalDocRequest.expense_type_id + ", \"expense_doctype\": \"" + insertUserDigitalDocRequest.expense_doctype + "\", \"expense_docnum\": \"" + insertUserDigitalDocRequest.expense_docnum + "\", \"internalCompanyId\": " + insertUserDigitalDocRequest.internalCompanyId + ", \"expense_sum\": " + insertUserDigitalDocRequest.expense_sum + ",\"expense_paid\":" + payed.ToString().ToLower() + ",\"expense_paid_date\":\"" + dateissued + "\"}";
                    // Prepare JSON data part
                    using (var httpClient = new HttpClient())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            // Add each field as a separate part
                            content.Add(new StringContent(insertUserDigitalDocRequest.supplier_id.ToString()), "supplier_id");
                            content.Add(new StringContent(insertUserDigitalDocRequest.expense_type_id.ToString()), "expense_type_id");
                            content.Add(new StringContent(insertUserDigitalDocRequest.expense_doctype), "expense_doctype");
                            content.Add(new StringContent(insertUserDigitalDocRequest.expense_docnum.ToString()), "expense_docnum");
                            content.Add(new StringContent(insertUserDigitalDocRequest.expense_sum.ToString()), "expense_sum");
                            // Add 'expense_paid' and 'expense_paid_date' only if necessary
                            content.Add(new StringContent(payed.ToString().ToLower()), "expense_paid");
                            content.Add(new StringContent(dateissued), "expense_paid_date");

                            // Download and add PDF file part
                            var pdfResponse = await httpClient.GetAsync(doc_url_copy);
                            if (pdfResponse.IsSuccessStatusCode)
                            {
                                var pdfData = await pdfResponse.Content.ReadAsByteArrayAsync();
                                var pdfContent = new ByteArrayContent(pdfData);
                                pdfContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                                content.Add(pdfContent, "scan", "scan.pdf");
                            }

                            // Send the request
                            var endpointExpenseCreate_Response = await httpClient.PostAsync(endpointExpenseCreate, content);
                            result_endpointExpenseCreate = await endpointExpenseCreate_Response.Content.ReadAsStringAsync();

                            // Process the response

                        }
                    }





                }
                if (insertUserDigitalDocRequest.expense_doctype == "invoice" || insertUserDigitalDocRequest.expense_doctype == "deal" || insertUserDigitalDocRequest.expense_doctype == "order" || insertUserDigitalDocRequest.expense_doctype == "refund" || insertUserDigitalDocRequest.expense_doctype == "delcert")
                {
                    // postData = "{\"supplier_id\": " + insertUserDigitalDocRequest.supplier_id + ", \"expense_type_id\": " + insertUserDigitalDocRequest.expense_type_id + ", \"expense_doctype\": \"" + insertUserDigitalDocRequest.expense_doctype + "\", \"expense_docnum\": \"" + insertUserDigitalDocRequest.expense_docnum + "\", \"internalCompanyId\": " + insertUserDigitalDocRequest.internalCompanyId + ", \"expense_sum\": " + insertUserDigitalDocRequest.expense_sum + "}";
                    //result_endpointExpenseCreate = await SendRequest(endpointExpenseCreate, method, postData);
                    using (var httpClient = new HttpClient())
                    {
                        using (var content = new MultipartFormDataContent())
                        {
                            // Add each field as a separate part
                            content.Add(new StringContent(insertUserDigitalDocRequest.supplier_id.ToString()), "supplier_id");
                            content.Add(new StringContent(insertUserDigitalDocRequest.expense_type_id.ToString()), "expense_type_id");
                            content.Add(new StringContent(insertUserDigitalDocRequest.expense_doctype), "expense_doctype");
                            content.Add(new StringContent(insertUserDigitalDocRequest.expense_docnum.ToString()), "expense_docnum");
                            content.Add(new StringContent(insertUserDigitalDocRequest.expense_sum.ToString()), "expense_sum");
                            // Add 'expense_paid' and 'expense_paid_date' only if necessary
                            //content.Add(new StringContent(payed.ToString().ToLower()), "expense_paid");
                            //content.Add(new StringContent(dateissued), "expense_paid_date");

                            // Download and add PDF file part
                            var pdfResponse = await httpClient.GetAsync(doc_url_copy);
                            if (pdfResponse.IsSuccessStatusCode)
                            {
                                var pdfData = await pdfResponse.Content.ReadAsByteArrayAsync();
                                var pdfContent = new ByteArrayContent(pdfData);
                                pdfContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                                content.Add(pdfContent, "scan", "scan.pdf");
                            }

                            // Send the request
                            var endpointExpenseCreate_Response = await httpClient.PostAsync(endpointExpenseCreate, content);
                            result_endpointExpenseCreate = await endpointExpenseCreate_Response.Content.ReadAsStringAsync();

                            // Process the response

                        }
                    }



                }

                // Set your POST data if needed




                createExpenseApiResponse response = JsonConvert.DeserializeObject<createExpenseApiResponse>(result_endpointExpenseCreate);
                // Handle the result as needed
                if (response.status)
                {
                    var businessDatarow = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == insertUserDigitalDocRequest.Jsondocumentid);
                    if (businessDatarow != null)
                    {
                        businessDatarow.DocumentApprovedtoUninet = true;
                        businessDatarow.ExpenseTypeId = insertUserDigitalDocRequest.expense_type_id;
                        await _repository.UpdateAsync(businessDatarow);


                    }
                }


                return response;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        private List<ExpenseType> GetExpenseTypeListWithDefault(string json, int expenseTypeId, bool Permanent)
        {
            var bsonDoc = BsonDocument.Parse(json);
            var expenseTypes = bsonDoc["expense_types"].AsBsonDocument;

            var expenseTypeList = new List<ExpenseType>();

            foreach (var expenseType in expenseTypes.Elements)
            {
                var expenseTypeDoc = expenseType.Value.AsBsonDocument;
                var expenseTypeDesc = expenseTypeDoc["expense_type_name"].AsString;

                // If Permanent is true, add "(P)" to the description
                if (Permanent)
                {
                    expenseTypeDesc += " (P)";
                }

                var newExpenseType = new ExpenseType
                {
                    ExpenseTypeId = expenseTypeDoc["expense_type_id"].AsInt32,
                    ExpenseTypeDesc = expenseTypeDesc,
                    IsDefault = expenseTypeDoc["expense_type_id"].AsInt32 == expenseTypeId
                };

                expenseTypeList.Add(newExpenseType);
            }

            return expenseTypeList;
        }

        private async Task<List<ExpenseType>> PostExpencetypeList(string endpoint, Dictionary<string, string> requestBody, string ExpenseTypeId)
        {
            var requestContent = new FormUrlEncodedContent(requestBody);
            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(endpoint, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            bool isPermanent = requestBody.ContainsKey("permanent_property") && requestBody["permanent_property"] == "true";
            List<ExpenseType> list = GetExpenseTypeListWithDefault(responseContent, Convert.ToInt32(ExpenseTypeId), isPermanent);
            return list;
        }

        private async Task<string> PostAndGetSupplierId(string endpoint, Dictionary<string, string> requestBody)
        {
            var requestContent = new FormUrlEncodedContent(requestBody);

            using var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(endpoint, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Parse the JSON response
            var jsonDocument = JsonDocument.Parse(responseContent);
            if (jsonDocument.RootElement.TryGetProperty("supplier_id", out var supplierIdProperty))
            {
                // Convert the JsonElement to string
                if (supplierIdProperty.ValueKind == JsonValueKind.String)
                {
                    return supplierIdProperty.GetString();
                }
                else if (supplierIdProperty.ValueKind == JsonValueKind.Number)
                {
                    return supplierIdProperty.GetInt32().ToString(); // Or GetInt64().ToString() if large numbers are expected
                }
            }

            return "0"; // Return a default value if supplier_id is not found or cannot be parsed
        }




    }

}
