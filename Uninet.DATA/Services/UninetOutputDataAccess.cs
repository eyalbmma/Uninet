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
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using static System.Net.WebRequestMethods;
using System.ComponentModel.Design;
using System.Reflection.Metadata;
using System.IO;
//Refactor the following method to improve performance


namespace Uninet.DATA.Services
{

    public class GreenInvoiceClassification
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Code { get; set; }
        public int ParentIrsCode { get; set; }
    }
    public class Item
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("irsCode")]
        public int IrsCode { get; set; }

        [JsonPropertyName("income")]
        public decimal Income { get; set; }

        [JsonPropertyName("type")]
        public int Type { get; set; }

        [JsonPropertyName("vat")]
        public decimal Vat { get; set; }
    }





    public class GreenInvoiceSearchResponse
    {
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Pages { get; set; }
        public List<ExpenseItem> Items { get; set; }
        
    }

    public class ExpenseItem
    {
        public string Id { get; set; }
        public int BusinessType { get; set; }
        public int DocumentType { get; set; }
        public int Status { get; set; }
        public int PaymentType { get; set; }
        public string Currency { get; set; }
        public double CurrencyRate { get; set; }
        public double AmountExcludeVat { get; set; }
        public double Vat { get; set; }
        public double Amount { get; set; }
        public string Date { get; set; }
        public string DueDate { get; set; }
        public string Number { get; set; }
        public bool Active { get; set; }
        public string Description { get; set; }
        public string Remarks { get; set; }
        public Supplier Supplier { get; set; }
        public AccountingClassification AccountingClassification { get; set; }
        public long LastUpdateDate { get; set; } // Add this property if it's missing
    }
    public class AccountingClassification
    {
        public string Id { get; set; }
        public string Key { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int IrsCode { get; set; }
        public int Income { get; set; }
        public int Type { get; set; }
        public int Vat { get; set; }
    }


    public class Supplier
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string TaxId { get; set; }
    }

 

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
        private readonly IMongoCollection<BsonDocument> _MorningException;
        
        private readonly IMongoCollection<BsonDocument> _IcountClientSuppliers;
        private readonly IMongoCollection<BsonDocument> _IcountExpenses;
        private readonly IMongoCollection<BsonDocument> _IcountExpensesTypes;
        private readonly IMongoCollection<BsonDocument> _MorningExpensesTypes;
        private readonly IMongoCollection<BsonDocument> _MorningexpenseTypesManual;
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
            _MorningexpenseTypesManual = database.GetCollection<BsonDocument>("MorningexpenseTypesManual");
            _MorningExpenses = database.GetCollection<BsonDocument>("MorningExpenses");
            _Icount_BussinesPartner_Clients_Suplliers = database.GetCollection<BsonDocument>("Icount_BussinesPartner_Clients_Suplliers");
            _MorningException= database.GetCollection<BsonDocument>("MorningException");
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
        private async Task<List<ExpenseType>>GetExpenseTypeListMorning(string endpoint)
        {
            try
            {
                
                string response = await SendRequestWithToken(endpoint, HttpMethod.Get);

                // Parse response into a list of ExpenseType
                var expenseTypeList = JsonConvert.DeserializeObject<List<ExpenseType>>(response);

                

                return expenseTypeList;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in PostExpenseTypeListMorning: {ex.Message}");
                return new List<ExpenseType>();
            }
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
                        expenseType.IsDefault = expenseType.ExpenseTypeId == expenseTypeId.ToString();
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

        private async Task<T> CallGreenInvoiceSearchAPI<T>(string endpoint, object requestPayload, string token)
        {
            using var client = new HttpClient();

            // Add Authorization header
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Send the request
            var response = await client.PostAsJsonAsync(endpoint, requestPayload);

            // Ensure the request was successful
            response.EnsureSuccessStatusCode();

            // Deserialize the response
            return await response.Content.ReadFromJsonAsync<T>();
        }


        private async Task<T> CallGreenInvoiceClassificationsAPI<T>(string endpoint, string token)
        {
            try
            {
                using var client = new HttpClient();

                // Add Authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send the request
                var response = await client.GetAsync(endpoint);

                // Ensure the request was successful
                response.EnsureSuccessStatusCode();

                // Deserialize the response
                return await response.Content.ReadFromJsonAsync<T>();
            }
            catch(Exception ex)
            {
                return default(T); 
            }
        }
        private async Task<List<ExpenseType>> CreateExpenseCategorylistMorning(
     int userId,
     int SubCompanyIdSuplier,
     int SubCompanyid_clientRelated,
     string latestSupplierId,
     string latestSupplierName,
     string supplierId = null,
     string businessVatId = null,
     bool? documentApprovedToUninet = false,
     string? expenseTypeId = "0"
 )
        {
            List<ExpenseType> expenseTypeList = new List<ExpenseType>();

            // Retrieve the business object and get the InternalCompanyId
            var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
            if (business == null)
                throw new Exception("Business not found for the given userId.");

            int internalCompanyId = business.BusinessId;

            // Get a new token
            string newToken = await GetNewToken(internalCompanyId, SubCompanyid_clientRelated, userId, 6);

            // Call the expenses search API
            var expenseEndpointObjSearch = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 46);
            var expenseApiResponse = await CallGreenInvoiceSearchAPI<GreenInvoiceSearchResponse>(
                expenseEndpointObjSearch.Endpoint,
                new
                {
                    supplierId = latestSupplierId,
                    supplierName = latestSupplierName,
                    page = 1,
                    pageSize = 20
                },
                newToken
            );

            string defaultClassificationId = null;

            if (expenseApiResponse?.Items != null && expenseApiResponse.Items.Any())
            {
                var latestExpense = expenseApiResponse.Items
                    .OrderByDescending(e => e.LastUpdateDate)
                    .First();

                // Extract accountingClassification for default
                var accountingClassification = latestExpense.AccountingClassification;
                if (accountingClassification != null)
                {
                    defaultClassificationId = accountingClassification.Id;

                    // Build filter for existing document
                    var existingFilter = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", internalCompanyId),
                        Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyid_clientRelated),
                        Builders<BsonDocument>.Filter.Eq("userId", userId)
                    );

                    var existingDocument = await _MorningExpenses.Find(existingFilter).FirstOrDefaultAsync();

                    // Convert latestExpense to BsonDocument and adjust AccountingClassification
                    var latestExpenseDocument = latestExpense.ToBsonDocument();
                    if (latestExpenseDocument.Contains("AccountingClassification"))
                    {
                        var classification = latestExpenseDocument["AccountingClassification"].AsBsonDocument;
                        if (classification.Contains("_id"))
                        {
                            classification["id"] = classification["_id"]; // Copy `_id` to `id`
                            classification.Remove("_id"); // Remove `_id` field
                        }
                    }

                    if (existingDocument != null)
                    {
                        var existingResultsList = existingDocument["results_list"].AsBsonArray;

                        // Check if an expense with this AccountingClassification.Id already exists
                        var classificationExists = existingResultsList.Any(x =>
                            x["AccountingClassification"]?["id"].AsString == accountingClassification.Id);

                        if (!classificationExists)
                        {
                            // No expense with this classification exists; push the new expense into results_list
                            var update = Builders<BsonDocument>.Update.Push("results_list", latestExpenseDocument);
                            await _MorningExpenses.UpdateOneAsync(existingFilter, update);
                        }
                        else
                        {
                            // At least one expense with this classification exists.
                            // Now check if one exists with the same classification AND the same Supplier._id.
                            string newSupplierId = "";
                            if (latestExpenseDocument.Contains("Supplier"))
                            {
                                var supplierDoc = latestExpenseDocument["Supplier"].AsBsonDocument;
                                if (supplierDoc.Contains("_id"))
                                {
                                    newSupplierId = supplierDoc["_id"].AsString;
                                }
                            }

                            var duplicateWithSupplier = existingResultsList.Any(x =>
                                x["AccountingClassification"]?["id"].AsString == accountingClassification.Id &&
                                x["Supplier"]?["_id"].AsString == newSupplierId);

                            if (!duplicateWithSupplier)
                            {
                                // The classification exists but with a different supplier.
                                // Instead of creating a new document, add a new item to results_list.
                                var update = Builders<BsonDocument>.Update.Push("results_list", latestExpenseDocument);
                                await _MorningExpenses.UpdateOneAsync(existingFilter, update);
                            }
                        }
                    }
                    else
                    {
                        // No existing document found, so create a new one with the results_list
                        var bsonDocument = latestExpense.ToBsonDocument();
                        if (bsonDocument.Contains("AccountingClassification"))
                        {
                            var classification = bsonDocument["AccountingClassification"].AsBsonDocument;
                            if (classification.Contains("_id"))
                            {
                                classification["id"] = classification["_id"]; // Copy `_id` back to `id`
                                classification.Remove("_id"); // Remove `_id` if needed
                            }
                        }

                        var newDocument = new BsonDocument
                {
                    { "internalCompanyId", internalCompanyId },
                    { "Client_SubCompanyid", SubCompanyid_clientRelated },
                    { "userId", userId },
                    { "results_list", new BsonArray { bsonDocument } }
                };

                        await _MorningExpenses.InsertOneAsync(newDocument);
                    }

                    // Add to expenseTypeList as the default classification
                    expenseTypeList.Add(new ExpenseType
                    {
                        ExpenseTypeId = accountingClassification.Id,
                        ExpenseTypeDesc = accountingClassification.Title,
                        IsDefault = true
                    });
                }
            }

            // Call the fallback API to get classifications
            var expenseTypeListEndpointObjSearch = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 94);
            var classificationApiResponse = await CallGreenInvoiceClassificationsAPI<List<GreenInvoiceClassification>>(
                expenseTypeListEndpointObjSearch.Endpoint,
                newToken
            );

            if (classificationApiResponse != null && classificationApiResponse.Any())
            {
                // List of classification names to exclude
                var excludedNames = new HashSet<string>
        {
            "עלות מכירות ושירותים",
            "הוצאות מכירה",
            "הוצאות הנהלה וכלליות",
            "הוצאות מימון",
            "רכוש"
        };

                foreach (var classification in classificationApiResponse)
                {
                    if (excludedNames.Contains(classification.Name))
                        continue;

                    if (!expenseTypeList.Any(et => et.ExpenseTypeId == classification.Id))
                    {
                        expenseTypeList.Add(new ExpenseType
                        {
                            ExpenseTypeId = classification.Id,
                            ExpenseTypeDesc = classification.Name,
                            IsDefault = classification.Id == defaultClassificationId
                        });
                    }
                }

                // Update or insert expense types into _MorningExpensesTypes
                var classificationDocument = new BsonDocument
        {
            { "internalCompanyId", internalCompanyId },
            { "Client_SubCompanyid", SubCompanyid_clientRelated },
            { "userId", userId },
            { "expense_types", new BsonArray(expenseTypeList.Select(et => new BsonDocument
                {
                    { "ExpenseTypeId", et.ExpenseTypeId },
                    { "ExpenseTypeDesc", et.ExpenseTypeDesc },
                    { "IsDefault", et.IsDefault }
                })) }
        };

                var classificationFilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("internalCompanyId", internalCompanyId),
                    Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyid_clientRelated),
                    Builders<BsonDocument>.Filter.Eq("userId", userId)
                );

                await _MorningExpensesTypes.UpdateOneAsync(
                    classificationFilter,
                    Builders<BsonDocument>.Update.Set("expense_types", classificationDocument["expense_types"]),
                    new UpdateOptions { IsUpsert = true });
            }

            return expenseTypeList;
        }


























        //private async Task<List<ExpenseType>> CreateExpenseCategorylistMorning(
        //    int userId,
        //    int SubCompanyIdSuplier,
        //    int SubCompanyid_clientRelated,
        //    string supplierId = null,
        //    string businessVatId = null,
        //    bool? documentApprovedToUninet = false,
        //    int? expenseTypeId = 0

        //)
        //{
        //    List<ExpenseType> expenseTypeList = new List<ExpenseType>();
        //    string supplierIdFromApi = "";

        //    // Retrieve the business object and get the InternalCompanyId
        //    var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
        //    if (business != null)
        //    {
        //        int internalCompanyId = business.BusinessId;

        //        // Get a new token for Morning API
        //        //string token = await GetNewToken(internalCompanyId, Convert.ToInt32(supplierId), userId, 6); // ExternalSystemId for Morning is 6

        //        //var expenseEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 94); // Replace with correct endpoint ID for Morning
        //        //var expenseEndpoint = expenseEndpointObj.Endpoint;
        //        //var expenseTypeListRes = await GetExpenseTypeListMorning(expenseEndpoint);
        //        //if (documentApprovedToUninet == false || documentApprovedToUninet == null)
        //        //{
        //        //    // Find supplier ID based on VAT ID
        //        //    var filter = Builders<BsonDocument>.Filter.And(
        //        //        Builders<BsonDocument>.Filter.Eq("UserID", userId),
        //        //        Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId)
        //        //    );

        //        //    var clientSuppliersItem = await _MorningClientSuppliers.Find(filter).ToListAsync();
        //        //    foreach (var item in clientSuppliersItem)
        //        //    {
        //        //        var suppliers = item["suppliers"].AsBsonDocument;
        //        //        foreach (var supplierKey in suppliers.Names)
        //        //        {
        //        //            var supplierDetails = suppliers[supplierKey].AsBsonDocument;
        //        //            if (supplierDetails["vat_id"].AsString == businessVatId)
        //        //            {
        //        //                supplierIdFromApi = supplierDetails["supplier_id"].AsString;
        //        //                break;
        //        //            }
        //        //        }
        //        //    }

        //        //    // Fetch expense type information using Morning API
        //        //    var expenseEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 94); // Replace with correct endpoint ID for Morning
        //        //    var expenseEndpoint = expenseEndpointObj.Endpoint;

        //        //    // Prepare request for Morning API
        //        //    var requestBody = new Dictionary<string, string>
        //        //    {
        //        //        { "supplier_id", supplierIdFromApi },
        //        //        { "token", token }
        //        //    };

        //        //    // Fetch permanent properties set to true
        //        //    requestBody["permanent_property"] = "true";
        //        //    var expenseTypeListTrue = await PostExpenseTypeListMorning(expenseEndpoint, requestBody, expenseTypeId);

        //        //    // Fetch permanent properties set to false
        //        //    requestBody["permanent_property"] = "false";
        //        //    var expenseTypeListFalse = await PostExpenseTypeListMorning(expenseEndpoint, requestBody, expenseTypeId);

        //        //    // Sort both lists
        //        //    expenseTypeListFalse = expenseTypeListFalse.OrderBy(et => et.ExpenseTypeDesc).ToList();
        //        //    expenseTypeListTrue = expenseTypeListTrue.OrderBy(et => et.ExpenseTypeDesc).ToList();

        //        //    // Merge lists: expenseTypeListFalse first, then expenseTypeListTrue
        //        //    expenseTypeList = expenseTypeListFalse.Concat(expenseTypeListTrue).ToList();
        //        //}

        //        //if (documentApprovedToUninet == true)
        //        //{
        //        //    // Handle case where document is approved to Uninet
        //        //    var expenseEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83); // Replace with correct endpoint ID for Morning
        //        //    var expenseEndpoint = expenseEndpointObj.Endpoint;

        //        //    var requestBody = new Dictionary<string, string>
        //        //    {
        //        //        { "supplier_id", supplierIdFromApi },
        //        //        { "token", token }
        //        //    };

        //        //    expenseTypeList = await PostExpenseTypeListMorning(expenseEndpoint, requestBody, expenseTypeId);
        //        //}
        //    }

        //    return expenseTypeList;
        //}


        //private async Task<List<ExpenseType>> CreateExpenseCategorylistIcount(int icountSubCompanyId, int userId, string supplierId = null, string BusinessVatId = null, string cidvalue = null, string uservalue = null, string passvalue = null, bool? DocumentApprovedtoUninet = false,  string? ExpenseTypeId = "0")
        //{
        //    List<ExpenseType> ExpenseTypeList = new List<ExpenseType>();
        //    string SuplierId = "";
        //    // Retrieve the business object and get the InternalCompanyId
        //    var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
        //    if (business != null)
        //    {
        //        if (DocumentApprovedtoUninet == false || DocumentApprovedtoUninet == null)
        //        {
        //            int internalCompanyId = business.BusinessId; // Assuming InternalCompanyId is a property of Businesses


        //            //eyal remared this code 10022025
        //            //because i get the sunplerid in the body request why am i extracting it from mongodb collection _IcountClientSuppliers
        //            // Create a filter to match the internalcompanid and UserID
        //            //var filter = Builders<BsonDocument>.Filter.Eq("UserID", userId) &
        //            //             Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId) & 
        //            //             Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", icountSubCompanyId);


        //            //var ClientSuppliersItem = await _IcountClientSuppliers.Find(filter).ToListAsync();
        //            //foreach (var item in ClientSuppliersItem)
        //            //{
        //            //    // Assuming suppliers are stored as a BsonDocument
        //            //    BsonDocument suppliers = item["suppliers"].AsBsonDocument;

        //            //    foreach (var supplierKey in suppliers.Names)
        //            //    {
        //            //        var supplierDetails = suppliers[supplierKey].AsBsonDocument;
        //            //        if (supplierDetails["vat_id"].AsString == BusinessVatId)
        //            //        {
        //            //            // Return supplier_id if vat_id matches
        //            //            SuplierId = supplierDetails["supplier_id"].AsString;
        //            //            break;
        //            //        }
        //            //    }
        //            //}


        //            var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
        //                    Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId),
        //                    Builders<BsonDocument>.Filter.Eq("UserID", userId),
        //                    Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", icountSubCompanyId)

        //                );

        //            var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();

        //            var expenseInfo = await GetRecentExpenseTypeInfo(ExpenseexistingDocument, supplierId);



        //            var ExpenseTypeListEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);
        //            var ExpenseTypeListEndpointinfo = ExpenseTypeListEndpoint.Endpoint;

        //            var requestBody = new Dictionary<string, string>
        //                {
        //                    {"cid",cidvalue},
        //                    {"pass",passvalue },
        //                    {"user",uservalue }
        //                };


        //            // First call with permanent_property = true
        //            requestBody["permanent_property"] = "true";
        //            var expenseTypeListTrue = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, expenseInfo.ExpenseTypeId);

        //            // Second call with permanent_property = false
        //            requestBody["permanent_property"] = "false";
        //            var expenseTypeListFalse = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, expenseInfo.ExpenseTypeId);

        //            // Sort both lists
        //            expenseTypeListFalse = expenseTypeListFalse.OrderBy(et => et.ExpenseTypeDesc).ToList();
        //            expenseTypeListTrue = expenseTypeListTrue.OrderBy(et => et.ExpenseTypeDesc).ToList();

        //            // Merge lists: expenseTypeListFalse first, then expenseTypeListTrue
        //            ExpenseTypeList = expenseTypeListFalse.Concat(expenseTypeListTrue).ToList();


        //        }

        //        if (DocumentApprovedtoUninet == true)
        //        {
        //            var ExpenseTypeListEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);
        //            var ExpenseTypeListEndpointinfo = ExpenseTypeListEndpoint.Endpoint;

        //            var requestBody = new Dictionary<string, string>
        //                {
        //                    {"cid",cidvalue},
        //                    {"pass",passvalue },
        //                    {"user",uservalue }
        //                };
        //            ExpenseTypeList = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, ExpenseTypeId.ToString());
        //        }

        //    }




        //    return ExpenseTypeList;

        //}
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
            //(string cid, string user, string pass) IcountSuplierCreds,
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
                    //IcountSuplierCreds,
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
            (string ApiToken, string SecretKey) morningSuplierCreds,
            ShowingDocsResults docsResults,
            int userId,
            int internalCompanyId,
            string currencyCode,
            decimal currencyRate,
            int Client_SubCompanyid
            
            )
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(request.JsonDocumentid));
            var doc = await _MorningWebHookData.Find(filter).FirstOrDefaultAsync();

            if (doc != null)
            {
                
                return await MapDocumentToResponseMorning(
                    doc,
                    docsResults,
                    request,
                    morningSuplierCreds,
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

            // ✅ Ensure Authorization Header is added
            if (headers.ContainsKey("Authorization"))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", headers["Authorization"].Replace("Bearer ", ""));
                headers.Remove("Authorization");
            }

            var request = new HttpRequestMessage(method, url);

            // ✅ Add remaining headers
            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            // ✅ Attach JSON content properly
            if (content != null)
            {
                request.Content = content;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            }

            // ✅ Print Debugging Information
            Console.WriteLine("===== HTTP REQUEST DEBUG =====");
            Console.WriteLine($"➡ URL: {url}");
            Console.WriteLine($"➡ Method: {method}");
            Console.WriteLine($"➡ Headers: {JsonConvert.SerializeObject(headers)}");
            if (content != null)
            {
                string requestBodyContent = await content.ReadAsStringAsync();
                Console.WriteLine($"➡ Request Body: {requestBodyContent}");
            }
            Console.WriteLine("================================");

            // ✅ Send Request
            var response = await client.SendAsync(request);
            string responseContent = await response.Content.ReadAsStringAsync();

            // ✅ Log Response
            Console.WriteLine("===== HTTP RESPONSE DEBUG =====");
            Console.WriteLine($"⬅ Status: {response.StatusCode}");
            Console.WriteLine($"⬅ Response Body: {responseContent}");
            Console.WriteLine("================================");

            return response.IsSuccessStatusCode ? responseContent : null;
        }






        private async Task<SupplierItemMorning> AddSupplierMorning(string token, string supplierVatId, string companyName)
        {
            try
            {
                var addSupplierEndpointObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 90);
                var addSupplierEndpoint = addSupplierEndpointObj.Endpoint;
                HttpMethod method = HttpMethod.Post;

                // ✅ Include required fields, even if empty
                var supplierData = new
                {
                    name = companyName,
                    active = true,
                    taxId = supplierVatId.ToString(),
                    department = "",
                    accountingKey = "",
                    paymentTerms = 0,
                    bankName = "",
                    bankBranch = "4",
                    bankAccount = "",
                    address = "",
                    city = "",
                    zip = "",
                    country = "IL",
                    phone = "",
                    fax = "",
                    mobile = "",
                    remarks = "",
                    contactPerson = "",
                    emails = new string[] { },  // Ensure empty array, not null
                    labels = new string[] { }   // Ensure empty array, not null
                };


                var requestBody = new StringContent(JsonConvert.SerializeObject(supplierData), Encoding.UTF8, "application/json");

                // ✅ Ensure correct Authorization header handling
                var requestHeaders = new Dictionary<string, string>
                {
                    { "Authorization", $"Bearer {token}" },
                    { "Accept", "application/json" }
                };


                // ✅ Send API request
                string response = await SendRequestWithHeaders(addSupplierEndpoint, method, requestHeaders, requestBody);

                if (string.IsNullOrEmpty(response))
                {
                    Console.WriteLine("Error: Empty response from Green Invoice API.");
                    return null;
                }

                // ✅ Correctly extract response properties
                var jsonDocument = JsonDocument.Parse(response);
                var jsonData = jsonDocument.RootElement;

                var SupplierItemMorning = new SupplierItemMorning
                {
                    supplier_id = jsonData.TryGetProperty("supplierId", out var supplierIdElement) ? supplierIdElement.GetString() : null,
                    vat_id = jsonData.TryGetProperty("taxId", out var vatIdElement) && vatIdElement.ValueKind == JsonValueKind.String
                        ? int.TryParse(vatIdElement.GetString(), out var vatId) ? vatId : 0
                        : 0,
                    supplier_name = jsonData.TryGetProperty("name", out var nameElement) ? nameElement.GetString() : "Unknown",
                    company_name = jsonData.TryGetProperty("bankName", out var bankNameElement) ? bankNameElement.GetString() : "Unknown"
                };

                return SupplierItemMorning;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in AddSupplierMorning: {ex.Message}");
                return null;
            }
        }





        private async Task<List<SupplierItemMorning>> GetSupplierListMorning(string token,int internalCompanyId,int Client_SubCompanyid,int userId)
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
                var originalMorningJson = BsonDocument.Parse(response);

                

                // Call the function to insert into MongoDB
                //await InsertSupplierListToMongo(originalMorningJson, internalCompanyId, Client_SubCompanyid, userId);
                //remarked by eyal becuase the user didnt choose to approve the doc yet so we can add this suplier to mongo we will do so 
                //only when he approved his doc
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
                        company_name = supplier.TryGetProperty("bankName", out var bankNameElement) ? bankNameElement.GetString() : null,
                        
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



        private async Task InsertSupplierListToMongo(
         BsonDocument originalMorningJson,
         int internalCompanyId,
         int Client_SubCompanyid,
         int userId)
        {
            try
            {
                // Add additional properties to the top level of the JSON
                originalMorningJson["internalCompanyId"] = internalCompanyId;
                originalMorningJson["Client_SubCompanyid"] = Client_SubCompanyid;
                originalMorningJson["userId"] = userId;

                // Define the filter to check if the document already exists
                var existingFilter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("internalCompanyId", internalCompanyId),
                    Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", Client_SubCompanyid),
                    Builders<BsonDocument>.Filter.Eq("userId", userId)
                );

                // Check if a document with the same keys already exists
                var existingDocument = await _MorningClientSuppliers.Find(existingFilter).FirstOrDefaultAsync();

                if (existingDocument == null)
                {
                    // Insert the new JSON into the collection
                    await _MorningClientSuppliers.InsertOneAsync(originalMorningJson);
                }
                else
                {
                    // Update the existing document with the new JSON
                    await _MorningClientSuppliers.ReplaceOneAsync(existingFilter, originalMorningJson);
                }
            }
            catch (Exception ex)
            {
                // Log any exceptions for debugging
                await LogException("InsertSupplierListToMongo", ex);
                throw;
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
    int Client_SubCompanyid
)
        {
            try
            {
                // Parse document fields
                int docNumber = doc.GetValue("number", 0).ToInt32();
                string docTypeName = doc.GetValue("typename", string.Empty).AsString;
                string docDate = doc.GetValue("date", string.Empty).AsString;
                double amountAV = doc.GetValue("total", 0.0).ToDouble();
                double amountBeforeVat = doc.GetValue("subtotal", 0.0).ToDouble();
                double vat = 0.0;

                // Extract VAT from the tax array
                if (doc.TryGetValue("tax", out var taxElement) && taxElement.IsBsonArray)
                {
                    var taxArray = taxElement.AsBsonArray;
                    var vatTax = taxArray.FirstOrDefault(t => t.AsBsonDocument.GetValue("name", string.Empty).AsString == "VAT");
                    if (vatTax != null)
                    {
                        vat = vatTax.AsBsonDocument.GetValue("total", 0.0).ToDouble();
                    }
                }

                return new ExpensesDigitalDocumentProp
                {
                    Supplier_name_Sender = "Unknown", // Will be calculated later
                    Supplier_ID = "0", // Will be calculated later
                    DocNumber = docNumber.ToString(),
                    Doctype = docTypeName,
                    DocDate = DateTime.Parse(docDate),
                    AmountAV = amountAV,
                    Vat = vat,
                    AmountBeforeVat = amountBeforeVat,
                    Jsondocumentid = request.JsonDocumentid,
                    currencyName = currencyCode,
                    CurrenctRateValue = currencyRate,
                    showingDocsResults = docsResults,
                    internalCompanyId = internalCompanyId,
                    TaxId = request.BusinessVatId,
                    SubCompanyIdSuplier = Client_SubCompanyid
                };
            }
            catch (Exception ex)
            {
                await LogException("MapDocumentToResponseMorning", ex);
                return null;
            }
        }

        //        private async Task<ExpensesDigitalDocumentProp> MapDocumentToResponseMorning(
        //    BsonDocument doc,
        //    ShowingDocsResults docsResults,
        //    DigitalDocumentDInputRequest request,
        //    (string ApiToken, string SecretKey) morningClientCreds,
        //    int userId,
        //    int internalCompanyId,
        //    string currencyCode,
        //    decimal currencyRate,
        //    int Client_SubCompanyid
        //)
        //        {
        //            try
        //            {
        //                // Get a new token for Morning
        //                string newToken = await GetNewToken(internalCompanyId, Client_SubCompanyid, userId, 6);

        //                // Fetch supplier details using Morning API
        //                string supplierVatId = request.BusinessVatId;
        //                var supplierList = await GetSupplierListMorning(newToken , internalCompanyId,  Client_SubCompanyid,  userId); // First call
        //                var supplierItem = supplierList.FirstOrDefault(s => s.vat_id == Convert.ToInt32(supplierVatId));

        //                // Define the filter and projection to retrieve company info
        //                var filter = Builders<BsonDocument>.Filter.Eq("taxId", supplierVatId);
        //                var projection = Builders<BsonDocument>.Projection.Include("name").Include("SubCompanyId").Exclude("_id");
        //                var resultMorningCompanyInfo = _MorningCompanisInfoCollection.Find(filter).Project(projection).FirstOrDefault();

        //                string companyName = string.Empty;
        //                int SubCompanyIdSuplier = 0;

        //                if (resultMorningCompanyInfo != null)
        //                {
        //                    if (resultMorningCompanyInfo.TryGetValue("name", out var nameElement))
        //                        companyName = nameElement.AsString;

        //                    if (resultMorningCompanyInfo.TryGetValue("SubCompanyId", out var subCompanyIdElement) && subCompanyIdElement.IsInt32)
        //                        SubCompanyIdSuplier = subCompanyIdElement.AsInt32;
        //                }
        //                /*
        //                 * i remark this code section becuase in the processes of showdocument we dont need to register a new suplier to the 
        //                 * morning system because the user didnt approve the doc yet
        //                if (supplierItem == null)//it means with iddnt find the suplier in the externak system of the client
        //                {
        //                    // Add supplier if not found
        //                    var addedSupplier = await AddSupplierMorning(newToken, supplierVatId, companyName);//so we add the sunpler to the client supliers at his system 
        //                    if (addedSupplier != null)
        //                    {
        //                        supplierItem = new SupplierItemMorning
        //                        {
        //                            supplier_id = addedSupplier.supplier_id,
        //                            vat_id = Convert.ToInt32(addedSupplier.vat_id),
        //                            supplier_name = addedSupplier.supplier_name,
        //                            company_name = addedSupplier.company_name
        //                        };
        //                    }

        //                    // Refresh the supplier list after adding the new supplier
        //                   // supplierList = await GetSupplierListMorning(newToken, internalCompanyId, Client_SubCompanyid, userId); // Second call
        //                   //eyal removed because in the phase of showdocument we dont need to save this suplier to mongodb supliers list collection
        //                }
        //                */



        //                // Parse document fields
        //                int docNumber = doc.GetValue("number", 0).ToInt32();
        //                string docTypeName = doc.GetValue("typename", string.Empty).AsString;
        //                string docDate = doc.GetValue("date", string.Empty).AsString;
        //                double amountAV = doc.GetValue("total", 0.0).ToDouble();
        //                double amountBeforeVat = doc.GetValue("subtotal", 0.0).ToDouble();
        //                double vat = 0.0;

        //                // Extract VAT from the tax array
        //                if (doc.TryGetValue("tax", out var taxElement) && taxElement.IsBsonArray)
        //                {
        //                    var taxArray = taxElement.AsBsonArray;
        //                    var vatTax = taxArray.FirstOrDefault(t => t.AsBsonDocument.GetValue("name", string.Empty).AsString == "VAT");
        //                    if (vatTax != null)
        //                    {
        //                        vat = vatTax.AsBsonDocument.GetValue("total", 0.0).ToDouble();
        //                    }
        //                }

        //                return new ExpensesDigitalDocumentProp
        //                {
        //                    Supplier_name_Sender = supplierItem?.supplier_name ?? "Unknown",
        //                    Supplier_ID = supplierItem?.supplier_id ?? "0",
        //                    DocNumber = docNumber.ToString(),
        //                    Doctype = docTypeName,
        //                    DocDate = DateTime.Parse(docDate),
        //                    AmountAV = amountAV,
        //                    Vat = vat,
        //                    AmountBeforeVat = amountBeforeVat,
        //                    Jsondocumentid = request.JsonDocumentid,
        //                    currencyName = currencyCode,
        //                    CurrenctRateValue = currencyRate,
        //                    showingDocsResults = docsResults,
        //                    internalCompanyId = internalCompanyId,
        //                    TaxId = supplierVatId,
        //                    SubCompanyIdSuplier = SubCompanyIdSuplier
        //                };
        //            }
        //            catch (Exception ex)
        //            {
        //                await LogException("MapDocumentToResponseMorning", ex);
        //                return null;
        //            }
        //        }





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
    //(string cid, string user, string pass) IcountSuplierCreds,
    int userId,
    int internalCompanyId,
    string currencyCode,
    decimal currencyRate)
        {
            try
            {
                var docType = doc["doc_info"]["doctype"].AsString;
                // Map document data to response
                return new ExpensesDigitalDocumentProp
                {
                    DocNumber = doc["doc_info"]["docnum"].AsString,
                    Doctype = doc["doc_info"]["doctype"].AsString,
                    DocDate = DateTime.Parse(doc["doc_info"]["dateissued"].AsString),
                    AmountAV =  doc["doc_info"]["total"].ToDouble(),
                    Vat = (docType == "invrec" || docType == "receipt") ? (double?)null : doc["doc_info"]["totalvat"].ToDouble(),
                    AmountBeforeVat = (docType == "invrec" || docType == "receipt") ? (double?)null : doc["doc_info"]["totalsum"].ToDouble(),
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


        //private async Task<ExpensesDigitalDocumentProp> MapDocumentToResponseIcount(
        //    BsonDocument doc,
        //    ShowingDocsResults docsResults,
        //    DigitalDocumentDInputRequest request,
        //    (string cid, string user, string pass) clientCreds,
        //    int userId,
        //    int internalCompanyId,
        //    string currencyCode,
        //    decimal currencyRate)
        //{
        //    try
        //    {
        //        // Fetch supplier details
        //        string supplierVatId = request.BusinessVatId;
        //        var supplierList = await GetClientSupplierList(clientCreds.cid, clientCreds.user, clientCreds.pass);

        //        var supplierItem = GetSupplierItemByVatId(supplierList, Convert.ToInt32(supplierVatId));
        //        if (supplierItem == null)
        //        {
        //            var supplierData = await FetchOrAddSupplier(supplierVatId, clientCreds);
        //            supplierItem = new SupplierItem
        //            {
        //                supplier_id = supplierData.supplierId,
        //                supplier_name = supplierData.supplierName,
        //                vat_id = Convert.ToInt32(supplierVatId)
        //            };
        //        }

        //        // Map document data to response
        //        return new ExpensesDigitalDocumentProp
        //        {
        //            Supplier_name_Sender = supplierItem.supplier_name.Split('_')[0],
        //            Supplier_ID = supplierItem.supplier_id,
        //            DocNumber = doc["doc_info"]["docnum"].AsString,
        //            Doctype = doc["doc_info"]["doctype"].AsString,
        //            DocDate = DateTime.Parse(doc["doc_info"]["dateissued"].AsString),
        //            AmountAV = doc["doc_info"]["total"].ToDouble(),
        //            Vat = doc["doc_info"]["totalvat"].ToDouble(),
        //            AmountBeforeVat = doc["doc_info"]["totalsum"].ToDouble(),
        //            Jsondocumentid = request.JsonDocumentid,
        //            currencyName = currencyCode,
        //            CurrenctRateValue = currencyRate,
        //            showingDocsResults = docsResults,
        //            internalCompanyId = internalCompanyId,
        //            TaxId = request.BusinessVatId
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await LogException("MapDocumentToResponseIcount", ex);
        //        return null;
        //    }
        //}
        
            private async Task<List<ExpenseType>> CreateExpenseCategorylistIcountOnAddexpense(int icountSubCompanyId, int userId, string supplierId = null, string BusinessVatId = null, string cidvalue = null, string uservalue = null, string passvalue = null, bool? DocumentApprovedtoUninet = false, string? ExpenseTypeId = "0")
        {
            List<ExpenseType> ExpenseTypeList = new List<ExpenseType>();
            string SuplierId = "";
            // Retrieve the business object and get the InternalCompanyId
            var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
            if (business != null)
            {
                
                    int internalCompanyId = business.BusinessId; // Assuming InternalCompanyId is a property of Businesses



                    var filter = Builders<BsonDocument>.Filter.Eq("UserID", userId) &
                                 Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId) &
                                 Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", icountSubCompanyId);


                    var ClientSuppliersItem = await _IcountClientSuppliers.Find(filter).ToListAsync();
                   
                    var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
                            Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId),
                            Builders<BsonDocument>.Filter.Eq("UserID", userId),
                            Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", icountSubCompanyId)

                        );

                    var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();

                    var expenseInfo = await GetRecentExpenseTypeInfo(ExpenseexistingDocument, supplierId);



                    var ExpenseTypeListEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);//https://api.icount.co.il/api/v3.php/expense_type/get_list
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




            return ExpenseTypeList;

        }
        private async Task<List<ExpenseType>> CreateExpenseCategorylistIcountWhenShowDocument(int icountSubCompanyId, int userId, string supplierId = null, string BusinessVatId = null, string cidvalue = null, string uservalue = null, string passvalue = null, bool? DocumentApprovedtoUninet = false, string? ExpenseTypeId = "0")
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


                   
                        var filter = Builders<BsonDocument>.Filter.Eq("UserID", userId) &
                                     Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId) &
                                     Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", icountSubCompanyId);


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
                            Builders<BsonDocument>.Filter.Eq("UserID", userId),
                            Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", icountSubCompanyId)

                        );

                    var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();

                    var expenseInfo = await GetRecentExpenseTypeInfo(ExpenseexistingDocument, supplierId);

                    

                    var ExpenseTypeListEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);//https://api.icount.co.il/api/v3.php/expense_type/get_list
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


        //private async Task<List<ExpenseType>> CreateExpenseCategorylist(int userId, string supplierId = null, string BusinessVatId = null, string cidvalue = null, string uservalue = null, string passvalue = null, bool? DocumentApprovedtoUninet = false, string? ExpenseTypeId = "0")
        //{
        //    List<ExpenseType> ExpenseTypeList = new List<ExpenseType>();
        //    string SuplierId = "";

        //    var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
        //    if (business != null)
        //    {
        //        if (DocumentApprovedtoUninet == false || DocumentApprovedtoUninet == null)
        //        {
        //            int internalCompanyId = business.BusinessId;
        //            var filter = Builders<BsonDocument>.Filter.Eq("UserID", userId) &
        //                         Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId);

        //            var ClientSuppliersItem = await _IcountClientSuppliers.Find(filter).ToListAsync();
        //            foreach (var item in ClientSuppliersItem)
        //            {
        //                BsonDocument suppliers = item["suppliers"].AsBsonDocument;
        //                foreach (var supplierKey in suppliers.Names)
        //                {
        //                    var supplierDetails = suppliers[supplierKey].AsBsonDocument;
        //                    if (supplierDetails["vat_id"].AsString == BusinessVatId)
        //                    {
        //                        SuplierId = supplierDetails["supplier_id"].AsString;
        //                        break;
        //                    }
        //                }
        //            }

        //            var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
        //                Builders<BsonDocument>.Filter.Eq("internalcompanid", internalCompanyId),
        //                Builders<BsonDocument>.Filter.Eq("UserID", userId)
        //            );

        //            var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();
        //            var expenseInfo = await GetRecentExpenseTypeInfo(ExpenseexistingDocument, supplierId);

        //            // If expenseInfo.ExpenseTypeId is null, return an empty list
        //            if (expenseInfo.ExpenseTypeId == null)
        //            {
        //                return ExpenseTypeList;
        //            }

        //            var ExpenseTypeListEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);
        //            var ExpenseTypeListEndpointinfo = ExpenseTypeListEndpoint.Endpoint;

        //            var requestBody = new Dictionary<string, string>
        //    {
        //        {"cid", cidvalue},
        //        {"pass", passvalue},
        //        {"user", uservalue}
        //    };

        //            requestBody["permanent_property"] = "true";
        //            var expenseTypeListTrue = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, expenseInfo.ExpenseTypeId);

        //            requestBody["permanent_property"] = "false";
        //            var expenseTypeListFalse = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, expenseInfo.ExpenseTypeId);

        //            expenseTypeListFalse = expenseTypeListFalse.OrderBy(et => et.ExpenseTypeDesc).ToList();
        //            expenseTypeListTrue = expenseTypeListTrue.OrderBy(et => et.ExpenseTypeDesc).ToList();

        //            ExpenseTypeList = expenseTypeListFalse.Concat(expenseTypeListTrue).ToList();
        //        }
        //    }

        //    return ExpenseTypeList;
        //}

        public async Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId)
        {
            try
            {
                int InternalCompanyIdClient = 0;  // Client receiving the document
                int InternalCompanyIdSupplier = 0; // Supplier providing the document
                int SubCompanyIdClient = 0;
                int SubCompanyIdSupplier = 0;
                string CompanyNameClientRelated = "";
                string CompanyNameSuplierRelated = "";
                int clientExternalid = 0;
                string vatId = expensesUserDoRequest.ClientVat_id.PadLeft(9, '0');
                string supplierVatId = expensesUserDoRequest.BusinessVatId;


                string morningAddress = string.Empty;
                string morningCity = string.Empty;
                string morningZip = string.Empty;
                string morningPhone = string.Empty;
                string morningEmail= string.Empty;



                int supplierExternalSystemId = await GetExternalSystemIdbyjsonId(userId, expensesUserDoRequest.JsonDocumentid);
                var config = _externalSystemConfig[supplierExternalSystemId];

                var RowBusinessData = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == expensesUserDoRequest.JsonDocumentid);
                if (RowBusinessData == null)
                {
                    return new ExpensesDigitalDocumentProp
                    {
                        showingDocsResults = new ShowingDocsResults { Success = false, ErrSec = "Business data not found." }
                    };
                }
                InternalCompanyIdSupplier = RowBusinessData.BusinessId;

                //start   getting info of company suplier from icount or morning based on supplierExternalSystemId
                
                if (supplierExternalSystemId == 6) //suplier is morning
                {
                    var MorningSuplierfilter = Builders<BsonDocument>.Filter.Eq("taxId", supplierVatId.ToString());
                    var morningSuplierCompany = await _MorningCompanisInfoCollection.Find(MorningSuplierfilter).FirstOrDefaultAsync();
                    if (morningSuplierCompany != null)
                    {
                        SubCompanyIdSupplier = morningSuplierCompany["SubCompanyId"].AsInt32;
                        InternalCompanyIdSupplier = morningSuplierCompany["InternalCompanyId"].AsInt32;
                        CompanyNameSuplierRelated = morningSuplierCompany["name"].AsString;

                        // Extract additional fields from the Morning company document
                         morningAddress = morningSuplierCompany.Contains("address") ? morningSuplierCompany["address"].AsString : "";
                         morningCity = morningSuplierCompany.Contains("city") ? morningSuplierCompany["city"].AsString : "";
                         morningZip = morningSuplierCompany.Contains("zip") ? morningSuplierCompany["zip"].AsString : "";
                         morningPhone = morningSuplierCompany.Contains("phone") ? morningSuplierCompany["phone"].AsString : "";
                         morningEmail = morningSuplierCompany.Contains("accountEmail") ? morningSuplierCompany["accountEmail"].AsString : "";

                        // Optionally, store these in class-level variables or include them in an object
                        // so you can use them later when constructing the API request.
                    }
                }
                if (supplierExternalSystemId == 2) //suplier is icount
                {
                    var IcountSuplierfilter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", supplierVatId.ToString());
                    var icountSuplierCompany = await _IcountCompaniesInfoCollection.Find(IcountSuplierfilter).FirstOrDefaultAsync();
                    if (icountSuplierCompany != null)
                    {
                        SubCompanyIdSupplier = icountSuplierCompany["company_info"]["SubCompanyId"].AsInt32;
                        InternalCompanyIdSupplier = icountSuplierCompany["company_info"]["InternalCompanyId"].AsInt32;
                    }
                }
                //End   getting info of company suplier from icount or morning



                //start checking if clientcompanyid is in morning or icount

                var filter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", RowBusinessData.ClientVat_id.ToString());
                var icountCompany = await _IcountCompaniesInfoCollection.Find(filter).FirstOrDefaultAsync();
                if (icountCompany != null)
                {
                    SubCompanyIdClient = icountCompany["company_info"]["SubCompanyId"].AsInt32;
                    InternalCompanyIdClient = icountCompany["company_info"]["InternalCompanyId"].AsInt32;
                    clientExternalid = 2;
                }

                filter = Builders<BsonDocument>.Filter.Eq("taxId", RowBusinessData.ClientVat_id.ToString());
                var morningCompany = await _MorningCompanisInfoCollection.Find(filter).FirstOrDefaultAsync();
                if (morningCompany != null)
                {
                    SubCompanyIdClient = morningCompany["SubCompanyId"].AsInt32;
                    InternalCompanyIdClient = morningCompany["InternalCompanyId"].AsInt32;
                    CompanyNameClientRelated = morningCompany["name"].AsString;
                    clientExternalid = 6;
                }
                //ebd checking if clientcompanyid is in morning or icount
                var clientCredentials = await GetCredentials(userId, InternalCompanyIdClient, SubCompanyIdClient, clientExternalid);
                ExpensesDigitalDocumentProp resultDocument = null;
                SupplierItemMorning supplierItemMorning = null;

                if (supplierExternalSystemId == 2) // Supplier is iCount
                {
                    resultDocument = await ProcessICountDocument(
                        RowBusinessData,
                        expensesUserDoRequest,
                        new ShowingDocsResults { Success = true },
                        userId,
                        InternalCompanyIdSupplier,
                        "ILS",
                        1
                    );


                    // Additional logic to include Supplier_name_Sender, Supplier_ID, and ExpenseTypeList
                    if (clientExternalid == 2) // Client is also iCount
                    {
                        (string cid, string user, string pass) clientCreds = (null, null, null);
                        var c = (dynamic)clientCredentials;
                        clientCreds = (c.Cid, c.User, c.Pass);

                        var supplierList = await GetClientSupplierList(clientCreds.cid, clientCreds.user, clientCreds.pass);
                        var supplierItem = GetSupplierItemByVatId(supplierList, Convert.ToInt32(supplierVatId));

                        if (supplierItem == null) // Supplier not found, add it
                        {
                            var IcountCompaniesfilter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", expensesUserDoRequest.BusinessVatId);
                            var senderCompanyClientRow = await _IcountCompaniesInfoCollection.Find(IcountCompaniesfilter).FirstOrDefaultAsync();

                            string businessName = "", email = "", addressCity = "", addressState = "", addressStreet = "", addressZip = "";

                            if (senderCompanyClientRow != null)
                            {
                                var senderCompanyInfo = senderCompanyClientRow["company_info"].AsBsonDocument;

                                businessName = senderCompanyInfo.Contains("businessName") ? senderCompanyInfo["businessName"].AsString : "";
                                email = senderCompanyInfo.Contains("email") ? senderCompanyInfo["email"].AsString : "";
                                addressCity = senderCompanyInfo.Contains("addressCity") ? senderCompanyInfo["addressCity"].AsString : "";
                                addressState = senderCompanyInfo.Contains("addressState") ? senderCompanyInfo["addressState"].AsString : "";
                                addressStreet = senderCompanyInfo.Contains("addressStreet") ? senderCompanyInfo["addressStreet"].AsString : "";
                                addressZip = senderCompanyInfo.Contains("addressZip") ? senderCompanyInfo["addressZip"].AsString : "";
                            }

                            var clientInfoEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 76);//https://api.icount.co.il/api/v3.php/supplier/add
                            var endpointClientInfo = clientInfoEndpoint.Endpoint;

                            string newBusinessName = businessName + "_" + DateTime.Now.ToString("dd-MM-yyyy");

                            var requestBody = new Dictionary<string, string>
            {
                { "cid", clientCreds.cid },
                { "pass", clientCreds.pass },
                { "user", clientCreds.user },
                { "supplier_name", newBusinessName },
                { "vat_id", expensesUserDoRequest.BusinessVatId },
                { "fname", "" },
                { "lname", "" },
                { "email", email },
                { "phone", "" },
                { "mobile", "" },
                { "fax", "" },
                { "bus_country", addressState },
                { "bus_city", addressCity },
                { "bus_zip", addressZip },
                { "bus_street", addressStreet },
                { "bus_no", "" },
                { "bank", "" },
                { "branch", "" },
                { "account", "" },
                { "faccount", "" },
                { "wht_percent", "" },
                { "wht_validity", "" },
                { "notes", "" }
            };

                            var addedSupplierId = await PostAndGetSupplierId(endpointClientInfo, requestBody);

                            if (!string.IsNullOrEmpty(addedSupplierId))
                            {
                                supplierList = await GetClientSupplierList(clientCreds.cid, clientCreds.user, clientCreds.pass);
                                supplierItem = GetSupplierItemByVatId(supplierList, Convert.ToInt32(supplierVatId));
                            }
                        }

                        if (supplierItem != null)
                        {
                            
                            //this 3 calls are inserting supliers expenses and expenses types to mongodv 
                            //so whe we approvedoc or add expense we will have the data in the mongo
                            await ProcessICountClientSupliersInfo(InternalCompanyIdClient, SubCompanyIdClient, userId, clientCreds.cid, clientCreds.user, clientCreds.pass);//here we call  https://api.icount.co.il/api/v3.php/supplier/get_list
                            await ProcessICountExpenses(InternalCompanyIdClient, SubCompanyIdClient, userId, clientCreds.cid, clientCreds.user, clientCreds.pass);
                            await ProcessICountExpensesTypes(InternalCompanyIdClient, SubCompanyIdClient, userId, clientCreds.cid, clientCreds.user, clientCreds.pass);


                            var expenseList = await CreateExpenseCategorylistIcountWhenShowDocument(SubCompanyIdClient,
                                userId,
                                supplierItem.supplier_id.ToString(),
                                expensesUserDoRequest.BusinessVatId,
                                clientCreds.cid,
                                clientCreds.user,
                                clientCreds.pass,
                                RowBusinessData.DocumentApprovedtoUninet,
                                RowBusinessData.ExpenseTypeId
                            );

                            resultDocument.Supplier_name_Sender = supplierItem.supplier_name.Split('_')[0];
                            resultDocument.Supplier_ID = supplierItem.supplier_id;
                            resultDocument.ExpenseTypeList = expenseList;
                        }
                    }


                    if (clientExternalid == 6) //client is  Morning
                    {
                        string newToken = await GetNewToken(InternalCompanyIdClient, SubCompanyIdClient, userId, 6);
                        var supplierList = await GetSupplierListMorning(newToken, InternalCompanyIdClient, SubCompanyIdClient, userId);
                        supplierItemMorning = supplierList.FirstOrDefault(s => s.vat_id == Convert.ToInt32(supplierVatId));

                        if (supplierItemMorning == null)
                        {
                            string SuplierName = "";
                            var filterSuplierIcount = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", RowBusinessData.BusinessVatId.ToString());
                            var icountCompanyinfoSuplier = await _IcountCompaniesInfoCollection.Find(filterSuplierIcount).FirstOrDefaultAsync();
                            if (icountCompanyinfoSuplier != null)
                            {
                                SuplierName = icountCompanyinfoSuplier["company_info"]["businessName"].AsString;
                               
                            }
                            
                            var addedSupplier = await AddSupplierMorning(newToken, supplierVatId, SuplierName);
                            //this is called again because we add new suplierid
                            supplierList = await GetSupplierListMorning(newToken, InternalCompanyIdClient, SubCompanyIdClient, userId);
                            supplierItemMorning = supplierList.FirstOrDefault(s => s.vat_id == Convert.ToInt32(supplierVatId));
                        }

                        var expenseList = await CreateExpenseCategorylistMorning(
                            userId,
                            resultDocument?.SubCompanyIdSuplier ?? 0,
                            SubCompanyIdClient,
                            supplierItemMorning?.supplier_id ?? "0",
                            supplierItemMorning?.supplier_name ?? "Unknown",
                            supplierVatId,
                            supplierVatId,
                            RowBusinessData.DocumentApprovedtoUninet,
                            RowBusinessData.ExpenseTypeId
                        );
                        if (resultDocument != null)
                        {
                            resultDocument.ExpenseTypeList = expenseList;
                        }
                    }


                }


                else if (supplierExternalSystemId == 6)//suplier is morning
                {
                    var morningSuplierCredsObj = await GetCredentials(userId, RowBusinessData.BusinessId, RowBusinessData.SubCompanyId, supplierExternalSystemId);
                    (string apiToken, string secretKey) morningSuplierCreds = (null, null);
                    var c = (dynamic)morningSuplierCredsObj;
                    morningSuplierCreds = (c.ApiToken, c.SecretKey);

                    resultDocument = await ProcessMorningDocument(
                        RowBusinessData,
                        expensesUserDoRequest,
                        morningSuplierCreds,
                        new ShowingDocsResults { Success = true },
                        userId,
                        InternalCompanyIdSupplier,
                        "ILS",
                        1,
                        SubCompanyIdClient
                    );


                    if (clientExternalid == 6) //client is  Morning
                    {
                        string newToken = await GetNewToken(InternalCompanyIdClient, SubCompanyIdClient, userId, 6);
                        var supplierList = await GetSupplierListMorning(newToken, InternalCompanyIdClient, SubCompanyIdClient, userId);
                        supplierItemMorning = supplierList.FirstOrDefault(s => s.vat_id == Convert.ToInt32(supplierVatId));

                        if (supplierItemMorning == null)
                        {
                            var addedSupplier = await AddSupplierMorning(newToken, supplierVatId, CompanyNameClientRelated);
                            //this is called again because we add new suplierid
                            supplierList = await GetSupplierListMorning(newToken, InternalCompanyIdClient, SubCompanyIdClient, userId);
                            supplierItemMorning = supplierList.FirstOrDefault(s => s.vat_id == Convert.ToInt32(supplierVatId));
                        }

                        var expenseList = await CreateExpenseCategorylistMorning(
                            userId,
                            resultDocument?.SubCompanyIdSuplier ?? 0,
                            SubCompanyIdClient,
                            supplierItemMorning?.supplier_id ?? "0",
                            supplierItemMorning?.supplier_name ?? "Unknown",
                            supplierVatId,
                            supplierVatId,
                            RowBusinessData.DocumentApprovedtoUninet,
                            RowBusinessData.ExpenseTypeId
                        );
                        if (resultDocument != null)
                        {
                            resultDocument.ExpenseTypeList = expenseList;
                        }
                    }


                    if (clientExternalid == 2) // Client is iCount but supplier is from Morning
                    {
                        (string cid, string user, string pass) clientCreds = (null, null, null);
                        var c1 = (dynamic)clientCredentials;
                        clientCreds = (c1.Cid, c1.User, c1.Pass);

                        var supplierList = await GetClientSupplierList(clientCreds.cid, clientCreds.user, clientCreds.pass);
                        var supplierItem = GetSupplierItemByVatId(supplierList, Convert.ToInt32(supplierVatId));

                        if (supplierItem == null) // Supplier not found, add it using Morning info
                        {
                            
                            string businessName = CompanyNameSuplierRelated; // from Morning
                            string email = morningEmail;                     // from Morning
                            string addressCity = morningCity;                // from Morning
                            string addressStreet = morningAddress;           // from Morning
                            string addressZip = morningZip;                  // from Morning
                                                                             // You might also want to use morningPhone if required by the API.

                            var clientInfoEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 76);
                            var endpointClientInfo = clientInfoEndpoint.Endpoint;

                            // Append a date or unique string to ensure the supplier name is unique
                            string newBusinessName = businessName;

                            var requestBody = new Dictionary<string, string>
        {
            { "cid", clientCreds.cid },
            { "pass", clientCreds.pass },
            { "user", clientCreds.user },
            { "supplier_name", newBusinessName },
            { "vat_id", expensesUserDoRequest.BusinessVatId },
            { "fname", "" },
            { "lname", "" },
            { "email", email },
            { "phone", morningPhone },
            { "mobile", "" },
            { "fax", "" },
            { "bus_country", "" },    // You may set a default value if not provided by Morning
            { "bus_city", addressCity },
            { "bus_zip", addressZip },
            { "bus_street", addressStreet },
            { "bus_no", "" },
            { "bank", "" },
            { "branch", "" },
            { "account", "" },
            { "faccount", "" },
            { "wht_percent", "" },
            { "wht_validity", "" },
            { "notes", "" }
        };

                            var addedSupplierId = await PostAndGetSupplierId(endpointClientInfo, requestBody);

                            if (!string.IsNullOrEmpty(addedSupplierId))
                            {
                                supplierList = await GetClientSupplierList(clientCreds.cid, clientCreds.user, clientCreds.pass);
                                supplierItem = GetSupplierItemByVatId(supplierList, Convert.ToInt32(supplierVatId));
                            }
                        }

                        if (supplierItem != null)
                        {
                            // These calls insert supplier expenses and expense types into MongoDB
                            await ProcessICountClientSupliersInfo(InternalCompanyIdClient, SubCompanyIdClient, userId, clientCreds.cid, clientCreds.user, clientCreds.pass);
                            await ProcessICountExpenses(InternalCompanyIdClient, SubCompanyIdClient, userId, clientCreds.cid, clientCreds.user, clientCreds.pass);
                            await ProcessICountExpensesTypes(InternalCompanyIdClient, SubCompanyIdClient, userId, clientCreds.cid, clientCreds.user, clientCreds.pass);

                            var expenseList = await CreateExpenseCategorylistIcountWhenShowDocument(
                                SubCompanyIdClient,
                                userId,
                                supplierItem.supplier_id.ToString(),
                                expensesUserDoRequest.BusinessVatId,
                                clientCreds.cid,
                                clientCreds.user,
                                clientCreds.pass,
                                RowBusinessData.DocumentApprovedtoUninet,
                                RowBusinessData.ExpenseTypeId
                            );

                            resultDocument.Supplier_name_Sender = supplierItem.supplier_name;
                            resultDocument.Supplier_ID = supplierItem.supplier_id;
                            resultDocument.ExpenseTypeList = expenseList;
                        }
                    }

                }




                if (resultDocument != null && supplierItemMorning != null)
                {
                    resultDocument.Supplier_name_Sender = supplierItemMorning.supplier_name;
                    resultDocument.Supplier_ID = supplierItemMorning.supplier_id;
                    resultDocument.SubCompanyIdSuplier = SubCompanyIdClient;
                    
                }
                return resultDocument;
                
            }
            catch (Exception ex)
            {
                await LogException("ShowDigitalDocumentDetails", ex);
                return new ExpensesDigitalDocumentProp
                {
                    showingDocsResults = new ShowingDocsResults { Success = false, ErrSec = "An error occurred while processing the request." }
                };
            }
        }







        //public async Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId)
        //{
        //    try
        //    {
        //        int InternalCompanyId = 0;
        //        int SubCompanyId = 0;
        //        int SubCompanyid_clientRelated = 0;
        //        int BussinesIdClientRelated = 0;
        //        string vatId = expensesUserDoRequest.ClientVat_id.PadLeft(9, '0');
        //        string supplierVatId = expensesUserDoRequest.BusinessVatId;

        //        int externalSystemId = await GetExternalSystemIdbyjsonId(userId, expensesUserDoRequest.JsonDocumentid);
        //        var config = _externalSystemConfig[externalSystemId];

        //        var RowBusinessData = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == expensesUserDoRequest.JsonDocumentid);
        //        if (RowBusinessData == null)
        //        {
        //            return new ExpensesDigitalDocumentProp
        //            {
        //                showingDocsResults = new ShowingDocsResults { Success = false, ErrSec = "Business data not found." }
        //            };
        //        }

        //        // Get SubCompanyid_clientRelated by externalSystemId
        //        if (externalSystemId == 2) // iCount
        //        {
        //            var filter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", RowBusinessData.ClientVat_id.ToString());
        //            var icountCompany = await _IcountCompaniesInfoCollection.Find(filter).FirstOrDefaultAsync();
        //            if (icountCompany != null)
        //            {
        //                SubCompanyid_clientRelated = icountCompany["company_info"]["SubCompanyId"].AsInt32;
        //                BussinesIdClientRelated = icountCompany["company_info"]["InternalCompanyId"].AsInt32;
        //            }
        //        }
        //        else if (externalSystemId == 6) // Morning
        //        {
        //            var filter = Builders<BsonDocument>.Filter.Eq("taxId", RowBusinessData.ClientVat_id.ToString());
        //            var morningCompany = await _MorningCompanisInfoCollection.Find(filter).FirstOrDefaultAsync();
        //            if (morningCompany != null)
        //            {
        //                SubCompanyid_clientRelated = morningCompany["SubCompanyId"].AsInt32;
        //                BussinesIdClientRelated = morningCompany["InternalCompanyId"].AsInt32;
        //            }
        //        }

        //        var clientCredentials = await GetCredentials(userId, BussinesIdClientRelated, SubCompanyid_clientRelated, externalSystemId);

        //        if (externalSystemId == 2)
        //        {
        //            (string cid, string user, string pass) clientCreds = (null, null, null);
        //            var c = (dynamic)clientCredentials;
        //            clientCreds = (c.Cid, c.User, c.Pass);



        //            //on this function i also get the list of supliers and if doesnt exist i register the new suplier on the client
        //            var resultDocument = await ProcessICountDocument(
        //                RowBusinessData,
        //                expensesUserDoRequest,
        //                clientCreds,
        //                new ShowingDocsResults { Success = true },
        //                userId,
        //                InternalCompanyId,
        //                "ILS",
        //                1
        //            );

        //            await ProcessICountClientInfo(BussinesIdClientRelated, SubCompanyid_clientRelated, RowBusinessData.UserId, clientCreds.cid, clientCreds.user, clientCreds.pass);//here we call  https://api.icount.co.il/api/v3.php/supplier/get_list
        //            await ProcessICountExpenses(BussinesIdClientRelated, RowBusinessData.UserId, clientCreds.cid, clientCreds.user, clientCreds.pass);
        //            await ProcessICountExpensesTypes(BussinesIdClientRelated, RowBusinessData.UserId, clientCreds.cid, clientCreds.user, clientCreds.pass);

        //            return resultDocument;
        //        }
        //        else if (externalSystemId == 6)
        //        {
        //            (string apiToken, string secretKey) morningClientCreds = (null, null);
        //            var c = (dynamic)clientCredentials;
        //            morningClientCreds = (c.ApiToken, c.SecretKey);

        //            var resultDocument = await ProcessMorningDocument(
        //                RowBusinessData,
        //                expensesUserDoRequest,
        //                morningClientCreds,
        //                new ShowingDocsResults { Success = true },
        //                userId,
        //                InternalCompanyId,
        //                "ILS",
        //                1,
        //                SubCompanyid_clientRelated
        //            );

        //            if (resultDocument != null && resultDocument.ExpenseTypeList == null)
        //            {
        //                var expenseList = await CreateExpenseCategorylistMorning(
        //                    userId,
        //                    resultDocument.SubCompanyIdSuplier,
        //                    SubCompanyid_clientRelated,
        //                    resultDocument.Supplier_ID,
        //                    resultDocument.Supplier_name_Sender,
        //                    supplierVatId,
        //                    supplierVatId,
        //                    RowBusinessData.DocumentApprovedtoUninet,
        //                    RowBusinessData.ExpenseTypeId
        //                );

        //                resultDocument.ExpenseTypeList = expenseList;
        //                return resultDocument;
        //            }
        //        }

        //        // Default return in case externalSystemId is not 2 or 6
        //        return new ExpensesDigitalDocumentProp
        //        {
        //            showingDocsResults = new ShowingDocsResults { Success = false, ErrSec = "Unsupported external system." }
        //        };
        //    }
        //    catch (Exception ex)
        //    {
        //        await LogException("ShowDigitalDocumentDetails", ex);
        //        return new ExpensesDigitalDocumentProp
        //        {
        //            showingDocsResults = new ShowingDocsResults { Success = false, ErrSec = "An error occurred while processing the request." }
        //        };
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


        public async Task<string> GetCompanyName(int subCompanyId, int mainCompanyId, int externalSystemId)
        {
            IMongoCollection<BsonDocument> collection;
            FilterDefinition<BsonDocument> filter;

            if (externalSystemId == 2)
            {
                collection = _IcountCompaniesInfoCollection;
                filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", mainCompanyId),
                    Builders<BsonDocument>.Filter.Eq("company_info.SubCompanyId", subCompanyId)
                );
            }
            else if (externalSystemId == 6)
            {
                collection = _MorningCompanisInfoCollection;
                filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("InternalCompanyId", mainCompanyId),
                    Builders<BsonDocument>.Filter.Eq("SubCompanyId", subCompanyId)
                );
            }
            else
            {
                throw new ArgumentException("Unsupported ExternalSystemId");
            }

            var projection = externalSystemId == 2
                ? Builders<BsonDocument>.Projection.Include("company_info.businessName")
                : Builders<BsonDocument>.Projection.Include("name");

            var result = await collection.Find(filter).Project(projection).FirstOrDefaultAsync();

            if (result != null)
            {
                try
                {
                    if (externalSystemId == 2 && result.Contains("company_info"))
                    {
                        var companyInfo = result["company_info"].AsBsonDocument;
                        if (companyInfo.Contains("businessName"))
                        {
                            return companyInfo["businessName"].AsString;
                        }
                    }
                    else if (externalSystemId == 6 && result.Contains("name"))
                    {
                        return result["name"].AsString;
                    }
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to extract company name from the document.", ex);
                }
            }

            return string.Empty;
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
                string companyName = await GetCompanyName(company.SubCopmanyId, company.MainCompanyId, ExternalSystemId);

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
                        ShowExceedsMessage = (DocsAccepted >= 200 && UserCreditCardHolderObj == null),
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

        private async Task<List<BusinessData>> FetchPaginatedDigitalDocuments(int UserID, string Typelist, int? subCompanyId, string ClientvatId, int currentPage, int itemsPerPage, bool? documentApprovedToUninet)
        {
            try
            {
                // Calculate startIndex based on currentPage and itemsPerPage
                int startIndex = (currentPage - 1) * itemsPerPage;

                // Fetch the paginated list of digital documents using the updated repository method
                var digitalDocuments = _repository.GetListOfObjectsPaging<BusinessData>(
                    x => x.ClientVat_id == Convert.ToUInt32(ClientvatId) &&
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
                DigitalDocumentToApproveObj resultObj = new DigitalDocumentToApproveObj
                {
                    listDigitalDocumentToApprove = new List<DigitalDocumentToApprove>()
                };

                int MainCompanyId = await ResolveMainCompanyId(UserID);
                List<MainSubCopmaniesMasters> companies = await GetRelatedCompanies(MainCompanyId, UserID);
                subCompanyId ??= ResolveSubCompanyId(companies, MainCompanyId);
                var usersExternalSystemDynamicFieldsResult = await _repository.GetFirstObjectAsync<UsersExternalSystemDynamicFields>(
                    x => x.Companyid == MainCompanyId && x.Userid == UserID && x.SubCompayId == subCompanyId 
                );
                var config = _externalSystemConfig[usersExternalSystemDynamicFieldsResult.ExternalSystemId];
                string ClientvatId = await GetVatId(MainCompanyId, subCompanyId.Value, config);

                List<BusinessData> ResListOfClientCompaniesThatWasSentDigitalDocument = new List<BusinessData>();

                switch (Typelist)
                {
                    case "notApproveOrRejected":
                        ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
                            UserID, Typelist, subCompanyId, ClientvatId, pageNumber, pageSize, null);
                        break;
                    case "Rejected":
                        ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
                            UserID, Typelist, subCompanyId, ClientvatId, pageNumber, pageSize, false);
                        break;
                    case "Approved":
                        ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
                            UserID, Typelist, subCompanyId, ClientvatId, pageNumber, pageSize, true);
                        break;
                    default:
                        break;
                }

                foreach (var doc in ResListOfClientCompaniesThatWasSentDigitalDocument)
                {
                    string JsonDocUrl = string.Empty;
                    string supplierNameSender = string.Empty;
                    int externalSystemId = 0; // Default value, will be determined dynamically

                    // Determine externalSystemId dynamically for each document
                    var filter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", doc.BusinessVatId.ToString());
                    var icountCompany = await _IcountCompaniesInfoCollection.Find(filter).FirstOrDefaultAsync();
                    if (icountCompany != null)
                    {
                        externalSystemId = 2; // ICount
                    }

                    filter = Builders<BsonDocument>.Filter.Eq("taxId", doc.BusinessVatId.ToString());
                    var morningCompany = await _MorningCompanisInfoCollection.Find(filter).FirstOrDefaultAsync();
                    if (morningCompany != null)
                    {
                        externalSystemId = 6; // Morning
                    }

                    // Get configuration based on determined externalSystemId
                    var config2 = GetConfigByExternalSystemId(externalSystemId);

                    if (doc.DataSourceEnum == externalSystemId && doc.DataSourceType == 2)
                    {
                        JsonDocUrl = await FetchJsonDocUrl(doc.JsonDocumentid, config2);
                        supplierNameSender = await FetchSupplierName(doc.BusinessId, doc.SubCompanyId, config2);
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

                resultObj.fullname = await GetUserFullName(UserID);
                resultObj.ListOfSubCompaniesandNames = await PopulateListOfSubCompaniesandNames(companies, UserID, usersExternalSystemDynamicFieldsResult.ExternalSystemId);

                return resultObj;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }


        private async Task ProcessMorningSupplierInfo(int MainCompanyId, int UserID, string token,int subCompanyId)
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

        private async Task ProcessMorningExpenses(int MainCompanyId, int UserID, string token, int subCompanyId)
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
                    // Define filter to check if the document exists for `internalCompanyId`, `UserID`, and `subCompanyId`
                    var filter = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", MainCompanyId),
                        Builders<BsonDocument>.Filter.Eq("UserID", UserID),
                        Builders<BsonDocument>.Filter.Eq("subCompanyId", subCompanyId)
                    );

                    var existingDocument = await _MorningExpenses.Find(filter).FirstOrDefaultAsync();

                    if (existingDocument == null)
                    {
                        // Create a new document with all items under `results_list`
                        var newDocument = new BsonDocument
                        {
                            { "internalCompanyId", MainCompanyId },
                            { "UserID", UserID },
                            { "subCompanyId", subCompanyId }, // Add subCompanyId
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
                            existingDocument["subCompanyId"] = subCompanyId; // Ensure subCompanyId is added or updated
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


        private async Task ProcessMorningExpensesTypes(int MainCompanyId, int UserID, string token,int subCompanyId)
        {
            try
            {
                // Fetch the endpoint configuration for Morning Expenses Types
                var allExpensesTypeObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id ==47 ); // Update with correct ID
                var allExpensesEndpoint = allExpensesTypeObj.Endpoint; // 
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


        private async Task<string> SendRequestWithToken(string endpoint, HttpMethod method, string token=null, string payload = null)
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

        private async Task ProcessICountClientSupliersInfo(int MainCompanyId,int SubCompanyid_clientRelated, int UserID, string cid, string user, string pass)//this name is wrong it gets only supliers
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
                bsonDocument.Add("Client_SubCompanyid", SubCompanyid_clientRelated);

                var filterClientSuppliers = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                    Builders<BsonDocument>.Filter.Eq("UserID", UserID),
                    Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyid_clientRelated)
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

        private async Task ProcessICountExpenses(int MainCompanyId, int SubCompanyid_clientRelated, int UserID, string cid, string user, string pass)
        {
            var ExpenseSearchObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 77);//https://api.icount.co.il/api/v3.php/expense/search
            var ExpenseSearchEndpoint = $"{ExpenseSearchObj.Endpoint}?cid={cid}&user={user}&pass={pass}";
            HttpMethod methodexpenseserach = HttpMethod.Get;
            var ReponsneExpenseSearch = await SendRequest(ExpenseSearchEndpoint, methodexpenseserach);

            var jsonDocumentReponsneExpenseSearch = JsonDocument.Parse(ReponsneExpenseSearch);
            var jsonDataExpenseSearch = jsonDocumentReponsneExpenseSearch.RootElement;
            var bsonDocumentExpenseSearch = BsonDocument.Parse(jsonDataExpenseSearch.ToString());
            bsonDocumentExpenseSearch.Add("internalcompanid", MainCompanyId);
            bsonDocumentExpenseSearch.Add("UserID", UserID);
            bsonDocumentExpenseSearch.Add("Client_SubCompanyid", SubCompanyid_clientRelated);

            var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                Builders<BsonDocument>.Filter.Eq("UserID", UserID),
                 Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyid_clientRelated)
            );

            var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();
            if (ExpenseexistingDocument != null)
            {
                _IcountExpenses.DeleteOne(filterExpenseSearch);
            }
            _IcountExpenses.InsertOne(bsonDocumentExpenseSearch);
        }

        private async Task ProcessICountExpensesTypes(int MainCompanyId, int SubCompanyid_clientRelated, int UserID, string cid, string user, string pass)
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
            bsonDocumentExpensetypes.Add("Client_SubCompanyid", SubCompanyid_clientRelated);

            var filterExpensetypes = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                Builders<BsonDocument>.Filter.Eq("UserID", UserID),
                Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyid_clientRelated)
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

        /*public async Task<AddGenericexpenseTypeResponse> AddexpenseType(AddexpenseTypeRequest addexpenseTypeRequest, int userId)
        {
            try
            {
                // Extract SubCompanyId from MorningCompanisInfo collection
                var filterMorningCompany = Builders<BsonDocument>.Filter.Eq("taxId", addexpenseTypeRequest.tax_id.ToString());
                var morningCompanyRow = await _MorningCompanisInfoCollection.Find(filterMorningCompany).FirstOrDefaultAsync();

                int morningSubCompanyId = morningCompanyRow?["SubCompanyId"].AsInt32 ?? 0;

                // Extract SubCompanyId from _IcountCompaniesInfoCollection
                var filterIcountCompany = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", addexpenseTypeRequest.tax_id.ToString());
                var icountCompanyRow = await _IcountCompaniesInfoCollection.Find(filterIcountCompany).FirstOrDefaultAsync();

                int icountSubCompanyId = icountCompanyRow?["company_info"]["SubCompanyId"].AsInt32 ?? 0;
                int internalCompanyId = icountCompanyRow?["company_info"]["InternalCompanyId"].AsInt32 ?? 0;

                // Query BusinessData table
                //var rowBusinessData = await _repository.GetFirstObjectAsync<BusinessData>(x =>
                //    x.ClientVat_id == addexpenseTypeRequest.tax_id &&
                //    x.UserId == userId &&
                //    x.BusinessId == addexpenseTypeRequest.internalCompanyId &&
                //    x.SubCompanyId == (morningSubCompanyId != 0 ? morningSubCompanyId : icountSubCompanyId));

                //if (rowBusinessData == null)
                //{
                //    throw new Exception("Business data not found.");
                //}

            //    if (rowBusinessData.DataSourceEnum == 6) // Morning logic
            //    {
            //        // Check if entry exists in _MorningexpenseTypesManual
            //        var existingDocumentFilter = Builders<BsonDocument>.Filter.And(
            //            Builders<BsonDocument>.Filter.Eq("internalCompanyId", addexpenseTypeRequest.internalCompanyId),
            //            Builders<BsonDocument>.Filter.Eq("UserID", userId),
            //            Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", morningSubCompanyId)
            //        );

            //        var existingDocument = await _MorningexpenseTypesManual.Find(existingDocumentFilter).FirstOrDefaultAsync();
            //        string NewexpenseTypeId = Guid.NewGuid().ToString() + "_temp";
            //        var newExpense = new BsonDocument
            //{
            //    { "vat_to_expense", addexpenseTypeRequest.vat_to_expense },
            //    { "expense_type_name", addexpenseTypeRequest.expense_type_name },
            //    { "deductable_vat", addexpenseTypeRequest.deductable_vat },
            //    { "deductable_expense", addexpenseTypeRequest.deductable_expense },
            //    { "permanent_property", addexpenseTypeRequest.permanent_property },
            //    { "no_vat", addexpenseTypeRequest.no_vat },
            //    { "supplier_ID", addexpenseTypeRequest.supplier_ID },
            //    { "Lang", addexpenseTypeRequest.Lang },
            //    { "tax_id", addexpenseTypeRequest.tax_id },
            //            {"ExpenseTypeid", NewexpenseTypeId}
            //};

            //        if (existingDocument != null)
            //        {
            //            // Check if the expense already exists
            //            var expensesArray = existingDocument["expenses"].AsBsonArray;
            //            bool expenseExists = expensesArray.Any(expense =>
            //                expense["expense_type_name"].AsString == addexpenseTypeRequest.expense_type_name &&
            //                expense["supplier_ID"].AsString == addexpenseTypeRequest.supplier_ID &&
            //                expense["tax_id"].AsInt32 == addexpenseTypeRequest.tax_id);

            //            if (!expenseExists)
            //            {
            //                // Add new expense to the "expenses" array
            //                var updateDefinition = Builders<BsonDocument>.Update.Push("expenses", newExpense);
            //                await _MorningexpenseTypesManual.UpdateOneAsync(existingDocumentFilter, updateDefinition);
            //            }
            //        }
            //        else
            //        {
            //            // Create a new document
            //            var newDocument = new BsonDocument
            //    {
            //        { "internalCompanyId", addexpenseTypeRequest.internalCompanyId },
            //        { "UserID", userId },
            //        { "Client_SubCompanyid", morningSubCompanyId },
            //        { "expenses", new BsonArray { newExpense } }
            //    };

            //            await _MorningexpenseTypesManual.InsertOneAsync(newDocument);
            //        }

            //        // Build the ExpenseTypeList
            //        List<ExpenseType> ExpenseTypeList = new List<ExpenseType>();

            //        // Fetch existing expenses from _MorningExpensesTypes collection
            //        var existingExpensesFilter = Builders<BsonDocument>.Filter.And(
            //            Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", morningSubCompanyId),
            //            Builders<BsonDocument>.Filter.Eq("userId", userId),
            //            Builders<BsonDocument>.Filter.Eq("internalCompanyId", addexpenseTypeRequest.internalCompanyId)
            //        );

            //        var existingExpensesDocument = await _MorningExpensesTypes.Find(existingExpensesFilter).FirstOrDefaultAsync();
            //        if (existingExpensesDocument != null)
            //        {
            //            var expenseTypes = existingExpensesDocument["expense_types"].AsBsonArray;
            //            foreach (var expenseType in expenseTypes)
            //            {
            //                ExpenseTypeList.Add(new ExpenseType
            //                {
            //                    ExpenseTypeId = expenseType["ExpenseTypeId"].AsString,
            //                    ExpenseTypeDesc = expenseType["ExpenseTypeDesc"].AsString,
            //                    IsDefault = false
            //                });
            //            }
            //        }

            //        // Add the new expense to the list
            //        ExpenseTypeList.Add(new ExpenseType
            //        {
            //            ExpenseTypeId = NewexpenseTypeId,
            //            ExpenseTypeDesc = addexpenseTypeRequest.expense_type_name,
            //            IsDefault = true
            //        });

            //        return new AddGenericexpenseTypeResponse
            //        {
            //            textResponse = addexpenseTypeRequest.Lang == 1 ? "The expense type was added successfully" : "סוג הוצאה התווסף בהצלחה",
            //            ExpenseTypeList = ExpenseTypeList
            //        };
            //    }
            //    else if (rowBusinessData.DataSourceEnum == 2) // iCount logic
            //    {
                    // iCount-specific logic (existing logic remains unchanged)
                    // Set up credentials
                    List<UsersExternalSystemDynamicFields> userExternalFields = null;
                    var userMaster = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == userId);

                    if (userMaster != null)
                    {
                        userExternalFields = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == internalCompanyId && x.Userid == userId && x.SubCompayId == icountSubCompanyId);
                    }
                    else
                    {
                        var relatedMaster = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == userId);
                        if (relatedMaster != null)
                        {
                            userExternalFields = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == relatedMaster.CompanyId && x.Userid == relatedMaster.Userid && x.SubCompayId == icountSubCompanyId);
                            userId = relatedMaster.Userid;
                        }
                        else
                        {
                            userExternalFields = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == internalCompanyId && x.Userid == userId && x.SubCompayId == icountSubCompanyId);
                        }
                    }

                    string cid = userExternalFields?.FirstOrDefault(x => x.FieldLabelName == "cid")?.FieldLabelValue;
                    string user = userExternalFields?.FirstOrDefault(x => x.FieldLabelName == "user")?.FieldLabelValue;
                    string pass = userExternalFields?.FirstOrDefault(x => x.FieldLabelName == "pass")?.FieldLabelValue;

                    var addExpenseEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 80);
                    string endpointUrl = addExpenseEndpoint.Endpoint + "?cid=" + cid + "&user=" + user + "&pass=" + pass;

                    string postData = "{" +
                        "\"vat_to_expense\": " + addexpenseTypeRequest.vat_to_expense.ToString().ToLower() + "," +
                        "\"expense_type_name\": \"" + addexpenseTypeRequest.expense_type_name.Replace("\"", "\\\"") + "\"," +
                        "\"deductable_vat\": " + addexpenseTypeRequest.deductable_vat + "," +
                        "\"deductable_expense\": " + addexpenseTypeRequest.deductable_expense + "," +
                        "\"permanent_property\": " + addexpenseTypeRequest.permanent_property.ToString().ToLower() + "," +
                        "\"no_vat\": " + addexpenseTypeRequest.no_vat.ToString().ToLower() + "}";

                    string response = await SendRequest(endpointUrl, HttpMethod.Post, postData);
                    var icountResult = JsonConvert.DeserializeObject<AddexpenseTypeResponse>(response);

                    if (!icountResult.status)
                    {
                        return new AddGenericexpenseTypeResponse
                        {
                            textResponse = addexpenseTypeRequest.Lang == 1 ? icountResult.error_details?[0] ?? "Error adding expense" : "שגיאה בהוספת הוצאה",
                            ExpenseTypeList = new List<ExpenseType>()
                        };
                    }

                    var expenseList = await CreateExpenseCategorylistIcountOnAddexpense(icountSubCompanyId,userId, addexpenseTypeRequest.supplier_ID, addexpenseTypeRequest.tax_id.ToString(), cid, user, pass);
                    return new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1 ? "The expense type was added successfully" : "סוג הוצאה התווסף בהצלחה",
                        ExpenseTypeList = expenseList
                    };
               // }

                return null;
            }
            catch (Exception ex)
            {
                // Log the exception
                return null;
            }
        }*/



        public async Task<AddGenericexpenseTypeResponse> AddexpenseType(AddexpenseTypeRequest addexpenseTypeRequest, int userId)
        {
            try
            {
                // 1. Check iCount collection
                var filterIcount = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", addexpenseTypeRequest.tax_id.ToString());
                var icountCompanyRow = await _IcountCompaniesInfoCollection.Find(filterIcount).FirstOrDefaultAsync();

                // 2. Check Morning collection
                var filterMorning = Builders<BsonDocument>.Filter.Eq("taxId", addexpenseTypeRequest.tax_id.ToString());
                var morningCompanyRow = await _MorningCompanisInfoCollection.Find(filterMorning).FirstOrDefaultAsync();

                // 3. Decide which system we are dealing with:
                //    iCount or Morning?
                if (icountCompanyRow != null)
                {
                    // ---------------------
                    //  iCOUNT LOGIC
                    // ---------------------

                    // Extract internalCompanyId & subCompanyId from the iCount document
                    int internalCompanyId = icountCompanyRow["company_info"]["InternalCompanyId"].AsInt32;
                    int icountSubCompanyId = icountCompanyRow["company_info"]["SubCompanyId"].AsInt32;

                    // Retrieve iCount credentials from dynamic fields
                    var userExternalFields = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                        x => x.Companyid == internalCompanyId &&
                             x.Userid == userId &&
                             x.SubCompayId == icountSubCompanyId
                    );

                    string cid = userExternalFields?.FirstOrDefault(x => x.FieldLabelName == "cid")?.FieldLabelValue;
                    string icountUser = userExternalFields?.FirstOrDefault(x => x.FieldLabelName == "user")?.FieldLabelValue;
                    string pass = userExternalFields?.FirstOrDefault(x => x.FieldLabelName == "pass")?.FieldLabelValue;

                    if (string.IsNullOrEmpty(cid) || string.IsNullOrEmpty(icountUser) || string.IsNullOrEmpty(pass))
                        throw new Exception("Missing iCount credentials (cid, user, pass).");

                    // Build endpoint URL (assuming you have it in your DB with Id=80)
                    var addExpenseEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 80);
                    if (addExpenseEndpoint == null)
                        throw new Exception("Missing iCount addExpenseType endpoint definition.");

                    string endpointUrl = $"{addExpenseEndpoint.Endpoint}?cid={cid}&user={icountUser}&pass={pass}";

                    // Build the JSON payload for iCount
                    var payloadObj = new
                    {
                        expense_type_name = addexpenseTypeRequest.expense_type_name,
                        deductable_expense = addexpenseTypeRequest.deductable_expense,
                        deductable_vat = addexpenseTypeRequest.deductable_vat,
                        permanent_property = addexpenseTypeRequest.permanent_property,
                        no_vat = addexpenseTypeRequest.no_vat,
                        vat_to_expense = addexpenseTypeRequest.vat_to_expense
                        // Add "sortcode_id" if needed
                    };
                    string postData = JsonConvert.SerializeObject(payloadObj);

                    // Send request to iCount
                    string response = await SendRequest(endpointUrl, HttpMethod.Post, postData);
                    var icountResult = JsonConvert.DeserializeObject<AddexpenseTypeResponse>(response);
                    if (!icountResult.status)
                    {
                        return new AddGenericexpenseTypeResponse
                        {
                            textResponse = addexpenseTypeRequest.Lang == 1
                                ? icountResult.error_details?[0] ?? "Error adding expense type to iCount"
                                : "שגיאה בהוספת סוג הוצאה ב-iCount",
                            ExpenseTypeList = new List<ExpenseType>()
                        };
                    }

                    // If success, refresh the iCount expense types
                    var expenseList = await CreateExpenseCategorylistIcountOnAddexpense(
                        icountSubCompanyId,
                        userId,
                        addexpenseTypeRequest.supplier_ID,
                        addexpenseTypeRequest.tax_id.ToString(),
                        cid,
                        icountUser,
                        pass
                    );

                    return new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1
                            ? "The expense type was added successfully to iCount"
                            : "סוג הוצאה התווסף בהצלחה ל-iCount",
                        ExpenseTypeList = expenseList
                    };
                }
               /* else if (morningCompanyRow != null)
                {
                    // ---------------------
                    //  MORNING LOGIC--- remark eyal mornig is unable to add  expense using api  this is the reason i remarl the code 
                    // ---------------------
                
                    int internalCompanyId = morningCompanyRow["InternalCompanyId"].AsInt32;
                    int morningSubCompanyId = morningCompanyRow["SubCompanyId"].AsInt32;

                    // Check if entry exists in _MorningexpenseTypesManual
                    var existingDocumentFilter = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", internalCompanyId),
                        Builders<BsonDocument>.Filter.Eq("UserID", userId),
                        Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", morningSubCompanyId)
                    );
                    var existingDocument = await _MorningexpenseTypesManual.Find(existingDocumentFilter).FirstOrDefaultAsync();

                    string newExpenseTypeId = Guid.NewGuid().ToString() + "_temp";
                    var newExpense = new BsonDocument
            {
                { "vat_to_expense", addexpenseTypeRequest.vat_to_expense },
                { "expense_type_name", addexpenseTypeRequest.expense_type_name },
                { "deductable_vat", addexpenseTypeRequest.deductable_vat },
                { "deductable_expense", addexpenseTypeRequest.deductable_expense },
                { "permanent_property", addexpenseTypeRequest.permanent_property },
                { "no_vat", addexpenseTypeRequest.no_vat },
                { "supplier_ID", addexpenseTypeRequest.supplier_ID },
                { "Lang", addexpenseTypeRequest.Lang },
                { "tax_id", addexpenseTypeRequest.tax_id },
                { "ExpenseTypeid", newExpenseTypeId }
            };

                    if (existingDocument != null)
                    {
                        // Check if the expense type already exists
                        var expensesArray = existingDocument["expenses"].AsBsonArray;
                        bool expenseExists = expensesArray.Any(expense =>
                            expense["expense_type_name"].AsString == addexpenseTypeRequest.expense_type_name &&
                            expense["supplier_ID"].AsString == addexpenseTypeRequest.supplier_ID &&
                            expense["tax_id"].AsInt32 == addexpenseTypeRequest.tax_id
                        );
                        if (!expenseExists)
                        {
                            var updateDefinition = Builders<BsonDocument>.Update.Push("expenses", newExpense);
                            await _MorningexpenseTypesManual.UpdateOneAsync(existingDocumentFilter, updateDefinition);
                        }
                    }
                    else
                    {
                        // Create a new doc if none exists
                        var newDocument = new BsonDocument
                {
                    { "internalCompanyId", internalCompanyId },
                    { "UserID", userId },
                    { "Client_SubCompanyid", morningSubCompanyId },
                    { "expenses", new BsonArray { newExpense } }
                };
                        await _MorningexpenseTypesManual.InsertOneAsync(newDocument);
                    }

                    // Build the expense type list to return
                    List<ExpenseType> expenseTypeList = new List<ExpenseType>();
                    var existingExpensesFilter = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", morningSubCompanyId),
                        Builders<BsonDocument>.Filter.Eq("userId", userId),
                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", internalCompanyId)
                    );
                    var existingExpensesDocument = await _MorningExpensesTypes.Find(existingExpensesFilter).FirstOrDefaultAsync();
                    if (existingExpensesDocument != null)
                    {
                        var expenseTypes = existingExpensesDocument["expense_types"].AsBsonArray;
                        foreach (var expenseType in expenseTypes)
                        {
                            expenseTypeList.Add(new ExpenseType
                            {
                                ExpenseTypeId = expenseType["ExpenseTypeId"].AsString,
                                ExpenseTypeDesc = expenseType["ExpenseTypeDesc"].AsString,
                                IsDefault = false
                            });
                        }
                    }
                    // Add the newly created expense type
                    expenseTypeList.Add(new ExpenseType
                    {
                        ExpenseTypeId = newExpenseTypeId,
                        ExpenseTypeDesc = addexpenseTypeRequest.expense_type_name,
                        IsDefault = true
                    });

                    return new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1
                            ? "The expense type was added successfully (Morning)"
                            : "סוג הוצאה התווסף בהצלחה (מורנינג)",
                        ExpenseTypeList = expenseTypeList
                    };
                }*/
                else
                {
                    // Not found in iCount or Morning
                    throw new Exception("No matching company found in iCount or Morning for tax_id " + addexpenseTypeRequest.tax_id);
                }
            }
            catch (Exception ex)
            {
                // Handle error
                return new AddGenericexpenseTypeResponse
                {
                    textResponse = "Error: " + ex.Message,
                    ExpenseTypeList = new List<ExpenseType>()
                };
            }
        }


        private async Task<string> SendToGreenInvoice(string endpoint, object payload, string token)
        {
            try
            {
                using var client = new HttpClient();

                // Add Authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Serialize payload to JSON
                var jsonPayload = JsonConvert.SerializeObject(payload);

                // Create the request content
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // Send the POST request
                var response = await client.PostAsync(endpoint, content);

                // Capture the response content even if the request fails
                var responseContent = await response.Content.ReadAsStringAsync();

                // Ensure the request was successful
                if (!response.IsSuccessStatusCode)
                {
                    // Parse the error content and throw a custom exception
                    throw new HttpRequestException($"Request failed with status code {response.StatusCode}")
                    {
                        Data =
                {
                    { "ResponseContent", responseContent }
                }
                    };
                }

                return responseContent;
            }
            catch (HttpRequestException ex)
            {
                // Handle the captured response content
                if (ex.Data.Contains("ResponseContent"))
                {
                    string responseContent = ex.Data["ResponseContent"] as string;

                    try
                    {
                        // Attempt to parse the response content
                        var errorResponse = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
                        if (errorResponse != null && errorResponse.ContainsKey("errorCode"))
                        {
                            ex.Data["errorCode"] = Convert.ToInt32(errorResponse["errorCode"]);
                            ex.Data["errorMessage"] = errorResponse["errorMessage"];
                        }
                    }
                    catch (Exception parseEx)
                    {
                        Console.WriteLine($"Error parsing response content: {parseEx.Message}");
                    }
                }

                // Log the exception and rethrow
                Console.WriteLine($"Request error: {ex.Message}");
                throw;
            }
        }




        //private string GetThumbnailFromDocument(BsonDocument doc)
        //{
        //    if (doc.Contains("files") && doc["files"].AsBsonDocument.Contains("downloadLinks"))
        //    {
        //        var downloadLinks = doc["files"]["downloadLinks"].AsBsonDocument;

        //        // Check for processedUrl or other valid links
        //        if (downloadLinks.Contains("processedUrl"))
        //        {
        //            var base64Content = downloadLinks["processedUrl"].AsString;

        //            // Ensure the Base64 string is formatted correctly as a data URI
        //            if (!base64Content.StartsWith("data:image/"))
        //            {
        //                base64Content = "data:image/png;base64," + base64Content; // Default to PNG
        //            }
        //            return base64Content;
        //        }
        //        else if (downloadLinks.Contains("he"))
        //        {
        //            return downloadLinks["he"].AsString; // Fallback to a URL
        //        }
        //        else if (downloadLinks.Contains("origin"))
        //        {
        //            return downloadLinks["origin"].AsString; // Fallback to another URL
        //        }
        //    }

        //    // Default to a placeholder image if no valid link is found
        //    return "https://your-default-thumbnail-url.com/placeholder.png";
        //}




        private async Task<byte[]> FetchDocumentData(string documentId, int externalSupplierSystemId)
        {
            try
            {
                if (externalSupplierSystemId == 6) // Morning system
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(documentId));
                    var document = await _MorningWebHookData.Find(filter).FirstOrDefaultAsync();
                    if (document == null)
                    {
                        throw new Exception("Document not found in MongoDB (Morning).");
                    }

                    if (document.Contains("files") && document["files"].IsBsonDocument)
                    {
                        var files = document["files"].AsBsonDocument;
                        if (files.Contains("processedUrl"))
                        {
                            string processedUrl = files["processedUrl"].AsString;
                            // Assume the URL is in the format "data:application/pdf;base64,xxx"
                            var parts = processedUrl.Split(',');
                            if (parts.Length > 1)
                            {
                                string base64Data = parts[1];
                                return Convert.FromBase64String(base64Data);
                            }
                        }

                        if (files.Contains("downloadLinks") && files["downloadLinks"].IsBsonDocument)
                        {
                            var downloadLinks = files["downloadLinks"].AsBsonDocument;
                            if (downloadLinks.Contains("he"))
                            {
                                string heUrl = downloadLinks["he"].AsString;
                                using var client = new HttpClient();
                                var response = await client.GetAsync(heUrl);
                                if (!response.IsSuccessStatusCode)
                                {
                                    throw new HttpRequestException($"Failed to download file from 'he' URL with status code {response.StatusCode}");
                                }
                                return await response.Content.ReadAsByteArrayAsync();
                            }
                        }
                    }
                    throw new Exception("No valid file URL found in MongoDB document (Morning).");
                }
                else if (externalSupplierSystemId == 2) // Icount system
                {
                    var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(documentId));
                    var document = await _IcountWebhookData.Find(filter).FirstOrDefaultAsync();
                    if (document == null)
                    {
                        throw new Exception("Document not found in MongoDB (Icount).");
                    }

                    if (document.Contains("doc_info") && document["doc_info"].IsBsonDocument)
                    {
                        var docInfo = document["doc_info"].AsBsonDocument;
                        if (docInfo.Contains("doc_url_copy"))
                        {
                            string docUrlCopy = docInfo["doc_url_copy"].AsString;
                            // If the document data is inline (base64-encoded)
                            if (docUrlCopy.StartsWith("data:application/pdf;base64,"))
                            {
                                string base64Data = docUrlCopy.Substring("data:application/pdf;base64,".Length);
                                return Convert.FromBase64String(base64Data);
                            }
                            else
                            {
                                using var client = new HttpClient();
                                var response = await client.GetAsync(docUrlCopy);
                                if (!response.IsSuccessStatusCode)
                                {
                                    throw new HttpRequestException($"Failed to download file from iCount doc_url_copy with status code {response.StatusCode}");
                                }
                                return await response.Content.ReadAsByteArrayAsync();
                            }
                        }
                        throw new Exception("doc_url_copy not found in doc_info (Icount).");
                    }
                    throw new Exception("No valid doc_info found in MongoDB document (Icount).");
                }
                else
                {
                    throw new Exception("Unsupported ExternalSupplierSystemId value.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching document data: {ex.Message}");
                throw;
            }
        }



        private async Task<(string uploadUrl, Dictionary<string, string> fields)> GetUploadUrl(string token, string expenseId)
        {
            try
            {
                using var client = new HttpClient();

                // Add Authorization header
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Morning API endpoint to retrieve the upload URL
                string url = $"https://api.sandbox.d.greeninvoice.co.il/file-upload/v1/url?context=expense&data=%7B%22source%22%3A5%2C%22id%22%3A%22{expenseId}%22%2C%22state%22%3A%22expense%22%7D";

                // Send the GET request
                var response = await client.GetAsync(url);
                var responseContent = await response.Content.ReadAsStringAsync();

                // Ensure the request was successful
                if (!response.IsSuccessStatusCode)
                {
                    throw new HttpRequestException($"Failed to retrieve upload URL with status code {response.StatusCode}");
                }

                // Parse the response
                var responseJson = JsonConvert.DeserializeObject<Dictionary<string, object>>(responseContent);
                if (responseJson == null || !responseJson.ContainsKey("url") || !responseJson.ContainsKey("fields"))
                {
                    throw new Exception("Invalid response: Missing 'url' or 'fields'.");
                }

                string uploadUrl = responseJson["url"].ToString();
                var fields = JsonConvert.DeserializeObject<Dictionary<string, string>>(responseJson["fields"].ToString());

                return (uploadUrl, fields);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving upload URL: {ex.Message}");
                throw;
            }
        }

        private async Task<string> UploadFileToUrl(string uploadUrl, byte[] fileData, string fileName, Dictionary<string, string> fields)
        {
            try
            {
                using var client = new HttpClient();

                // Create multipart form-data content
                using var content = new MultipartFormDataContent();

                // Add the fields to the form-data
                foreach (var field in fields)
                {
                    content.Add(new StringContent(field.Value), field.Key);
                }

                // Add the file to the form-data
                var fileContent = new ByteArrayContent(fileData);
                fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                content.Add(fileContent, "file", fileName);

                // Send the POST request
                var response = await client.PostAsync(uploadUrl, content);

                // Ensure the request was successful
                if (response.StatusCode != HttpStatusCode.NoContent)
                {
                    throw new HttpRequestException($"Failed to upload file with status code {response.StatusCode}");
                }

                Console.WriteLine("File uploaded successfully.");
                return "Upload successful";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
                throw;
            }
        }


        private async Task<string> SerializeFormDataContent(MultipartFormDataContent content)
        {
            var formData = new Dictionary<string, object>();

            foreach (var item in content)
            {
                if (item is StringContent stringContent)
                {
                    formData[item.Headers.ContentDisposition.Name.Trim('"')] = await stringContent.ReadAsStringAsync();
                }
                else if (item is ByteArrayContent byteArrayContent)
                {
                    formData[item.Headers.ContentDisposition.Name.Trim('"')] = new
                    {
                        FileName = item.Headers.ContentDisposition.FileName?.Trim('"'),
                        ContentType = byteArrayContent.Headers.ContentType?.MediaType,
                        ContentLength = byteArrayContent.Headers.ContentLength
                    };
                }
            }

            return System.Text.Json.JsonSerializer.Serialize(formData, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        }



        public async Task<createExpenseApiResponse> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest insertUserDigitalDocRequest, int userId)
        {
            try
            {
                string dateissued = "";
                string doc_url_copy = "";
                string DatePayed = "";
                string result_endpointIcountExpenseCreate = "";
                bool payed = true;
                createExpenseApiResponse response = null;
                List<UsersExternalSystemDynamicFields> UserexternalSystemDynamicFieldslist = null;

                // Get ClientVat_id from BusinessData using JsonDocumentid
                var businessData = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == insertUserDigitalDocRequest.Jsondocumentid);
                if (businessData == null)
                    throw new Exception("Business data not found.");

                var ClientVat_id = businessData.ClientVat_id;
                var SuplierVat_id = businessData.BusinessVatId;
                //var filterClientVat_id = Builders<BsonDocument>.Filter.Eq("taxId", ClientVat_id.ToString());
                //var projection = Builders<BsonDocument>.Projection.Include("name").Include("SubCompanyId").Exclude("_id");
                //var resultMorningCompanyInfo = _MorningCompanisInfoCollection.Find(filterClientVat_id).Project(projection).FirstOrDefault();

                string companyName = "";
                int SubCompanyIdClient = 0;
                int? dataSourceEnum = businessData.DataSourceEnum;
                int ExternalClientSystemId = 0;
                int InternalCompanyIdClient = 0;



                int SubCompanyIdSuplier = 0;
                int ExternalSupplierSystemId = 0;
                int InternalCompanyIdSupplier = 0;
                //adedd eyal logic for icount morning different
                //start suplier section
                var filterSuplier = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", SuplierVat_id.ToString());
                var icountCompany = await _IcountCompaniesInfoCollection.Find(filterSuplier).FirstOrDefaultAsync();
                if (icountCompany != null)
                {
                    ExternalSupplierSystemId = 2; // ICount
                }

                filterSuplier = Builders<BsonDocument>.Filter.Eq("taxId", SuplierVat_id.ToString());
                var morningCompany = await _MorningCompanisInfoCollection.Find(filterSuplier).FirstOrDefaultAsync();
                if (morningCompany != null)
                {
                    ExternalSupplierSystemId = 6; // Morning
                }
                var configsuplier = GetConfigByExternalSystemId(ExternalSupplierSystemId);

                if (configsuplier != null)
                {
                    var subCompanyIdFieldPath = configsuplier.SubCompanyFieldPath;
                    var internalCompanyIdFieldPath = configsuplier.CompanyFieldPath;
                    BsonValue subCompanyId = BsonNull.Value;
                    BsonValue internalCompanyId = BsonNull.Value;

                    if (icountCompany != null && icountCompany.TryGetValue("company_info", out BsonValue companyInfoValue) && companyInfoValue.IsBsonDocument)
                    {
                        var companyInfo = companyInfoValue.AsBsonDocument;

                        if (companyInfo.TryGetValue("SubCompanyId", out BsonValue subCompanyIdValue) && subCompanyIdValue.IsInt32)
                        {
                            subCompanyId = subCompanyIdValue;
                        }

                        if (companyInfo.TryGetValue("InternalCompanyId", out BsonValue internalCompanyIdValue) && internalCompanyIdValue.IsInt32)
                        {
                            internalCompanyId = internalCompanyIdValue;
                        }
                    }
                    else if (ExternalSupplierSystemId == 6 && morningCompany != null) // Morning System
                    {
                        subCompanyId = morningCompany.GetValue(subCompanyIdFieldPath, BsonNull.Value);
                        internalCompanyId = morningCompany.GetValue(internalCompanyIdFieldPath, BsonNull.Value);
                    }

                    if (subCompanyId.BsonType != BsonType.Null)
                    {
                        SubCompanyIdSuplier = subCompanyId.AsInt32;
                        Console.WriteLine($"Extracted SubCompanyId: {SubCompanyIdSuplier}");
                    }
                    else
                    {
                        Console.WriteLine("SubCompanyId not found or is null.");
                    }

                    if (internalCompanyId.BsonType != BsonType.Null)
                    {
                        InternalCompanyIdSupplier = internalCompanyId.AsInt32;
                        Console.WriteLine($"Extracted InternalCompanyId: {InternalCompanyIdSupplier}");
                    }
                    else
                    {
                        Console.WriteLine("InternalCompanyId not found or is null.");
                    }
                }


                //end  suplier section




                //start cleint section
                // Find client in iCount system
                var filterClient = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", ClientVat_id.ToString());
                var icountCompanyClient = await _IcountCompaniesInfoCollection.Find(filterClient).FirstOrDefaultAsync();
                if (icountCompanyClient != null)
                {
                    ExternalClientSystemId = 2; // iCount
                }

                // Find client in Morning system
                filterClient = Builders<BsonDocument>.Filter.Eq("taxId", ClientVat_id.ToString());
                var morningCompanyClient = await _MorningCompanisInfoCollection.Find(filterClient).FirstOrDefaultAsync();
                if (morningCompanyClient != null)
                {
                    ExternalClientSystemId = 6; // Morning
                }

                // Retrieve configuration based on the external system
                var configClient = GetConfigByExternalSystemId(ExternalClientSystemId);
                if (configClient != null)
                {
                    var subCompanyIdFieldPath = configClient.SubCompanyFieldPath;
                    var internalCompanyIdFieldPath = configClient.CompanyFieldPath;

                    BsonValue subCompanyId = BsonNull.Value;
                    BsonValue internalCompanyId = BsonNull.Value;

                    // Determine the appropriate source document
                    BsonDocument companyClient = ExternalClientSystemId switch
                    {
                        2 => icountCompanyClient,  // iCount system
                        6 => morningCompanyClient, // Morning system
                        _ => null
                    };

                    if (companyClient != null)
                    {
                        // try with morning  as Client  Directly try to retrieve the values from the root document
                        if (ExternalClientSystemId == 6)
                        {
                            if (companyClient.TryGetValue("SubCompanyId", out subCompanyId))
                            {
                                Console.WriteLine($"Successfully retrieved SubCompanyId: {subCompanyId}");
                            }
                            else
                            {
                                Console.WriteLine("SubCompanyId not found.");
                            }

                            if (companyClient.TryGetValue("InternalCompanyId", out internalCompanyId))
                            {
                                Console.WriteLine($"Successfully retrieved InternalCompanyId: {internalCompanyId}");
                            }
                            else
                            {
                                Console.WriteLine("InternalCompanyId not found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("companyClient is null.");
                        }


                        // Validate numeric conversion
                        if (!subCompanyId.IsBsonNull && subCompanyId.IsInt32)
                        {
                            SubCompanyIdClient = subCompanyId.AsInt32;
                            Console.WriteLine($"Extracted SubCompanyId: {SubCompanyIdClient}");
                        }
                        else
                        {
                            Console.WriteLine("SubCompanyId is not a valid numeric value or is null.");
                        }

                        if (!internalCompanyId.IsBsonNull && internalCompanyId.IsInt32)
                        {
                            InternalCompanyIdClient = internalCompanyId.AsInt32;
                            Console.WriteLine($"Extracted InternalCompanyId: {InternalCompanyIdClient}");
                        }
                        else
                        {
                            Console.WriteLine("InternalCompanyId is not a valid numeric value or is null.");
                        }
                    }

                    if (ExternalClientSystemId == 2)
                    {
                        //now try with icount as Client 
                        if (companyClient.TryGetValue("company_info", out var companyInfoValue) && companyInfoValue.IsBsonDocument)
                        {
                            var companyInfo = companyInfoValue.AsBsonDocument;

                            if (companyInfo.TryGetValue("SubCompanyId", out subCompanyId))
                            {
                                Console.WriteLine($"Successfully retrieved SubCompanyId: {subCompanyId}");
                                SubCompanyIdClient = subCompanyId.AsInt32;
                            }
                            else
                            {
                                Console.WriteLine("SubCompanyId not found.");
                            }

                            if (companyInfo.TryGetValue("InternalCompanyId", out internalCompanyId))
                            {
                                Console.WriteLine($"Successfully retrieved InternalCompanyId: {internalCompanyId}");
                                InternalCompanyIdClient = internalCompanyId.AsInt32;
                            }
                            else
                            {
                                Console.WriteLine("InternalCompanyId not found.");
                            }
                        }
                        else
                        {
                            Console.WriteLine("company_info not found or is not a document.");
                        }

                    }
                }


                //end  client section




                // Handle Morning System (dataSourceEnum == 6)
                if (ExternalClientSystemId == 6)//client is morning 
                {
                    int? Pymanttype = null; //מזומן
                    object greenInvoicePayload = null;
                    string greenInvoiceToken = await GetNewToken(InternalCompanyIdClient, SubCompanyIdClient, userId, 6);
                    //accordnig to the eliav doc check if suplier exist 
                    string supplierVatId = businessData.BusinessVatId;
                    var supplierList = await GetSupplierListMorning(greenInvoiceToken, InternalCompanyIdClient, SubCompanyIdClient, userId); // First call
                    var supplierItem = supplierList.FirstOrDefault(s => s.vat_id == Convert.ToInt32(supplierVatId));
                    int type = 0;

                    if (ExternalSupplierSystemId == 6)
                    {

                        var filtermorningwebhookdata = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(insertUserDigitalDocRequest.Jsondocumentid));
                        var doc = await _MorningWebHookData.Find(filtermorningwebhookdata).FirstOrDefaultAsync();
                        type = doc.GetValue("type").ToInt32();
                        if (supplierItem != null)
                        {


                            if (type == 400 || type == 320)
                            {
                                if (doc.Contains("transactions"))
                                {
                                    var transactions = doc["transactions"].AsBsonArray;
                                    string StrpaymentType = "";
                                    // Iterate through the transactions array
                                    foreach (var transaction in transactions)
                                    {
                                        var paymentMethod = transaction["paymentMethod"].AsBsonDocument;
                                        if (paymentMethod.Contains("type"))
                                        {
                                            StrpaymentType = paymentMethod["type"].AsString;

                                        }
                                    }
                                    switch (StrpaymentType)
                                    {
                                        case "cash":
                                            Pymanttype = 1;
                                            break;
                                        case "default":
                                            Pymanttype = 11;
                                            break;
                                        case "wire-transfer":
                                            Pymanttype = 4;
                                            break;
                                        case "credit-card":
                                            Pymanttype = 3;
                                            break;
                                        case "cheque":
                                            Pymanttype = 2;
                                            break;
                                        default:
                                            Pymanttype = 11;
                                            break;


                                    }

                                }
                            }
                            else { Pymanttype = -1; }

                            // Check if supplier_ID exists in _MorningExpenses //if we found suplierid in morning expenses it means this suplier is already
                            //registered in his client as suplier that has an expense or as an expense hat related to a suplier
                            var supplierFilter = Builders<BsonDocument>.Filter.And(
                                Builders<BsonDocument>.Filter.Eq("internalCompanyId", insertUserDigitalDocRequest.internalCompanyId),
                                Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyIdClient),
                                Builders<BsonDocument>.Filter.Eq("userId", userId),
                                Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>("results_list", Builders<BsonDocument>.Filter.Eq("Supplier._id", insertUserDigitalDocRequest.supplier_id))
                            );

                            var matchingExpense = await _MorningExpenses.Find(supplierFilter).FirstOrDefaultAsync();

                            if (matchingExpense != null)
                            {
                                // Check if expense_type_id matches AccountingClassification._id
                                var matchingClassification = matchingExpense["results_list"].AsBsonArray
                                    .FirstOrDefault(x => x["AccountingClassification"]?["id"].AsString == insertUserDigitalDocRequest.expense_type_id.ToString());

                                if (matchingClassification != null)
                                {
                                    // Add supplier to _MorningClientSuppliers
                                    var supplierDocument = new BsonDocument
                                {
                                    { "internalCompanyId", insertUserDigitalDocRequest.internalCompanyId },
                                    { "Client_SubCompanyid", SubCompanyIdClient },
                                    { "userId", userId },
                                    { "Suppliers", new BsonArray { new BsonDocument
                                        {
                                            { "id", insertUserDigitalDocRequest.supplier_id },
                                            { "Name", matchingClassification["Supplier"]["Name"].AsString }
                                        }}
                                    }
                                };

                                    var supplierFilterDoc = Builders<BsonDocument>.Filter.And(
                                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", insertUserDigitalDocRequest.internalCompanyId),
                                        Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyIdClient),
                                        Builders<BsonDocument>.Filter.Eq("userId", userId)
                                    );

                                    await _MorningClientSuppliers.UpdateOneAsync(supplierFilterDoc,
                                        Builders<BsonDocument>.Update.Set("Suppliers", supplierDocument["Suppliers"]),
                                        new UpdateOptions { IsUpsert = true });


                                    // Prepare the GreenInvoice payload
                                    greenInvoicePayload = new
                                    {
                                        paymentType = Pymanttype,
                                        currency = "ILS",
                                        currencyRate = 1,
                                        vat = insertUserDigitalDocRequest.expense_vat_sum,
                                        amount = insertUserDigitalDocRequest.expense_sum,
                                        date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                                        dueDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                                        reportingDate = DateTime.UtcNow.ToString("yyyy-MM-01"),
                                        documentType = type,
                                        number = insertUserDigitalDocRequest.expense_docnum,
                                        description = "הוצאה נירשמה דרך יונינט",
                                        remarks = "הוצאה נירשמה דרך יונינט",
                                        supplier = new
                                        {
                                            id = insertUserDigitalDocRequest.supplier_id,
                                            name = matchingClassification["Supplier"]["Name"].AsString,
                                            active = true,
                                            taxId = businessData.BusinessVatId.ToString(),
                                        },
                                        //thumbnail = "https://www.w3schools.com/w3images/lights.jpg",//GetThumbnailFromDocument(doc),
                                        accountingClassification = matchingClassification["AccountingClassification"].AsBsonDocument.ToDictionary(),
                                        addRecipient = false,//becuse this is not a new suplier we need to use false
                                        addAccountingClassification = false
                                    };

                                }
                                else
                                {

                                    var expenseTypeFilter = Builders<BsonDocument>.Filter.And(
                                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", insertUserDigitalDocRequest.internalCompanyId),
                                        Builders<BsonDocument>.Filter.Eq("userId", userId),
                                        Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyIdClient),
                                        Builders<BsonDocument>.Filter.ElemMatch("expense_types", Builders<BsonDocument>.Filter.Eq("ExpenseTypeId", insertUserDigitalDocRequest.expense_type_id))
                                    );

                                    var expenseTypeDocument = await _MorningExpensesTypes.Find(expenseTypeFilter).FirstOrDefaultAsync();

                                    if (expenseTypeDocument != null)
                                    {
                                        // **Added**: Extract `ExpenseTypeDesc` from `expense_types`
                                        var expenseType = expenseTypeDocument["expense_types"].AsBsonArray
                                            .FirstOrDefault(x => x["ExpenseTypeId"].AsString == insertUserDigitalDocRequest.expense_type_id);

                                        if (expenseType != null)
                                        {
                                            string expenseTypeDesc = expenseType["ExpenseTypeDesc"].AsString; // **Added**: Get ExpenseTypeDesc

                                            // **Added**: Create the new payload
                                            greenInvoicePayload = new
                                            {
                                                paymentType = Pymanttype,
                                                currency = "ILS",
                                                currencyRate = 1,
                                                vat = insertUserDigitalDocRequest.expense_vat_sum,
                                                amount = insertUserDigitalDocRequest.expense_sum,
                                                date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                                                dueDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                                                reportingDate = DateTime.UtcNow.ToString("yyyy-MM-01"),
                                                documentType = type,
                                                number = insertUserDigitalDocRequest.expense_docnum,
                                                description = "הוצאה נירשמה דרך יונינט",
                                                remarks = "הוצאה נירשמה דרך יונינט",
                                                supplier = new
                                                {
                                                    id = insertUserDigitalDocRequest.supplier_id,
                                                    name = supplierItem.supplier_name, // **Edited**: Use `supplier_name` from the supplier item
                                                    active = true,
                                                    taxId = businessData.BusinessVatId.ToString(),
                                                },
                                                // thumbnail = "https://www.w3schools.com/w3images/lights.jpg",//GetThumbnailFromDocument(doc),
                                                accountingClassification = new
                                                {
                                                    id = insertUserDigitalDocRequest.expense_type_id, // **Edited**: Use the provided ID
                                                    title = expenseTypeDesc, // **Added**: Use `ExpenseTypeDesc` from `_MorningExpensesTypes`
                                                    irsCode = 3055, // Example value, replace if dynamic mapping exists
                                                    vat = insertUserDigitalDocRequest.expense_vat_sum, // Use VAT from the request
                                                    active = true
                                                },
                                                addRecipient = false, // **Edited**: Not a new supplier
                                                addAccountingClassification = false // **Edited**: Add new accounting classification
                                            };
                                        }
                                        else
                                        {
                                            throw new Exception("Expense type not found in `_MorningExpensesTypes` collection.");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Matching document not found in `_MorningExpensesTypes`.");
                                    }



                                    if (insertUserDigitalDocRequest.expense_type_id.ToString().Contains("_temp")) // User added expense manually
                                    {

                                    }

                                }



                                try
                                {
                                    var greenInvoiceEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 41);
                                    var result = await SendToGreenInvoice(greenInvoiceEndpoint.Endpoint, greenInvoicePayload, greenInvoiceToken);

                                    response = JsonConvert.DeserializeObject<createExpenseApiResponse>(result);

                                    if (response?.status == true)
                                    {
                                        string expenseId = response.id;
                                        businessData.DocumentApprovedtoUninet = true;
                                        businessData.ExpenseTypeId = insertUserDigitalDocRequest.expense_type_id.ToString();
                                        await _repository.UpdateAsync(businessData);

                                        try
                                        {

                                            // Step 1: Retrieve the Upload URL
                                            var (uploadUrl, fields) = await GetUploadUrl(greenInvoiceToken, expenseId);

                                            // Step 2: Fetch the Document Data from MongoDB
                                            byte[] documentData = await FetchDocumentData(insertUserDigitalDocRequest.Jsondocumentid, ExternalSupplierSystemId);

                                            // Step 3: Upload the Document to the Morning System
                                            string fileName = "uploaded_file.pdf";
                                            var uploadResult = await UploadFileToUrl(uploadUrl, documentData, fileName, fields);

                                            Console.WriteLine($"Document uploaded successfully: {uploadResult}");

                                        }
                                        catch (Exception uploadEx)
                                        {
                                            Console.WriteLine($"Failed to upload document: {uploadEx.Message}");
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        // Fetch error message from MongoDB if available
                                        string errorMessage = response?.reason;
                                        if (int.TryParse(response?.reason, out int errorCode))
                                        {
                                            errorMessage = await GetErrorMessageFromMongo(errorCode);
                                        }

                                        return new createExpenseApiResponse
                                        {
                                            status = false,
                                            reason = errorMessage ?? "Failed to send data to GreenInvoice."
                                        };
                                    }
                                }
                                catch (HttpRequestException ex)
                                {
                                    string errorMessage = ex.Data.Contains("errorCode") && ex.Data.Contains("errorMessage")
                                        ? await GetErrorMessageFromMongo((int)ex.Data["errorCode"])
                                        : "An error occurred while sending data to GreenInvoice.";

                                    return new createExpenseApiResponse
                                    {
                                        status = false,
                                        reason = errorMessage
                                    };
                                }
                            }
                        }
                        else //we didnt find in the supliers  list (of the current client) that came back from mongo any suplier that matches the current suplier of this document
                        {
                            greenInvoicePayload = new
                            {
                                paymentType = 2,
                                currency = "ILS",
                                currencyRate = 1,
                                vat = insertUserDigitalDocRequest.expense_vat_sum,
                                amount = insertUserDigitalDocRequest.expense_sum,
                                date = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                                dueDate = DateTime.UtcNow.ToString("yyyy-MM-dd"),
                                reportingDate = DateTime.UtcNow.ToString("yyyy-MM-01"),
                                documentType = 20,
                                number = insertUserDigitalDocRequest.expense_docnum,
                                description = "הוצאה נירשמה דרך יונינט",
                                remarks = "הוצאה נירשמה דרך יונינט",
                                //thumbnail = "https://www.w3schools.com/w3images/lights.jpg",// GetThumbnailFromDocument(doc),
                                accountingClassification = new
                                {
                                    id = insertUserDigitalDocRequest.expense_type_id,
                                    title = "Expense Classification Title", // Update with dynamic classification title if needed
                                    irsCode = 3055,
                                    vat = insertUserDigitalDocRequest.expense_vat_sum
                                },
                                addRecipient = true, // the morning system will add their own suplierid guid
                                addAccountingClassification = false
                            };

                            try
                            {
                                var greenInvoiceEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 41);
                                var result = await SendToGreenInvoice(greenInvoiceEndpoint.Endpoint, greenInvoicePayload, greenInvoiceToken);

                                response = JsonConvert.DeserializeObject<createExpenseApiResponse>(result);

                                if (response?.status == true)
                                {
                                    string expenseId = response.id;
                                    businessData.DocumentApprovedtoUninet = true;
                                    businessData.ExpenseTypeId = insertUserDigitalDocRequest.expense_type_id.ToString();
                                    await _repository.UpdateAsync(businessData);

                                    try
                                    {


                                        // Step 1: Retrieve the Upload URL
                                        var (uploadUrl, fields) = await GetUploadUrl(greenInvoiceToken, expenseId);

                                        // Step 2: Fetch the Document Data from MongoDB
                                        byte[] documentData = await FetchDocumentData(insertUserDigitalDocRequest.Jsondocumentid, ExternalSupplierSystemId);

                                        // Step 3: Upload the Document to the Morning System
                                        string fileName = "uploaded_file.pdf";
                                        var uploadResult = await UploadFileToUrl(uploadUrl, documentData, fileName, fields);

                                        Console.WriteLine($"Document uploaded successfully: {uploadResult}");



                                    }
                                    catch (Exception uploadEx)
                                    {
                                        Console.WriteLine($"Failed to upload document: {uploadEx.Message}");
                                        throw;
                                    }
                                }
                                else
                                {
                                    // Fetch error message from MongoDB if available
                                    string errorMessage = response?.reason;
                                    if (int.TryParse(response?.reason, out int errorCode))
                                    {
                                        errorMessage = await GetErrorMessageFromMongo(errorCode);
                                    }

                                    return new createExpenseApiResponse
                                    {
                                        status = false,
                                        reason = errorMessage ?? "Failed to send data to GreenInvoice."
                                    };
                                }
                            }
                            catch (HttpRequestException ex)
                            {
                                string errorMessage = ex.Data.Contains("errorCode") && ex.Data.Contains("errorMessage")
                                    ? await GetErrorMessageFromMongo((int)ex.Data["errorCode"])
                                    : "An error occurred while sending data to GreenInvoice.";

                                return new createExpenseApiResponse
                                {
                                    status = false,
                                    reason = errorMessage
                                };
                            }
                        }
                    }//suplier is morning 
                    if (ExternalSupplierSystemId == 2)
                    {
                        var filterIcountWebookdata = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(insertUserDigitalDocRequest.Jsondocumentid));
                        var doc = await _IcountWebhookData.Find(filterIcountWebookdata).FirstOrDefaultAsync();

                        if (doc == null)
                        {
                            throw new Exception("Document not found in Icount Webhook Data.");
                        }

                        string doctype = doc.Contains("doctype") ? doc["doctype"].AsString : "unknown";

                        // Extract document issue date
                        string documentDate = (doc.Contains("doc_info") && doc["doc_info"].IsBsonDocument && doc["doc_info"].AsBsonDocument.Contains("dateissued"))
                            ? DateTime.Parse(doc["doc_info"]["dateissued"].AsString).ToString("yyyy-MM-dd")
                            : DateTime.UtcNow.ToString("yyyy-MM-dd");

                        // Default paymentDate to documentDate (fallback)
                        string paymentDate = documentDate;

                        // Ensure doc_info exists before accessing its fields
                        // Extract doc_info
                        if (!doc.Contains("doc_info") || !doc["doc_info"].IsBsonDocument)
                        {
                            throw new Exception("doc_info is missing or invalid in the document.");
                        }

                        var docInfo = doc["doc_info"].AsBsonDocument;

                        // Extract document issue date
                       

                        // 1) PayPal Payment Date
                        if (docInfo.Contains("has_pp") && docInfo["has_pp"].ToBoolean() &&
                            docInfo.Contains("paypal") && docInfo["paypal"].IsBsonDocument &&
                            docInfo["paypal"].AsBsonDocument.Contains("payment_date"))
                        {
                            string rawPaymentDate = docInfo["paypal"]["payment_date"].AsString;
                            if (!string.IsNullOrWhiteSpace(rawPaymentDate))
                            {
                                paymentDate = DateTime.Parse(rawPaymentDate).ToString("yyyy-MM-dd");
                            }
                        }

                        // 2) Cheque Payment Date
                        else if (docInfo.Contains("has_cheques") && docInfo["has_cheques"].ToBoolean() &&
                                 docInfo.Contains("cheques") && docInfo["cheques"].IsBsonArray && docInfo["cheques"].AsBsonArray.Count > 0)
                        {
                            var cheque = docInfo["cheques"].AsBsonArray.FirstOrDefault();
                            if (cheque != null && cheque.IsBsonDocument && cheque.AsBsonDocument.Contains("chequeDate"))
                            {
                                string rawChequeDate = cheque["chequeDate"].AsString;
                                if (!string.IsNullOrWhiteSpace(rawChequeDate))
                                {
                                    paymentDate = DateTime.Parse(rawChequeDate).ToString("yyyy-MM-dd");
                                }
                            }
                        }

                        // 3) Bank Transfer Payment Date
                        else if (docInfo.Contains("has_banktransfer") && docInfo["has_banktransfer"].ToBoolean() &&
                                 docInfo.Contains("banktransfer") && docInfo["banktransfer"].IsBsonDocument &&
                                 docInfo["banktransfer"].AsBsonDocument.Contains("peraonDate"))
                        {
                            string rawBankTransferDate = docInfo["banktransfer"]["peraonDate"].AsString;
                            if (!string.IsNullOrWhiteSpace(rawBankTransferDate))
                            {
                                paymentDate = DateTime.Parse(rawBankTransferDate).ToString("yyyy-MM-dd");
                            }
                        }




                        // Instead of checking for a non-existent "transactions" array, extract payment info from "doc_info"
                        Pymanttype = -1; // default
                                         // Extract payment info from doc_info
                        if (doc.Contains("doc_info") && doc["doc_info"].IsBsonDocument)
                        {


                            // 1) Cash
                            if (docInfo.Contains("has_cash") && docInfo["has_cash"].BsonType == BsonType.Boolean && docInfo["has_cash"].ToBoolean())
                            {
                                Pymanttype = 1; // Cash
                            }
                            // 2) Cheque
                            else if (docInfo.Contains("has_cheques") && docInfo["has_cheques"].BsonType == BsonType.Boolean && docInfo["has_cheques"].ToBoolean())
                            {
                                Pymanttype = 2; // Cheque
                            }
                            // 3) Credit Card
                            else if (docInfo.Contains("has_cc") && docInfo["has_cc"].BsonType == BsonType.Boolean && docInfo["has_cc"].ToBoolean())
                            {
                                Pymanttype = 3; // Credit card
                            }
                            // 4) Wire/Bank Transfer (Fixed to check explicitly for "1" as a string)
                            else if (docInfo.Contains("has_bt") && docInfo["has_bt"].BsonType == BsonType.String && docInfo["has_bt"].AsString == "1")
                            {
                                Pymanttype = 4; // Bank/wire transfer
                            }
                            // 5) PayPal (Fixed to check explicitly for "1" as a string)
                            else if (docInfo.Contains("has_pp") && docInfo["has_pp"].BsonType == BsonType.String && docInfo["has_pp"].AsString == "1")
                            {
                                Pymanttype = 5; // PayPal
                            }
                            // 6) HK (Unknown mapping, fixed to check explicitly for "1" as a string)
                            else if (docInfo.Contains("has_hk") && docInfo["has_hk"].BsonType == BsonType.String && docInfo["has_hk"].AsString == "1")
                            {
                                Pymanttype = 11; // Other/Unknown
                            }
                            // 7) Barter (Fixed to check explicitly for "1" as a string)
                            else if (docInfo.Contains("has_barter") && docInfo["has_barter"].BsonType == BsonType.String && docInfo["has_barter"].AsString == "1")
                            {
                                Pymanttype = 11; // Other
                            }
                            // 8) Payment App (Bit, Pepper, etc.) - Found in `payments`, moved outside `doc_info`
                            else if (docInfo.Contains("payments") && docInfo["payments"].IsBsonDocument)
                            {
                                var payments = docInfo["payments"].AsBsonDocument;

                                if (payments.Contains("has_payment_app") && payments["has_payment_app"].BsonType == BsonType.Boolean && payments["has_payment_app"].ToBoolean())
                                {
                                    Pymanttype = 10; // Payment App (Bit, Pepper, etc.)
                                }
                            }
                            else
                            {
                                // No recognized method => remain "unpaid" or treat as "other"
                                Pymanttype = -1;
                            }
                        }



                        if (supplierItem != null)
                        {
                            switch (insertUserDigitalDocRequest.expense_doctype.ToLower())
                            {
                                case "invrec":
                                    type = 320; // חשבונית מס / קבלה
                                    break;
                                case "receipt":
                                    type = 400; // קבלה
                                    break;
                                case "invoice":
                                    type = 305; // חשבונית מס
                                    break;
                                case "deal":
                                    type = 10;  // הצעת מחיר
                                    break;
                                case "order":
                                    type = 100; // הזמנה
                                    break;
                                case "refund":
                                    type = 330; // חשבונית זיכוי
                                    break;
                                case "delcert":
                                    type = 200; // תעודת משלוח
                                    break;
                                case "supplier":
                                    type = 305; // חשבונית מס (supplier invoice should be a tax invoice)
                                    break;
                                default:
                                    throw new Exception("Unknown document type for Morning API.");
                            }


                            // Check if supplier_ID exists in _MorningExpenses //if we found suplierid in morning expenses it means this suplier is already
                            //registered in his client as suplier that has an expense or as an expense hat related to a suplier
                            var supplierFilter = Builders<BsonDocument>.Filter.And(
                                Builders<BsonDocument>.Filter.Eq("internalCompanyId", InternalCompanyIdClient),
                                Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyIdClient),
                                Builders<BsonDocument>.Filter.Eq("userId", userId),
                                Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>("results_list", Builders<BsonDocument>.Filter.Eq("Supplier._id", insertUserDigitalDocRequest.supplier_id))
                            );

                            var matchingExpense = await _MorningExpenses.Find(supplierFilter).FirstOrDefaultAsync();
                            float? vatsum = insertUserDigitalDocRequest.expense_vat_sum > 0
                            ? insertUserDigitalDocRequest.expense_vat_sum
                            : (docInfo.Contains("totalvat") && float.TryParse(docInfo["totalvat"].ToString(), out float parsedVat)
                            ? parsedVat
                            : insertUserDigitalDocRequest.expense_sum - (insertUserDigitalDocRequest.expense_sum / 1.18f));



                            if (matchingExpense != null)
                            {
                                // Check if expense_type_id matches AccountingClassification._id
                                var matchingClassification = matchingExpense["results_list"].AsBsonArray
                                    .FirstOrDefault(x => x["AccountingClassification"]?["id"].AsString == insertUserDigitalDocRequest.expense_type_id.ToString());

                                if (matchingClassification != null)
                                {
                                    // Add supplier to _MorningClientSuppliers
                                    var supplierDocument = new BsonDocument
                                {
                                    { "internalCompanyId", InternalCompanyIdClient },
                                    { "Client_SubCompanyid", SubCompanyIdClient },
                                    { "userId", userId },
                                    { "Suppliers", new BsonArray { new BsonDocument
                                        {
                                            { "id", insertUserDigitalDocRequest.supplier_id },
                                            { "Name", matchingClassification["Supplier"]["Name"].AsString }
                                        }}
                                    }
                                };

                                    var supplierFilterDoc = Builders<BsonDocument>.Filter.And(
                                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", InternalCompanyIdClient),
                                        Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyIdClient),
                                        Builders<BsonDocument>.Filter.Eq("userId", userId)
                                    );

                                    await _MorningClientSuppliers.UpdateOneAsync(supplierFilterDoc,
                                        Builders<BsonDocument>.Update.Set("Suppliers", supplierDocument["Suppliers"]),
                                        new UpdateOptions { IsUpsert = true });

                                  
                                    // Prepare the GreenInvoice payload
                                    greenInvoicePayload = new
                                    {
                                        paymentType = Pymanttype,
                                        currency = "ILS",
                                        currencyRate = 1,
                                        vat = vatsum,
                                        amount = insertUserDigitalDocRequest.expense_sum,
                                        date = documentDate,
                                        dueDate = paymentDate,  // Use payment date for due date if available
                                        reportingDate = DateTime.Parse(documentDate).ToString("yyyy-MM-01"),
                                        documentType = type,
                                        number = insertUserDigitalDocRequest.expense_docnum,
                                        description = "הוצאה נירשמה דרך יונינט",
                                        remarks = "הוצאה נירשמה דרך יונינט",
                                        supplier = new
                                        {
                                            id = insertUserDigitalDocRequest.supplier_id,
                                            name = matchingClassification["Supplier"]["Name"].AsString,
                                            active = true,
                                            taxId = businessData.BusinessVatId.ToString(),
                                        },
                                        //thumbnail = "https://www.w3schools.com/w3images/lights.jpg",//GetThumbnailFromDocument(doc),
                                        accountingClassification = matchingClassification["AccountingClassification"].AsBsonDocument.ToDictionary(),
                                        addRecipient = false,//becuse this is not a new suplier we need to use false
                                        addAccountingClassification = false
                                    };

                                }
                                else
                                {

                                    var expenseTypeFilter = Builders<BsonDocument>.Filter.And(
                                        Builders<BsonDocument>.Filter.Eq("internalCompanyId", InternalCompanyIdClient),
                                        Builders<BsonDocument>.Filter.Eq("userId", userId),
                                        Builders<BsonDocument>.Filter.Eq("Client_SubCompanyid", SubCompanyIdClient),
                                        Builders<BsonDocument>.Filter.ElemMatch("expense_types", Builders<BsonDocument>.Filter.Eq("ExpenseTypeId", insertUserDigitalDocRequest.expense_type_id))
                                    );

                                    var expenseTypeDocument = await _MorningExpensesTypes.Find(expenseTypeFilter).FirstOrDefaultAsync();

                                    if (expenseTypeDocument != null)
                                    {
                                        // **Added**: Extract `ExpenseTypeDesc` from `expense_types`
                                        var expenseType = expenseTypeDocument["expense_types"].AsBsonArray
                                            .FirstOrDefault(x => x["ExpenseTypeId"].AsString == insertUserDigitalDocRequest.expense_type_id);

                                        if (expenseType != null)
                                        {
                                            string expenseTypeDesc = expenseType["ExpenseTypeDesc"].AsString; // **Added**: Get ExpenseTypeDesc

                                            // **Added**: Create the new payload
                                            greenInvoicePayload = new
                                            {
                                                paymentType = Pymanttype,
                                                currency = "ILS",
                                                currencyRate = 1,
                                                vat = vatsum,
                                                amount = insertUserDigitalDocRequest.expense_sum,
                                                date = documentDate,
                                                dueDate = paymentDate,  // Use payment date for due date if available
                                                reportingDate = DateTime.Parse(documentDate).ToString("yyyy-MM-01"),
                                                documentType = type,
                                                number = insertUserDigitalDocRequest.expense_docnum,
                                                description = "הוצאה נירשמה דרך יונינט",
                                                remarks = "הוצאה נירשמה דרך יונינט",
                                                supplier = new
                                                {
                                                    id = insertUserDigitalDocRequest.supplier_id,
                                                    name = supplierItem.supplier_name, // **Edited**: Use `supplier_name` from the supplier item
                                                    active = true,
                                                    taxId = businessData.BusinessVatId.ToString(),
                                                },
                                                // thumbnail = "https://www.w3schools.com/w3images/lights.jpg",//GetThumbnailFromDocument(doc),
                                                accountingClassification = new
                                                {
                                                    id = insertUserDigitalDocRequest.expense_type_id, // **Edited**: Use the provided ID
                                                    title = expenseTypeDesc, // **Added**: Use `ExpenseTypeDesc` from `_MorningExpensesTypes`
                                                    irsCode = 3055, // Example value, replace if dynamic mapping exists
                                                    vat = insertUserDigitalDocRequest.expense_vat_sum, // Use VAT from the request
                                                    active = true
                                                },
                                                addRecipient = false, // **Edited**: Not a new supplier
                                                addAccountingClassification = false // **Edited**: Add new accounting classification
                                            };
                                        }
                                        else
                                        {
                                            throw new Exception("Expense type not found in `_MorningExpensesTypes` collection.");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Matching document not found in `_MorningExpensesTypes`.");
                                    }



                                    if (insertUserDigitalDocRequest.expense_type_id.ToString().Contains("_temp")) // User added expense manually
                                    {

                                    }

                                }



                                try
                                {
                                    var greenInvoiceEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 41);
                                    var result = await SendToGreenInvoice(greenInvoiceEndpoint.Endpoint, greenInvoicePayload, greenInvoiceToken);

                                    response = JsonConvert.DeserializeObject<createExpenseApiResponse>(result);

                                    if (response?.status == true)
                                    {
                                        string expenseId = response.id;
                                        businessData.DocumentApprovedtoUninet = true;
                                        businessData.ExpenseTypeId = insertUserDigitalDocRequest.expense_type_id.ToString();
                                        await _repository.UpdateAsync(businessData);

                                        try
                                        {

                                            // Step 1: Retrieve the Upload URL
                                            var (uploadUrl, fields) = await GetUploadUrl(greenInvoiceToken, expenseId);

                                            // Step 2: Fetch the Document Data from MongoDB
                                            byte[] documentData = await FetchDocumentData(insertUserDigitalDocRequest.Jsondocumentid, ExternalSupplierSystemId);

                                            // Step 3: Upload the Document to the Morning System
                                            string fileName = "uploaded_file.pdf";
                                            var uploadResult = await UploadFileToUrl(uploadUrl, documentData, fileName, fields);

                                            Console.WriteLine($"Document uploaded successfully: {uploadResult}");

                                        }
                                        catch (Exception uploadEx)
                                        {
                                            Console.WriteLine($"Failed to upload document: {uploadEx.Message}");
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        // Fetch error message from MongoDB if available
                                        string errorMessage = response?.reason;
                                        if (int.TryParse(response?.reason, out int errorCode))
                                        {
                                            errorMessage = await GetErrorMessageFromMongo(errorCode);
                                        }

                                        return new createExpenseApiResponse
                                        {
                                            status = false,
                                            reason = errorMessage ?? "Failed to send data to GreenInvoice."
                                        };
                                    }
                                }
                                catch (HttpRequestException ex)
                                {
                                    string errorMessage = ex.Data.Contains("errorCode") && ex.Data.Contains("errorMessage")
                                        ? await GetErrorMessageFromMongo((int)ex.Data["errorCode"])
                                        : "An error occurred while sending data to GreenInvoice.";

                                    return new createExpenseApiResponse
                                    {
                                        status = false,
                                        reason = errorMessage
                                    };
                                }
                            }
                            else //we didnt find in the supliers  list (of the current client) that came back from mongo any suplier that matches the current suplier of this document
                            {
                                greenInvoicePayload = new
                                {
                                    paymentType = 2,
                                    currency = "ILS",
                                    currencyRate = 1,
                                    vat = vatsum,
                                    amount = insertUserDigitalDocRequest.expense_sum,
                                    date = documentDate,
                                    dueDate = paymentDate,  // Use payment date for due date if available
                                    reportingDate = DateTime.Parse(documentDate).ToString("yyyy-MM-01"),
                                    documentType = 20,
                                    number = insertUserDigitalDocRequest.expense_docnum,
                                    description = "הוצאה נירשמה דרך יונינט",
                                    remarks = "הוצאה נירשמה דרך יונינט",
                                    //thumbnail = "https://www.w3schools.com/w3images/lights.jpg",// GetThumbnailFromDocument(doc),
                                    supplier = new
                                    {
                                        id = insertUserDigitalDocRequest.supplier_id,
                                        name = supplierItem.supplier_name, // **Edited**: Use `supplier_name` from the supplier item
                                        active = true,
                                        taxId = businessData.BusinessVatId.ToString(),
                                    },
                                    accountingClassification = new
                                    {
                                        id = insertUserDigitalDocRequest.expense_type_id,
                                        title = "Expense Classification Title", // Update with dynamic classification title if needed
                                        irsCode = 3055,
                                        vat = insertUserDigitalDocRequest.expense_vat_sum
                                    },
                                    addRecipient = true, // the morning system will add their own suplierid guid
                                    addAccountingClassification = false
                                };

                                try
                                {
                                    var greenInvoiceEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 41);
                                    var result = await SendToGreenInvoice(greenInvoiceEndpoint.Endpoint, greenInvoicePayload, greenInvoiceToken);

                                    response = JsonConvert.DeserializeObject<createExpenseApiResponse>(result);

                                    if (response?.status == true)
                                    {
                                        string expenseId = response.id;
                                        businessData.DocumentApprovedtoUninet = true;
                                        businessData.ExpenseTypeId = insertUserDigitalDocRequest.expense_type_id.ToString();
                                        await _repository.UpdateAsync(businessData);

                                        try
                                        {
                                            //remark eyal  since this is a code to bring the document and to upload it to the system 
                                            //we are here in morning  client andicount suplier so we need to get doc from icount webbhook and 
                                            //than upload it to morning account as expnse





                                            //// Step 1: Retrieve the Upload URL
                                            var (uploadUrl, fields) = await GetUploadUrl(greenInvoiceToken, expenseId);

                                            //// Step 2: Fetch the Document Data from MongoDB
                                            var config = GetConfigByExternalSystemId(ExternalSupplierSystemId);
                                            string JsonDocUrl = await FetchJsonDocUrl(insertUserDigitalDocRequest.Jsondocumentid, config);
                                            byte[] documentData = await DownloadFileAsByteArray(JsonDocUrl);//await FetchDocumentData(insertUserDigitalDocRequest.Jsondocumentid);

                                            //// Step 3: Upload the Document to the Morning System
                                            string fileName = "uploaded_file.pdf";
                                            var uploadResult = await UploadFileToUrl(uploadUrl, documentData, fileName, fields);

                                            //Console.WriteLine($"Document uploaded successfully: {uploadResult}");



                                        }
                                        catch (Exception uploadEx)
                                        {
                                            Console.WriteLine($"Failed to upload document: {uploadEx.Message}");
                                            throw;
                                        }
                                    }
                                    else
                                    {
                                        // Fetch error message from MongoDB if available
                                        string errorMessage = response?.reason;
                                        if (int.TryParse(response?.reason, out int errorCode))
                                        {
                                            errorMessage = await GetErrorMessageFromMongo(errorCode);
                                        }

                                        return new createExpenseApiResponse
                                        {
                                            status = false,
                                            reason = errorMessage ?? "Failed to send data to GreenInvoice."
                                        };
                                    }
                                }
                                catch (HttpRequestException ex)
                                {
                                    string errorMessage = ex.Data.Contains("errorCode") && ex.Data.Contains("errorMessage")
                                        ? await GetErrorMessageFromMongo((int)ex.Data["errorCode"])
                                        : "An error occurred while sending data to GreenInvoice.";

                                    return new createExpenseApiResponse
                                    {
                                        status = false,
                                        reason = errorMessage
                                    };
                                }
                            }
                        }

                    }//suplier is icount 


                }


              
                if (ExternalClientSystemId == 2)//client is Icount 
                {

                    if (ExternalSupplierSystemId == 6)//suplier morning
                    {
                        double vatValue = 0;
                        string docDate = "";
                        string paidDate = "";
                        int type = 0;
                        UserexternalSystemDynamicFieldslist = await GetUserDynamicFields(userId, InternalCompanyIdClient, SubCompanyIdClient);
                        string base64Data = "";
                        string cidValue = UserexternalSystemDynamicFieldslist?.FirstOrDefault(x => x.FieldLabelName == "cid")?.FieldLabelValue;
                        string userValue = UserexternalSystemDynamicFieldslist?.FirstOrDefault(x => x.FieldLabelName == "user")?.FieldLabelValue;
                        string passValue = UserexternalSystemDynamicFieldslist?.FirstOrDefault(x => x.FieldLabelName == "pass")?.FieldLabelValue;

                        if (string.IsNullOrEmpty(cidValue) || string.IsNullOrEmpty(userValue) || string.IsNullOrEmpty(passValue))
                            throw new Exception("Missing dynamic field values for Icount.");

                        var ExpenseCreateEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 79);
                        string endpointExpenseCreate = $"{ExpenseCreateEndpoint.Endpoint}?cid={cidValue}&user={userValue}&pass={passValue}";
                        
                        //because the suplier is morning we need to get the document from morning collection
                        var filtermorningwebhookdata = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(insertUserDigitalDocRequest.Jsondocumentid));
                        var Morningwebhookdoc = await _MorningWebHookData.Find(filtermorningwebhookdata).FirstOrDefaultAsync();
                        
                        if (Morningwebhookdoc != null)
                        {
                            

                            // Add VAT if exists in the document
                            if (Morningwebhookdoc.Contains("tax") && Morningwebhookdoc["tax"].IsBsonArray && Morningwebhookdoc["tax"].AsBsonArray.Count > 0)
                            {
                                 vatValue = Morningwebhookdoc["tax"][0]["total"].ToDouble();
                                
                            }


                            string processedUrl = "";
                             docDate = Morningwebhookdoc["date"].AsString;
                             paidDate = docDate; // fallback

                            // If there's a transactions array with a date, use that as the paid date
                            if (Morningwebhookdoc.Contains("transactions")
                                && Morningwebhookdoc["transactions"].IsBsonArray
                                && Morningwebhookdoc["transactions"].AsBsonArray.Count > 0)
                            {
                                paidDate = Morningwebhookdoc["transactions"][0]["date"].AsString;
                            }


                            doc_url_copy =await FetchJsonDocUrl(insertUserDigitalDocRequest.Jsondocumentid, configsuplier);
                            base64Data = doc_url_copy.Replace("data:application/pdf;base64,", "");

                        }
                        // Retrieve the type value from the webhook
                        // Mapping from your numeric type to iCount’s doctype key
                        var numericToDoctypeKey = new Dictionary<int, string>
                        {
                            {320,"invrec" },//חשבונית מס / קבלה
                            { 305, "invoice" },  // חשבונית מס
                            { 400, "receipt" },  // קבלה
                            { 300, "deal" },     // חשבון עסקה
                            { 10,  "offer" },    // הצעת מחיר
                            { 100, "order" },    // הזמנה
                            { 210, "refund" },   // תעודת החזרה (or חשבונית זיכוי as defined by iCount)
                            { 200, "delcert" }   // תעודת משלוח
                        };

                        // Retrieve the numeric type from your webhook
                         type = Morningwebhookdoc.GetValue("type").ToInt32();

                        // Map the numeric type to the iCount doctype key.
                        // If the type isn't found, fallback to the original value.
                        string expenseDocTypeKey = numericToDoctypeKey.ContainsKey(type)
                            ? numericToDoctypeKey[type]
                            : insertUserDigitalDocRequest.expense_doctype;

                        // Optionally, determine if the expense is paid (for example, receipts are marked as paid)
                        payed = (type == 400 || type == 320);

                        using (var httpClient = new HttpClient())
                        {
                            using (var content = new MultipartFormDataContent())
                            {
                                content.Add(new StringContent(insertUserDigitalDocRequest.supplier_id.ToString()), "supplier_id");
                                content.Add(new StringContent(insertUserDigitalDocRequest.expense_type_id.ToString()), "expense_type_id");
                                // Use the mapped doctype key (in English) directly
                                content.Add(new StringContent(expenseDocTypeKey), "expense_doctype");
                                content.Add(new StringContent(insertUserDigitalDocRequest.expense_docnum.ToString()), "expense_docnum");
                                content.Add(new StringContent(insertUserDigitalDocRequest.expense_sum.ToString()), "expense_sum");
                               
                                // expense_date, invoice_date, vat_date = doc date
                                content.Add(new StringContent(paidDate), "expense_date");
                                content.Add(new StringContent(docDate), "invoice_date");
                                content.Add(new StringContent(docDate), "vat_date");

                                // expense_paid_date = the payment date from transactions
                                content.Add(new StringContent(paidDate), "expense_paid_date");
                                content.Add(new StringContent(payed.ToString().ToLower()), "expense_paid");
                                
                                
                                content.Add(new StringContent("ההוצאה נרשמה דרך יונינט"), "comment");
                               

                                if (!string.IsNullOrEmpty(base64Data))
                                {
                                    byte[] pdfBytes = Convert.FromBase64String(base64Data);
                                    var pdfContent = new ByteArrayContent(pdfBytes);
                                    pdfContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/pdf");
                                    content.Add(pdfContent, "scan", "scan.pdf");
                                }

                                var endpointExpenseCreate_Response = await httpClient.PostAsync(endpointExpenseCreate, content);
                                result_endpointIcountExpenseCreate = await endpointExpenseCreate_Response.Content.ReadAsStringAsync();
                            }
                        }




                        response = JsonConvert.DeserializeObject<createExpenseApiResponse>(result_endpointIcountExpenseCreate);
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




                    }


                    if (ExternalSupplierSystemId == 2)//suplier is icount 
                    {

                        UserexternalSystemDynamicFieldslist = await GetUserDynamicFields(userId, InternalCompanyIdClient, SubCompanyIdClient);

                        string cidValue = UserexternalSystemDynamicFieldslist?.FirstOrDefault(x => x.FieldLabelName == "cid")?.FieldLabelValue;
                        string userValue = UserexternalSystemDynamicFieldslist?.FirstOrDefault(x => x.FieldLabelName == "user")?.FieldLabelValue;
                        string passValue = UserexternalSystemDynamicFieldslist?.FirstOrDefault(x => x.FieldLabelName == "pass")?.FieldLabelValue;

                        if (string.IsNullOrEmpty(cidValue) || string.IsNullOrEmpty(userValue) || string.IsNullOrEmpty(passValue))
                            throw new Exception("Missing dynamic field values for Icount.");

                        var ExpenseCreateEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 79);
                        string endpointExpenseCreate = $"{ExpenseCreateEndpoint.Endpoint}?cid={cidValue}&user={userValue}&pass={passValue}";

                        // Prepare and send data to Icount system
                        var filter = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse(insertUserDigitalDocRequest.Jsondocumentid));
                        var webhookdoc = _IcountWebhookData.Find(filter).FirstOrDefault();

                        if (webhookdoc != null)
                        {

                            dateissued = webhookdoc["doc_info"]["dateissued"].AsString;
                            doc_url_copy = webhookdoc["doc_info"]["doc_url_copy"].AsString;
                            


                        }

                        if (insertUserDigitalDocRequest.expense_doctype == "invrec" || insertUserDigitalDocRequest.expense_doctype == "receipt")
                        {
                            var docInfo = webhookdoc["doc_info"].AsBsonDocument;
                            string expensePaidDate = docInfo.Contains("dateissued") ? docInfo["dateissued"].AsString : "";
                            string invoiceDate = expensePaidDate; // Default to dateissued

                            // Determine payment method
                            if (docInfo.Contains("has_pp") && docInfo["has_pp"].ToString() == "1" &&
                                docInfo.Contains("paypal") && !docInfo["paypal"].IsBsonNull)
                            {
                                // PayPal Payment
                                var paypalInfo = docInfo["paypal"].AsBsonDocument;
                                expensePaidDate = paypalInfo.Contains("payment_date") ? paypalInfo["payment_date"].AsString : expensePaidDate;
                                invoiceDate = paypalInfo.Contains("asmachtaDate") ? paypalInfo["asmachtaDate"].AsString : invoiceDate;
                            }
                            else if (docInfo.Contains("payments") && docInfo["payments"].AsBsonDocument.Contains("has_payment_app") &&
                                     docInfo["payments"]["has_payment_app"].ToString() == "1")
                            {
                                // Application Payment (e.g., Bit)
                                var paymentApp = docInfo["payments"]["payment_app"].AsBsonDocument;
                                expensePaidDate = paymentApp.Contains("payment_date") && !paymentApp["payment_date"].IsBsonNull
                                    ? paymentApp["payment_date"].AsString
                                    : expensePaidDate;
                            }
                            else if (docInfo.Contains("has_banktransfer") && docInfo["has_banktransfer"].ToString() == "true" &&
                                     docInfo.Contains("banktransfer") && !docInfo["banktransfer"].IsBsonNull)
                            {
                                // Bank Transfer Payment
                                var bankTransfer = docInfo["banktransfer"].AsBsonDocument;
                                expensePaidDate = bankTransfer.Contains("peraonDate") ? bankTransfer["peraonDate"].AsString : expensePaidDate;
                            }
                            else if (docInfo.Contains("has_cc") && docInfo["has_cc"].ToString() == "1" &&
                                     docInfo.Contains("cc") && !docInfo["cc"].IsBsonNull)
                            {
                                // Credit Card Payment
                                var ccInfo = docInfo["cc"].AsBsonArray.FirstOrDefault();
                                if (ccInfo != null && ccInfo.AsBsonDocument.Contains("cc_peraondate"))
                                {
                                    expensePaidDate = ccInfo["cc_peraondate"].AsString;
                                }
                            }
                            else if (docInfo.Contains("cheques") && !docInfo["cheques"].IsBsonNull)
                            {
                                // Cheque Payment
                                var chequesArray = docInfo["cheques"].AsBsonArray;
                                if (chequesArray.Count > 0 && chequesArray[0].AsBsonDocument.Contains("chequeDate"))
                                {
                                    expensePaidDate = chequesArray[0]["chequeDate"].AsString;
                                }
                            }

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

                                    // Add 'expense_paid' and 'expense_paid_date'
                                    content.Add(new StringContent(payed.ToString().ToLower()), "expense_paid");
                                    content.Add(new StringContent(invoiceDate), "vat_date"); // תאריך ערך
                                    content.Add(new StringContent(invoiceDate), "invoice_date"); // תאריך אסמכתא
                                    content.Add(new StringContent(expensePaidDate), "expense_paid_date"); // תאריך תשלום
                                    content.Add(new StringContent(invoiceDate), "expense_date");

                                    content.Add(new StringContent("ההוצאה נרשמה דרך יונינט"), "comment");

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
                                    result_endpointIcountExpenseCreate = await endpointExpenseCreate_Response.Content.ReadAsStringAsync();

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
                                    content.Add(new StringContent(dateissued), "expense_date");
                                    content.Add(new StringContent(dateissued), "invoice_date");
                                    content.Add(new StringContent(dateissued), "vat_date");
                                    content.Add(new StringContent("ההוצאה נרשמה דרך יונינט"), "comment");
                                    
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
                                    result_endpointIcountExpenseCreate = await endpointExpenseCreate_Response.Content.ReadAsStringAsync();

                                    // Process the response

                                }
                            }



                        }

                        response = JsonConvert.DeserializeObject<createExpenseApiResponse>(result_endpointIcountExpenseCreate);
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





                    }




                }

                return response;
            }
            catch (Exception ex)
            {
                // Log exception and return null
                Console.WriteLine(ex.Message);
                return null;
            }
        }

        private async Task<byte[]> DownloadFileAsByteArray(string fileUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetByteArrayAsync(fileUrl);
            }
        }


        //_MorningException
        private async Task<string> GetErrorMessageFromMongo(int errorCode)
        {
            try
            {
                var filter = Builders<BsonDocument>.Filter.ElemMatch<BsonDocument>("data", Builders<BsonDocument>.Filter.Eq("Error Code", errorCode));
                var projection = Builders<BsonDocument>.Projection.Include("data.$");
                var result = await _MorningException.Find(filter).Project(projection).FirstOrDefaultAsync();

                if (result != null && result.Contains("data"))
                {
                    var errorData = result["data"].AsBsonArray.FirstOrDefault();
                    return errorData?["Description"].AsString ?? "Unknown error";
                }
                else
                {
                    return "Error code not found in exceptions collection.";
                }
            }
            catch (Exception ex)
            {
                // Log exception
                Console.WriteLine($"Error accessing MongoDB: {ex.Message}");
                return "Error retrieving exception details.";
            }
        }
        
        private async Task<List<UsersExternalSystemDynamicFields>> GetUserDynamicFields(int userId, int companyId, int subCompanyId)
        {
            List<UsersExternalSystemDynamicFields> userExternalSystemDynamicFieldsList = null;

            // Check if the user is a master user
            var masterUser = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == userId);
            if (masterUser != null)
            {
                // Retrieve dynamic fields for the master user
                userExternalSystemDynamicFieldsList = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                    x => x.Companyid == companyId &&
                         x.Userid == userId &&
                         x.SubCompayId == subCompanyId
                );
            }
            else
            {
                // Retrieve the related master ID for the sub-user
                var relatedMaster = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == userId);
                if (relatedMaster != null)
                {
                    // Retrieve dynamic fields for the related master user
                    userExternalSystemDynamicFieldsList = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                        x => x.Companyid == relatedMaster.CompanyId &&
                             x.Userid == relatedMaster.Userid &&
                             x.SubCompayId == subCompanyId
                    );

                    // Update the userId to the master user ID
                    userId = relatedMaster.Userid;
                }
                else
                {
                    // Retrieve dynamic fields for the sub-user
                    userExternalSystemDynamicFieldsList = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(
                        x => x.Companyid == companyId &&
                             x.Userid == userId &&
                             x.SubCompayId == subCompanyId
                    );
                }
            }

            return userExternalSystemDynamicFieldsList;
        }
        

             private List<ExpenseType> GetExpenseTypeListWithNoDefault(string json,  bool Permanent)
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
                    ExpenseTypeId = expenseTypeDoc["expense_type_id"].ToString(),
                    ExpenseTypeDesc = expenseTypeDesc,
                    IsDefault = false
                };

                expenseTypeList.Add(newExpenseType);
            }

            return expenseTypeList;
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
                    ExpenseTypeId = expenseTypeDoc["expense_type_id"].AsInt32.ToString(),
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
            List<ExpenseType> list = null;
            if (ExpenseTypeId != null)
            {
                 list = GetExpenseTypeListWithDefault(responseContent, Convert.ToInt32(ExpenseTypeId), isPermanent);
            }
            else
            {
                list = GetExpenseTypeListWithNoDefault(responseContent,  isPermanent);
            }
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
