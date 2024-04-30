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

namespace Uninet.DATA.Services
{
    public class UninetOutputDataAccess : IUninetOutputDataAccess
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IMongoCollection<BsonDocument> _ICountCollection;
        private readonly IMongoCollection<BsonDocument> _ICountDocInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountClientInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountCompaniesInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountClientSuppliers;
        private readonly IMongoCollection<BsonDocument> _IcountExpenses;
        private readonly IMongoCollection<BsonDocument> _IcountExpensesTypes;
        private readonly IMongoCollection<BsonDocument> _IcountWebhookData;
        private readonly IDataMailassist _dataMailassist;
        //IcountWebhookData
        public UninetOutputDataAccess(IRepository<UninetContext> repository, IMongoClient client, IDataMailassist dataMailassist)//, IloginRepository loginRepository
        {
            var database = client.GetDatabase("Uninet");
            _ICountCollection = database.GetCollection<BsonDocument>("Icount");
            _ICountDocInfoCollection = database.GetCollection<BsonDocument>("IcountDocInfo");
            _IcountClientInfoCollection = database.GetCollection<BsonDocument>("icountClientInfo");
            _IcountCompaniesInfoCollection = database.GetCollection<BsonDocument>("IcountCompanisInfo");
            _IcountClientSuppliers = database.GetCollection<BsonDocument>("icountClientSuppliers");
            _IcountExpenses = database.GetCollection<BsonDocument>("IcountExpenses");
            _IcountExpensesTypes= database.GetCollection<BsonDocument>("IcountExpensesTypes");
            _IcountWebhookData= database.GetCollection<BsonDocument>("IcountWebhookData");
            _repository = repository;
            _dataMailassist = dataMailassist;

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

            var ClinetinfoEndpoint =await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);  ////api.icount.co.il/api/v3.php/supplier/get_list
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
                    supplier_id = Convert.ToInt32(supplier.Name),
                    vat_id = Convert.ToInt32(supplier.Value.GetProperty("vat_id").GetString()),
                    supplier_name = supplier.Value.GetProperty("supplier_name").GetString(),
                    company_name = supplier.Value.GetProperty("company_name").GetString(),
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

        private async Task<List<ExpenseType>> CreateExpenseCategorylist(int userId, string supplierId = null,string BusinessVatId=null, string cidvalue=null, string uservalue=null, string  passvalue=null,bool? DocumentApprovedtoUninet=false,int? ExpenseTypeId=0)
        {
            List<ExpenseType> ExpenseTypeList = new List<ExpenseType>();
            string SuplierId = "";
            // Retrieve the business object and get the InternalCompanyId
            var business = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
            if (business != null)
            {
                if (DocumentApprovedtoUninet == false || DocumentApprovedtoUninet==null)
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



                    var ExpenseTypeListEndpoint =await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);
                    var ExpenseTypeListEndpointinfo = ExpenseTypeListEndpoint.Endpoint;

                    var requestBody = new Dictionary<string, string>
                        {
                            {"cid",cidvalue},
                            {"pass",passvalue },
                            {"user",uservalue }
                        };

                    ExpenseTypeList = await PostExpencetypeList(ExpenseTypeListEndpointinfo, requestBody, expenseInfo.ExpenseTypeId);
                }

                if (DocumentApprovedtoUninet==true)
                {
                    var ExpenseTypeListEndpoint =await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 83);
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
        
            //////////////////////////////////////////////////////////////////////
            //var InternalCompanyId = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == userId);
            //var filter = Builders<BsonDocument>.Filter.And(
            //    Builders<BsonDocument>.Filter.Eq("internalcompanid", InternalCompanyId.BusinessId),
            //    Builders<BsonDocument>.Filter.Eq("UserID", userId)
            //);

            //var documents = _IcountExpenses.Find(filter).ToList();
            //var expenseTypeList = new List<ExpenseType>();

            //foreach (var document in documents)
            //{
            //    // Get the "results_list" field as a BsonDocument
            //    var resultsList = document["results_list"].AsBsonDocument;

            //    foreach (var item in resultsList)
            //    {
            //        try
            //        {
            //            // Extract "expense_type_id" and "expense_type_name" from the current item
            //            var expenseTypeId = item.Value["expense_type_id"].AsString;
            //            var expenseTypeName = item.Value["expense_type_name"].AsString;

            //            var expenseType = new ExpenseType
            //            {
            //                ExpenseTypeId = Convert.ToInt32(expenseTypeId),
            //                ExpenseTypeDesc = expenseTypeName
            //            };

            //            // Check if an ExpenseType with the same ExpenseTypeId already exists

            //            //expenseType.IsDefault = true;
            //            if (expenseType.ExpenseTypeId == 104)
            //            {
            //                expenseType.IsDefault = true;
            //            }
                      //bool expenseTypeExists = expenseTypeList.Any(et => et.ExpenseTypeId == expenseType.ExpenseTypeId);

            //            if (!expenseTypeExists)
            //            {
            //                expenseTypeList.Add(expenseType);
            //            }
            //        }
            //        catch(Exception ex) 
            //        { 

            //        }
            //   }

        //}



            /////////////////////get all expenses into expenseTypeList
            //var documentexpensetypes = _IcountExpensesTypes.Find(filter).ToList();

            //foreach (var document in documentexpensetypes)
            //{
            //    // Get the "results_list" field as a BsonDocument
            //    var resultsListexpensetypes = document["expense_types"].AsBsonDocument;

            //    foreach (var item in resultsListexpensetypes)
            //    {
            //        try
            //        {
            //            var expenseTypeId = item.Value["expense_type_id"].AsInt32;
            //            var expenseTypeName = item.Value["expense_type_name"].AsString;

            //            bool itemExists = expenseTypeList.Any(expenseType => expenseType.ExpenseTypeId == Convert.ToInt32(expenseTypeId));

            //            if (!itemExists)
            //            {
            //                var expenseType = new ExpenseType();
            //                expenseType.ExpenseTypeId = Convert.ToInt32(expenseTypeId);
            //                expenseType.ExpenseTypeDesc = expenseTypeName.ToString();

            //                expenseTypeList.Add(expenseType);
            //            }


            //        }
            //        catch (Exception ex) { }

            //    }

            //}


         

            //return expenseTypeList;
        }

        public async Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId)
        {
            try
            {
                Int32 InternalCompanyId = 0;
                int SubCompanyId = 0;
                string Email = "";
                string addressCity = "";
                string addressState = "";
                string addressStreet = "";
                string addressZip = "";
                string vatId = expensesUserDoRequest.ClientVat_id;
                vatId = vatId.PadLeft(9, '0');
                var FilterClientvatidCompanyInfo = Builders<BsonDocument>.Filter.Or(
                    Builders<BsonDocument>.Filter.Eq("company_info.vat_id", vatId),
                    Builders<BsonDocument>.Filter.Eq("company_info.vat_id", vatId.TrimStart('0'))
                );

                var companyClientRow = await _IcountCompaniesInfoCollection.Find(FilterClientvatidCompanyInfo).FirstOrDefaultAsync();

                string BussinessName = "";
                if (companyClientRow != null)
                {
                    var companyInfo = companyClientRow["company_info"].AsBsonDocument;
                    if (companyInfo.Contains("InternalCompanyId"))
                    {
                        var internalCompanyId = companyInfo["InternalCompanyId"].AsInt32;
                        InternalCompanyId = internalCompanyId;
                        // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
                    }
                    else
                    {
                        // Handle the case where 'InternalCompanyId' is not present in the document.
                    }
                    if (companyInfo.Contains("SubCompanyId"))
                    {
                        var subCompanyId = companyInfo["SubCompanyId"].AsInt32;
                        SubCompanyId = subCompanyId;
                        // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
                    }
                    else
                    {
                        // Handle the case where 'InternalCompanyId' is not present in the document.
                    }

                }
                else
                {
                    // Handle the case where no document matches the filter.
                }

                //we get all list of supliers for the user loged into uninet and get his suplierid and supliername
                List<UsersExternalSystemDynamicFields> UserexternalSystemDynamicFieldslist = null;
                var CheckUsermasterExist =await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == userId);///if user id exist in column userid in table MainSubCopmaniesMasters than he is a master
                if (CheckUsermasterExist != null)
                {
                    UserexternalSystemDynamicFieldslist =await  _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == InternalCompanyId && x.Userid == userId && x.SubCompayId== SubCompanyId);
                }
                else
                {
                    var GetRelatedMasterId =await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == userId);
                    UserexternalSystemDynamicFieldslist =await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == GetRelatedMasterId.CompanyId && x.Userid == GetRelatedMasterId.Userid && x.SubCompayId == SubCompanyId);
                    userId = GetRelatedMasterId.Userid;
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
                var resSUpplierLIst = await GetClientSupplierList(cidvalue, uservalue, passvalue);
                //var resSUpplierLIst = await GetClientSupplierList(userId);

                //BusinessData
                var RowBusinessData = await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == expensesUserDoRequest.JsonDocumentid);
                //extract doctype,DocDate,total from IcountDocInfo
                string docnum = "";
                string Doctype = "";
                string dateissuedstr = "";
                string total_before_nicui = "";
                DateTime DocDate = DateTime.MinValue;
                string totalstr = "";
                double total = 0;
                double AmountBeforeVat = 0;
                double total_before_nicuiDouble = 0;
                string AmountBeforeVatstr = "";
                double DoubleVatresult = 0;
                string DoubleVatresultstr = "";
                string TaxId = "";
                if (RowBusinessData != null)
                {

                    if (RowBusinessData.DataSourceType == 1)
                    {
                        var Documentidfilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(expensesUserDoRequest.JsonDocumentid));
                        var doctypeprojection = Builders<BsonDocument>.Projection.Include("doctype").Exclude("_id");
                        var doctyperesult = _ICountDocInfoCollection.Find(Documentidfilter).Project(doctypeprojection).FirstOrDefault();

                        var DocDateprojection = Builders<BsonDocument>.Projection.Include("doc_info.dateissued").Exclude("_id");
                        var DocDateresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(DocDateprojection).FirstOrDefault();


                        var totalprojection = Builders<BsonDocument>.Projection.Include("doc_info.total").Exclude("_id");
                        var totalresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(totalprojection).FirstOrDefault();

                        var docnumprojection = Builders<BsonDocument>.Projection.Include("doc_info.docnum").Exclude("_id");
                        var docnumresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(docnumprojection).FirstOrDefault();

                        ////////////////////////////////////////////////////////////////////////////////////////////////////
                        ///
                        /*
                          "TaxId": ---זה למעשה ח"פ מספר ישות של החברה 
                          "AmountBeforeVat"-- סכום לפני מיסוי 
                          "Vat"--מיסוי עצמו
                        */





                        //vat_percent




                        var TaxIdprojection = Builders<BsonDocument>.Projection.Include("doc_info.vat_id").Exclude("_id");
                        var Taxresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(TaxIdprojection).FirstOrDefault();


                        if (Taxresult != null)
                        {

                            TaxId = Taxresult["doc_info"]["vat_id"].AsString;
                        }





                        var AmountBeforeVatprojection = Builders<BsonDocument>.Projection.Include("doc_info.totalsum").Exclude("_id");
                        var AmountBeforeVatresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(AmountBeforeVatprojection).FirstOrDefault();

                        if (AmountBeforeVatresult != null)
                        {
                            BsonValue totalValue = AmountBeforeVatresult["doc_info"]["totalsum"];
                            if (totalValue.IsString)
                            {
                                string totalString = totalValue.AsString;
                                if (double.TryParse(totalString, out double totalDouble))
                                {
                                    AmountBeforeVat = totalDouble;
                                }
                                else
                                {
                                    // Handle the case when the string cannot be parsed as a double
                                }
                            }
                            else if (totalValue.IsDouble)
                            {
                                AmountBeforeVat = totalValue.AsDouble;
                            }
                            else
                            {
                                // Handle other data types if necessary
                            }
                        }


                        var Vatprojection = Builders<BsonDocument>.Projection.Include("doc_info.totalvat").Exclude("_id");
                        var Vatresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(Vatprojection).FirstOrDefault();

                        //totalvat eyal to deploy 

                        if (Vatresult != null)
                        {
                            BsonValue totalValue = Vatresult["doc_info"]["totalvat"];
                            if (totalValue.IsString)
                            {
                                string totalString = totalValue.AsString;
                                if (double.TryParse(totalString, out double totalDouble))
                                {
                                    DoubleVatresult = totalDouble;
                                }
                                else
                                {
                                    // Handle the case when the string cannot be parsed as a double
                                }
                            }
                            else if (totalValue.IsDouble)
                            {
                                DoubleVatresult = totalValue.AsDouble;
                            }
                            else
                            {
                                // Handle other data types if necessary
                            }
                        }






                        if (totalresult != null)
                        {
                            BsonValue totalValue = totalresult["doc_info"]["total"];
                            if (totalValue.IsString)
                            {
                                string totalString = totalValue.AsString;
                                if (double.TryParse(totalString, out double totalDouble))
                                {
                                    total = totalDouble;
                                }
                                else
                                {
                                    // Handle the case when the string cannot be parsed as a double
                                }
                            }
                            else if (totalValue.IsDouble)
                            {
                                total = totalValue.AsDouble;
                            }
                            else
                            {
                                // Handle other data types if necessary
                            }
                        }








                        if (DocDateresult != null)
                        {
                            BsonValue dateValue = DocDateresult["doc_info"]["dateissued"];
                            if (dateValue.IsString)
                            {
                                string dateString = dateValue.AsString;
                                if (DateTime.TryParse(dateString, out DateTime parsedDate))
                                {
                                    DocDate = parsedDate;
                                }
                                else
                                {
                                    // Handle the case when the string cannot be parsed as a DateTime
                                }
                            }
                            else if (dateValue.IsDateTime)
                            {
                                DocDate = dateValue.AsDateTime;
                            }
                            else
                            {
                                // Handle other data types if necessary
                            }
                        }




                        if (doctyperesult != null)
                        {

                            Doctype = doctyperesult["doctype"].AsString;
                        }



                        if (docnumresult != null)
                        {
                            docnum = docnumresult["doc_info"]["docnum"].AsString;

                        }
                    }
                    if (RowBusinessData.DataSourceType == 2)
                    {
                        var filter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(expensesUserDoRequest.JsonDocumentid));
                        var webhookdoc = _IcountWebhookData.Find(filter).FirstOrDefault();

                        if (webhookdoc != null)
                        {

                            docnum = webhookdoc["doc_info"]["docnum"].AsString;

                            Doctype = webhookdoc["doc_info"]["doctype"].AsString;


                            dateissuedstr = webhookdoc["doc_info"]["dateissued"].AsString;
                            DateTime date = DateTime.ParseExact(dateissuedstr, "yyyy-MM-dd", CultureInfo.InvariantCulture);

                            DocDate = date;

                            totalstr = webhookdoc["doc_info"]["total"].AsString;
                            total = Convert.ToDouble(totalstr);

                            if (webhookdoc["doc_info"].AsBsonDocument.Contains("totalsum"))
                            {

                                AmountBeforeVatstr = webhookdoc["doc_info"]["totalsum"].AsString;
                                AmountBeforeVat = Convert.ToDouble(AmountBeforeVatstr);
                            }

                            if (webhookdoc["doc_info"].AsBsonDocument.Contains("total_before_nicui"))
                            {


                                total_before_nicui = webhookdoc["doc_info"]["total_before_nicui"].AsString;
                                total_before_nicuiDouble = Convert.ToDouble(total_before_nicui);
                            }

                            if (webhookdoc["doc_info"].AsBsonDocument.Contains("totalvat"))
                            {
                                DoubleVatresultstr = webhookdoc["doc_info"]["totalvat"].AsString;
                                DoubleVatresult = Convert.ToDouble(DoubleVatresultstr);

                            }


                            if (webhookdoc["doc_info"].AsBsonDocument.Contains("totalwithnicui"))
                            {
                                DoubleVatresultstr = webhookdoc["doc_info"]["totalwithnicui"].AsString;
                                DoubleVatresult = Convert.ToDouble(DoubleVatresultstr);

                            }




                            //JsonDocumentid  //BusinessData





                        }
                    }








                    //now we should loop on the resSUpplierLIst
                    //and find if the vatId exist in the suplier list
                    var ItemFound = GetSupplierItemByVatId(resSUpplierLIst, Convert.ToInt32(expensesUserDoRequest.BusinessVatId));
                    ShowingDocsResults docsResults = new ShowingDocsResults();
                    docsResults.Success = true;
                    docsResults.ErrSec = "";
                    if (ItemFound != null)
                    {
                        //string cidvalue = null;
                        //string uservalue = null;
                        //string passvalue = null;

                        //JsonDocumentid
                       
                        List<ExpenseType> res = await CreateExpenseCategorylist(userId, ItemFound.supplier_id.ToString(), expensesUserDoRequest.BusinessVatId, cidvalue, uservalue, passvalue, RowBusinessData.DocumentApprovedtoUninet, RowBusinessData.ExpenseTypeId);
                        var expensesDigitalDocumentProp = new ExpensesDigitalDocumentProp
                        {
                            Supplier_name_Sender = ItemFound.supplier_name,
                            Supplier_ID = ItemFound.supplier_id,
                            DocNumber = docnum,
                            Doctype = Doctype,
                            DocDate = DocDate,
                            AmountAV = total,
                            ExpenseTypeList = res,
                            internalCompanyId = InternalCompanyId,
                            Jsondocumentid = expensesUserDoRequest.JsonDocumentid,
                            TaxId = expensesUserDoRequest.BusinessVatId,
                            AmountBeforeVat = AmountBeforeVat == 0 ? total_before_nicuiDouble : AmountBeforeVat,
                            Vat = DoubleVatresult,
                            showingDocsResults= docsResults



                        };
                        return expensesDigitalDocumentProp;
                    }
                    else//if not found call  // https://api.icount.co.il/api/v3.php/supplier/add
                    {


                        var FilterClientvatidSenderCompanyInfo = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", expensesUserDoRequest.BusinessVatId);




                        var SendercompanyClientRow = await _IcountCompaniesInfoCollection.Find(FilterClientvatidSenderCompanyInfo).FirstOrDefaultAsync();


                        var sendercompanyInfo = SendercompanyClientRow["company_info"].AsBsonDocument;


                        if (sendercompanyInfo.Contains("businessName"))
                        {
                            var businessName = sendercompanyInfo["businessName"].AsString;
                            BussinessName = businessName;
                            // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
                        }

                        if (sendercompanyInfo.Contains("email"))
                        {
                            var businessEmail = sendercompanyInfo["email"].AsString;
                            businessEmail = businessEmail;
                            // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
                        }


                        if (sendercompanyInfo.Contains("addressCity"))
                        {
                            var businessaddressCity = sendercompanyInfo["addressCity"].AsString;
                            addressCity = businessaddressCity;
                            // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
                        }

                        if (sendercompanyInfo.Contains("addressState"))
                        {
                            var businessaddressState = sendercompanyInfo["addressState"].AsString;
                            addressState = businessaddressState;
                            // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
                        }

                        if (sendercompanyInfo.Contains("addressStreet"))
                        {
                            var businessaddressStreet = sendercompanyInfo["addressStreet"].AsString;
                            addressStreet = businessaddressStreet;
                            // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
                        }

                        if (sendercompanyInfo.Contains("addressZip"))
                        {
                            var businessaddressZip = sendercompanyInfo["addressZip"].AsString;
                            addressZip = businessaddressZip;
                            // Now you have the InternalCompanyId value in the 'internalCompanyId' variable.
                        }









                        var ClinetinfoEndpoint =await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 76);
                        var endpointClinetinfo = ClinetinfoEndpoint.Endpoint;

                        //var FilterClientvatidCompanyInfo = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", expensesUserDoRequest.ClientVat_id);
                        //var companyclientRow = await _IcountCompaniesInfoCollection.Find(FilterClientvatidCompanyInfo).FirstOrDefaultAsync();


                        var requestBody = new Dictionary<string, string>
                        {
                            {"cid",cidvalue},
                            {"pass",passvalue },
                            {"user",uservalue },
                            { "supplier_name", BussinessName },//to get the provider name from the document its on the pdf document for example uninetconnect
                            { "vat_id", expensesUserDoRequest.BusinessVatId },
                            { "fname", "" },
                            { "lname", "" },
                            { "email", Email },//remark eyal to add an eamil her fro, the pdf doc
                            { "phone", "" },
                            { "mobile", "" },
                            { "fax", "" },
                            { "bus_country", addressState },
                            { "bus_city", addressCity },
                            { "bus_zip", addressZip },
                            { "bus_street", addressStreet },//remark eyal to get this and city here 
                            { "bus_no", "" },
                            { "bank", "" },
                            { "branch", "" },
                            { "account", "" },
                            { "faccount", "" },
                            { "wht_percent", "" },
                            { "wht_validity", "" },
                            { "notes", "" }
                        };

                        /*
                            string Email = "";
                            string addressCity = "";
                            string addressState = "";
                            string addressStreet = "";
                            string addressZip = "";

                        */
                        var supplierId = await PostAndGetSupplierId(endpointClinetinfo, requestBody);


                        var supplierItem = new SupplierItem
                        {
                            company_name = BussinessName,
                            supplier_id = supplierId,
                            supplier_name = BussinessName,
                            vat_id = Convert.ToInt32(expensesUserDoRequest.BusinessVatId)
                        };
                        resSUpplierLIst.Add(supplierItem);


                        //await CreateExpenseCategorylist(userId, ItemFound.supplier_id.ToString(), expensesUserDoRequest.BusinessVatId, cidvalue, uservalue, passvalue, RowBusinessData.DocumentApprovedtoUninet, RowBusinessData.ExpenseTypeId);
                        List<ExpenseType> res = await CreateExpenseCategorylist(userId, supplierId.ToString(), expensesUserDoRequest.BusinessVatId, cidvalue, uservalue, passvalue, RowBusinessData.DocumentApprovedtoUninet, RowBusinessData.ExpenseTypeId);
                        var SuplierItemFound = GetSupplierItemByVatId(resSUpplierLIst, Convert.ToInt32(expensesUserDoRequest.BusinessVatId));

                        var expensesDigitalDocumentProp = new ExpensesDigitalDocumentProp
                        {
                            Supplier_name_Sender = SuplierItemFound.supplier_name,
                            Supplier_ID = SuplierItemFound.supplier_id,
                            DocNumber = docnum,
                            Doctype = Doctype,
                            DocDate = DocDate,
                            AmountAV = total,
                            ExpenseTypeList = res,
                            internalCompanyId = InternalCompanyId,
                            Jsondocumentid = expensesUserDoRequest.JsonDocumentid,
                            TaxId = expensesUserDoRequest.BusinessVatId,
                            AmountBeforeVat = AmountBeforeVat,
                            Vat = DoubleVatresult,
                            showingDocsResults = docsResults

                        };
                       
                        return expensesDigitalDocumentProp;

                    }

                }
                    //}
                    return null;
               
            }
            catch (Exception ex) { return null; }
        }




        public async Task<responseTest> test(int UserID, string Typelist)
        {
            string text = "";
            try
            {
               
                string jsonResult = "";
                text = "UserID=" + UserID + "***";
                List<DigitalDocumentToApprove> List_DigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
                var ResListOfCompaniesRelatedToLogedinUser =await _repository.GetListOfObjectsAsync<Businesses>(x => x.AdminUserid == UserID);
                text= text+"result from " +System.Text.Json.JsonSerializer.Serialize(ResListOfCompaniesRelatedToLogedinUser);
                //loop on the list of Companies for each company attached to user we need to extract her vat_id from IcountCompanisInfo 
                foreach (var Company in ResListOfCompaniesRelatedToLogedinUser)
                {

                     text = text+ "1 ";
                    var filter = Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", Company.BusinessId);
                    var projection = Builders<BsonDocument>.Projection.Include("company_info.vat_id").Exclude("_id");
                    text = text + "2 before filter looking for Company.BusinessId"+ Company.BusinessId+ "in IcountCompanisInfo colection";
                    var result = _IcountCompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();
                    text = text + "3 ";
                    jsonResult = System.Text.Json.JsonSerializer.Serialize(result.ToString());
                    text = text + "4 ";
                    return new responseTest { text = "eyal111   "+ text };
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

                return new responseTest { text ="eyal222" };
            }
            catch (Exception ex) {
                return new responseTest { text = ex.Message + ex.InnerException+ text };
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

        public async Task<string> GetCompanyName(int subCompanyId,int MainCompanyId)
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
        public async Task<List<CompanyNameRelatedToUser>> PopulateListOfSubCompaniesandNames(List<MainSubCopmaniesMasters> ResListOfCompaniesRelatedToLogedinUser)
        {
            List<CompanyNameRelatedToUser> ListOfSubCompaniesandNames = new List<CompanyNameRelatedToUser>();

            // Find the most recent LastTimeDataShowed
            DateTime mostRecentTime = ResListOfCompaniesRelatedToLogedinUser.Max(c => c.LastTimeDataShowed);

            foreach (var company in ResListOfCompaniesRelatedToLogedinUser)
            {
                string companyName = await GetCompanyName(company.SubCopmanyId, company.MainCompanyId);
                if (companyName != null)
                {
                    bool isDefault = company.LastTimeDataShowed == mostRecentTime;

                    ListOfSubCompaniesandNames.Add(new CompanyNameRelatedToUser
                    {
                        SubCopmanyId = company.SubCopmanyId,
                        CompanyName = companyName,
                        IsDefault = isDefault
                    });
                }
            }

            // Assign the populated list to the property
            return ListOfSubCompaniesandNames;
        }


        public List<MainSubCopmaniesMasters> GetMainSubCompanies(int userId, int companyId)
        {
            List<MainSubCopmaniesMasters> ResListOfCompaniesRelatedToLogedinUser = new List<MainSubCopmaniesMasters>();

            // Get SubUserCredentials based on userId
            var subUserCredentials = _repository.GetListOfObjects<SubUserCredentials>(x => x.SubUserId == userId);

            // Get MainSubCopmaniesMasters
            var mainSubCompaniesMasters = _repository.GetListOfObjects<MainSubCopmaniesMasters>(x => x.MainCompanyId == companyId);

            // Loop through SubUserCredentials
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

            return ResListOfCompaniesRelatedToLogedinUser;
        }
        private async Task<List<BusinessData>> FetchPaginatedDigitalDocuments(int UserID, string Typelist, int? subCompanyId, string vatId, int pageNumber, int pageSize, bool? documentApprovedToUninet)
        {
            try
            {
                // Fetch the paginated list of digital documents using the updated repository method
                var digitalDocuments = _repository.GetListOfObjectsPaging<BusinessData>(
                    x => x.ClientVat_id == Convert.ToUInt32(vatId) &&
                         (documentApprovedToUninet == null || x.DocumentApprovedtoUninet == documentApprovedToUninet),
                    pageNumber,
                    pageSize
                ).OrderByDescending(x => x.docDate).ToList();

                return digitalDocuments;
            }
            catch (Exception ex)
            {
                // Handle exceptions here
                return new List<BusinessData>();
            }
        }

        public async Task<int> GetDocAmountSupplier(string vatId,int UserID, int subCopmanyId,int MainCompanyId, string cidvalue, string uservalue, string passvalue,string Supplier_id)
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
        private async Task<string> GetStatus(string vatId)
        {
            var filter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", vatId);
            var existingDocument = await _IcountCompaniesInfoCollection.Find(filter).FirstOrDefaultAsync();
            if (existingDocument != null)
            {
                return "Connected to Uninet";
            }
            else 
            {
                // Implement logic to determine other status values based on your requirements
                // For example:
                // if (someCondition) return "Missing email details";
                // else if (anotherCondition) return "Waiting for invitation";
                // else return "Status not determined";
                return "Still not connected";
            }
        }
        private async Task<List<BusinessPartnerProp>> GetSuppliers(string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId, int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId)
        {
            var suppliersList = new List<BusinessPartnerProp>();

            var supplierget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);
            var endpointsupplierget_list = supplierget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
            HttpMethod methodsupplierget_list = HttpMethod.Get;
            var Reponsnesupplierget_list = await SendRequest(endpointsupplierget_list, methodsupplierget_list);
            var jsonDocument = JsonDocument.Parse(Reponsnesupplierget_list);
            var jsonData = jsonDocument.RootElement;
            var suppliersData = jsonData.GetProperty("suppliers");

            foreach (var supplier in suppliersData.EnumerateObject())
            {
                var supplierData = supplier.Value;
                string businesspartnerName = supplierData.GetProperty("supplier_name").GetString();
                string vatId = supplierData.GetProperty("vat_id").GetString();
                string Supplier_id = supplierData.GetProperty("supplier_id").GetString();
                string SupplierEmail= supplierData.GetProperty("email").GetString();
                int docAmount =await GetDocAmountSupplier(vatId, userId, subCompanyId, MainCompanyId, cidvalue,  uservalue,  passvalue, Supplier_id);
                string status = await GetStatus(vatId);
                DateTime? lastInvitationDate = null;
                ActionItem action = null;

                var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(x => x.VatId == Convert.ToInt32(vatId) && x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId);

                if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue)
                {
                    if (status == "Connected to Uninet")
                    {
                        action = new ActionItem("Connected", ActionType.String);
                    }
                    else if (status == "Still not connected")
                    {
                        if (BusinessPartnersObj.EmailSent.Value)
                        {
                            var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
                            if (difference.TotalDays < 365)
                            {
                                action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
                            }
                            else
                            {
                                action = new ActionItem("Invite", ActionType.Button);
                            }
                        }
                        else
                        {
                            status = "Missing email details";
                            action = new ActionItem("Complete details", ActionType.Button);
                        }
                    }
                }
                else if (status == "Connected to Uninet")
                {
                    action = new ActionItem("Connected", ActionType.String);
                }
                else
                {
                    status = "Waiting for invitation";
                    action = new ActionItem("Invite", ActionType.Button);
                }

                suppliersList.Add(new BusinessPartnerProp
                {
                    BusinesspartnerName = businesspartnerName,
                    VatId = vatId,
                    supplier_id = Supplier_id,
                    DocAmount = docAmount,
                    Email= SupplierEmail,
                    SubCompanyId = subCompanyId,
                    Type = "Supplier",
                    Status = status,
                    LastInvitationDate = BusinessPartnersObj?.LastDateSent,
                    Actions = action
                });
            }

            return suppliersList;
        }



        // Function to handle case 2 logic
        private async Task<List<BusinessPartnerProp>> GetClients(string cidvalue, string uservalue, string passvalue, int userId, int subCompanyId, int MainCompanyId, int ExtrnalsystemIdOfsubCopmanyId)
        {
            var clientsList = new List<BusinessPartnerProp>();

            var clientget_listEndpoint = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 84);
            var endpointclientget_list = clientget_listEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
            HttpMethod methodclientget_list = HttpMethod.Get;
            var Reponsneclientget_list = await SendRequest(endpointclientget_list, methodclientget_list);
            var jsonDocumentclientget_list = JsonDocument.Parse(Reponsneclientget_list);
            var jsonDataclientget_list = jsonDocumentclientget_list.RootElement;
            var clientget_lisData = jsonDataclientget_list.GetProperty("clients");

            foreach (var client in clientget_lisData.EnumerateObject())
            {
                var clientData = client.Value;
                string businesspartnerName = clientData.GetProperty("client_name").GetString();
                string vatId = clientData.GetProperty("vat_id").GetString();
                string ClientEmail= clientData.GetProperty("email").GetString();
                int docAmount = await GetDocAmountclient(vatId, userId, subCompanyId, MainCompanyId, cidvalue, uservalue, passvalue);
                string status = await GetStatus(vatId);

                var BusinessPartnersObj = await _repository.GetFirstObjectAsync<BusinessPartnersEmails>(x => x.VatId == Convert.ToInt32(vatId) && x.UserId == userId && x.SubCompanyId == subCompanyId && x.OrganizationId == MainCompanyId);

                ActionItem action = null;

                if (status == "Connected to Uninet")
                {
                    action = new ActionItem("Connected", ActionType.String);
                }
                else if (status == "Still not connected")
                {
                    if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue && BusinessPartnersObj.EmailSent.Value)
                    {
                        var difference = DateTime.Now - BusinessPartnersObj.LastDateSent.Value;
                        if (difference.TotalDays < 365)
                        {
                            action = new ActionItem(BusinessPartnersObj.LastDateSent.Value.ToString(), ActionType.DateTime);
                        }
                        else
                        {
                            action = new ActionItem("Invite", ActionType.Button);
                        }
                    }
                    else if (BusinessPartnersObj != null && BusinessPartnersObj.EmailSent.HasValue && !BusinessPartnersObj.EmailSent.Value)
                    {
                        status = "Missing email details";
                        action = new ActionItem("Complete details", ActionType.Button);
                    }
                    else
                    {
                        status = "Waiting for invitation";
                        action = new ActionItem("Invite", ActionType.Button);
                    }
                }

                clientsList.Add(new BusinessPartnerProp
                {
                    BusinesspartnerName = businesspartnerName,
                    VatId = vatId,
                    DocAmount = docAmount,
                    Email= ClientEmail,
                    SubCompanyId = subCompanyId,
                    Type = "Client",
                    Status = status,
                    LastInvitationDate = BusinessPartnersObj?.LastDateSent,
                    Actions = action
                });
            }

            return clientsList;
        }


        public async Task<List<BusinessPartnerProp>> GetBusinessPartnersByFilter(int filterType,int userId, int subCopmanyId)
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
                var UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == MainCompanyId && x.Userid == userId && x.SubCompayId== subCopmanyId);
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
                List<BusinessPartnerProp> businessPartnersSupliers = new List<BusinessPartnerProp>();
                List<BusinessPartnerProp> businessPartnersClients = new List<BusinessPartnerProp>();
                switch (filterType)
                {
                    case 1:
                        //get suppliers
                        businessPartnersSupliers = await GetSuppliers(cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);
                        return businessPartnersSupliers;



                        break;
                    case 2:
                        businessPartnersClients = await GetClients(cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);
                        return businessPartnersClients;





                        break;
                    case 3:
                        var PartnersSuppliers = await GetSuppliers(cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);
                        var PartnersClients = await GetClients(cidvalue, uservalue, passvalue, userId, subCopmanyId, MainCompanyId, ExtrnalsystemIdOfsubCopmanyId);
                        var mergedList = PartnersSuppliers.Concat(PartnersClients).ToList();
                        return mergedList;



                        break;
                      
                }
                

               
                //get clients

                //get both


                //var gridData = new List<BusinessPartnerProp>
                //{
                //    new BusinessPartnerProp
                //    {
                //    BusinesspartnerName = "Partner 0",
                //    VatId = "123456789",
                //    DocAmount = 10,
                //    Status = "Active",
                //    LastInvitationDate = DateTime.Now,
                //    Actions = new ActionItem("Invite", ActionType.Button)
                //    },

                //    new BusinessPartnerProp
                //    {
                //        BusinesspartnerName = "Partner 1",
                //        VatId = "123456780",
                //        DocAmount = 5,
                //        Status = "Active",
                //        LastInvitationDate=DateTime.Now,
                //        Actions = new ActionItem("Connected", ActionType.String)// Example for "Connected" option
                //    },
                //    new BusinessPartnerProp
                //    {
                //        BusinesspartnerName = "Partner 2",
                //        VatId = "987654321",
                //        DocAmount = 10,
                //        Status = "Inactive",
                //        LastInvitationDate=DateTime.Now,
                //        Actions = new ActionItem("Complete details", ActionType.Button)
                       
                //    },
                //     new BusinessPartnerProp
                //    {
                //        BusinesspartnerName = "Partner 3",
                //        VatId = "987654321",
                //        DocAmount = 1,
                //        Status = "Inactive",
                //        LastInvitationDate=DateTime.Now,
                //        Actions = new ActionItem(DateTime.Now.ToString(), ActionType.DateTime)

                //    }

                //};

                return null;
                    // Add more rows as needed...
                

            }
            catch (Exception ex) { return null; }
        }
        public async Task<DigitalDocumentToApproveObj> GetDigitalDocumentToApproveListByUser(int UserID, string Typelist, int? subCompanyId, int pageNumber, int pageSize )
        {
            try
            {
                // List<DigitalDocumentToApprove> List_DigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
                string FullName = "";
                List<MainSubCopmaniesMasters> ResListOfCompaniesRelatedToLogedinUser = null;
                DigitalDocumentToApproveObj resObj = new DigitalDocumentToApproveObj();
                resObj.listDigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
                var CheckUsermasterExist =await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == UserID);///if user id exist in column userid in table MainSubCopmaniesMasters than he is a master
                int MainCompanyId = 0;
                if (CheckUsermasterExist!=null)//the user is master
                {
                    var MainCompanyIdObj= await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);
                    MainCompanyId = MainCompanyIdObj.BusinessId;
                    ResListOfCompaniesRelatedToLogedinUser = await _repository.GetListOfObjectsAsync<MainSubCopmaniesMasters>(x => x.MainCompanyId == MainCompanyId);
                }
                else
                {
                    var MainCompanyIdObj = await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == UserID);
                    MainCompanyId = MainCompanyIdObj.CompanyId;
                    ResListOfCompaniesRelatedToLogedinUser = GetMainSubCompanies(UserID, MainCompanyId);
                }
                
                 
               
                if (subCompanyId==null)
                {

                    if (ResListOfCompaniesRelatedToLogedinUser.Count > 1)
                    {


                        var subCompanyIdList = await _repository.GetAllAsync<MainSubCopmaniesMasters>();
                        subCompanyId = subCompanyIdList
                           .Where(x => x.MainCompanyId == MainCompanyId)
                           .OrderByDescending(x => x.LastTimeDataShowed)
                           .Select(x => x.SubCopmanyId)
                           .FirstOrDefault();
                    }
                    else
                    {
                        subCompanyId = ResListOfCompaniesRelatedToLogedinUser[0].SubCopmanyId;
                    }


                   
                    if (subCompanyId != default)
                    {
                        
                        var SubCopmaniesMastersRow=await _repository.GetFirstObjectAsync<MainSubCopmaniesMasters>(x => x.SubCopmanyId == subCompanyId && x.MainCompanyId == MainCompanyId);
                        SubCopmaniesMastersRow.LastTimeDataShowed = DateTime.Now; // Current DateTime

                        await _repository.UpdateAsync(SubCopmaniesMastersRow);
                    }
                    else
                    {
                        // Handle case where subCompanyId is not found
                    }

                }
                else
                {
                    var SubCopmaniesMastersRow =await _repository.GetFirstObjectAsync<MainSubCopmaniesMasters>(x => x.SubCopmanyId == subCompanyId && x.MainCompanyId == MainCompanyId);
                    SubCopmaniesMastersRow.LastTimeDataShowed = DateTime.Now; // Current DateTime

                    await _repository.UpdateAsync(SubCopmaniesMastersRow);

                }



               
                //loop on the list of Companies for each company attached to user we need to extract her vat_id from IcountCompanisInfo 
                //foreach (var Company in ResListOfCompaniesRelatedToLogedinUser)
                //{

                var filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", MainCompanyId),
                    Builders<BsonDocument>.Filter.Eq("company_info.SubCompanyId", subCompanyId)
                );

                var projection = Builders<BsonDocument>.Projection.Include("company_info.vat_id").Exclude("_id");

                var result = _IcountCompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();

                int totalCount = 0;
                if (result != null)
                    {
                        var vatId = result["company_info"]["vat_id"].AsString;




                        //now we go to BusinessData  table that has all digitaldocument sent to clients and check if client is there by his vat_id
                        //if its found we need to extract JsonDocumentid  and BusinessId (as the company that sent the document)
                        //and return a list of them to the client to show this waitingto approve list to insert as expenses
                        // Replace with your desired VAT ID

                        List<BusinessData> ResListOfClientCompaniesThatWasSentDigitalDocument = null;

                   


                    switch (Typelist)
                    {
                        case "notApproveOrRejected":
                            var totalCountObj = await _repository.GetListOfObjectsAsync<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == null);
                            totalCount= totalCountObj.Count();
                            ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
                                UserID, Typelist, subCompanyId, vatId, pageNumber, pageSize, null);
                            break;
                        case "Rejected":
                            var totalCountObj1 = await _repository.GetListOfObjectsAsync<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == false);
                            totalCount = totalCountObj1.Count();
                            ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
                                UserID, Typelist, subCompanyId, vatId, pageNumber, pageSize, false);
                            break;
                        case "Approved":
                            var totalCountObj2 = await _repository.GetListOfObjectsAsync<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == true);
                            totalCount = totalCountObj2.Count();
                            ResListOfClientCompaniesThatWasSentDigitalDocument = await FetchPaginatedDigitalDocuments(
                                UserID, Typelist, subCompanyId, vatId, pageNumber, pageSize, true);
                            break;
                        default:
                            // Handle invalid Typelist value
                            break;
                    }
                    //  string jsonResult = System.Text.Json.JsonSerializer.Serialize(ResListOfClientCompaniesThatWasSentDigitalDocument);


                    

                    foreach (var DigitalClientRow in ResListOfClientCompaniesThatWasSentDigitalDocument)
                        {

                            if (DigitalClientRow.DataSourceType == 1)
                            {
                                string JsonDocUrl = "";
                                var DocInfofilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(DigitalClientRow.JsonDocumentid));
                                var DocInfoprojection = Builders<BsonDocument>.Projection.Include("doc_info.doc_url_copy").Exclude("_id");
                                var DocInforesult = _ICountDocInfoCollection.Find(DocInfofilter).Project(DocInfoprojection).FirstOrDefault();
                                if (DocInforesult != null)
                                {
                                    JsonDocUrl = DocInforesult["doc_info"]["doc_url_copy"].AsString;
                                }
                                resObj.listDigitalDocumentToApprove.Add(new DigitalDocumentToApprove() { JsonDocumentid = DigitalClientRow.JsonDocumentid, ClientVat_id = Convert.ToInt32(DigitalClientRow.ClientVat_id), SendingDigitalDocumentBusinessID = DigitalClientRow.BusinessId, BusinessVatId = DigitalClientRow.BusinessVatId, DocInfoUrl = JsonDocUrl, supplier_name_Sender = DigitalClientRow.supplier_name_Sender, docDate = DigitalClientRow.docDate, amountAV = DigitalClientRow.amountAV, currency_code = DigitalClientRow.currency_code });
                            }
                            if (DigitalClientRow.DataSourceType == 2)//webhook data collection
                            {
                                string JsonDocUrl = "";
                                var DocInfofilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(DigitalClientRow.JsonDocumentid));
                                var DocInfoprojection = Builders<BsonDocument>.Projection.Include("doc_info.doc_url_copy").Exclude("_id");
                                var DocInforesult = _IcountWebhookData.Find(DocInfofilter).Project(DocInfoprojection).FirstOrDefault();
                                if (DocInforesult != null)
                                {
                                    JsonDocUrl = DocInforesult["doc_info"]["doc_url_copy"].AsString;
                                }

                            var filtercompany = Builders<BsonDocument>.Filter.And(
                                 Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", DigitalClientRow.BusinessId),
                                 Builders<BsonDocument>.Filter.Eq("company_info.SubCompanyId", DigitalClientRow.SubCompanyId)
                                );
                            var projectioncompany = Builders<BsonDocument>.Projection.Include("company_info.businessName").Exclude("_id");

                            var resultcompany = _IcountCompaniesInfoCollection.Find(filtercompany).Project(projectioncompany).FirstOrDefault();
                            string supplierNameSender = "";
                            if (resultcompany != null)
                            {
                                supplierNameSender = resultcompany["company_info"]["businessName"].AsString;
                            }
                            resObj.listDigitalDocumentToApprove.Add(new DigitalDocumentToApprove() { JsonDocumentid = DigitalClientRow.JsonDocumentid, ClientVat_id = Convert.ToInt32(DigitalClientRow.ClientVat_id), SendingDigitalDocumentBusinessID = DigitalClientRow.BusinessId, BusinessVatId = DigitalClientRow.BusinessVatId, DocInfoUrl = JsonDocUrl, supplier_name_Sender = supplierNameSender, docDate = DigitalClientRow.docDate, amountAV = DigitalClientRow.amountAV, currency_code = DigitalClientRow.currency_code });
                            }
                        }                    
                    }

                 //}

                    var UserexternalSystemDynamicFieldslist =await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == MainCompanyId && x.Userid == UserID && x.SubCompayId== subCompanyId);

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
                    var ClinetinfoEndpoint =await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 75);  ////api.icount.co.il/api/v3.php/supplier/get_list
                    var endpointClinetinfo = ClinetinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                    HttpMethod methodclientinfo = HttpMethod.Get;
                    var ReponsneClientInfo = await SendRequest(endpointClinetinfo, methodclientinfo);

                    // Parse the JSON response
                    var jsonDocument = JsonDocument.Parse(ReponsneClientInfo);
                    var jsonData = jsonDocument.RootElement;

                   

                    var bsonDocument = BsonDocument.Parse(jsonData.ToString());
                    // Add "internalcompanid" and "UserID" properties
                    bsonDocument.Add("internalcompanid", MainCompanyId);
                    bsonDocument.Add("UserID", UserID);


                    // Define the query to find and delete the existing document
                    var filterClientSuppliers = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                        Builders<BsonDocument>.Filter.Eq("UserID", UserID)
                    );

                    // Check if a document with the specified internalcompanid and UserID exists
                    var existingDocument = await _IcountClientSuppliers.Find(filterClientSuppliers).FirstOrDefaultAsync();

                    if (existingDocument != null)
                    {
                        // If the document exists, delete it
                         _IcountClientSuppliers.DeleteOne(filterClientSuppliers);
                        _IcountClientSuppliers.InsertOne(bsonDocument);
                    }
                    else
                    {
                        _IcountClientSuppliers.InsertOne(bsonDocument);
                    }
                    

                    

                    //added logic eyal to add json to expensesmongodb
                    var ExpenseSearchObj = await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 77);//api.icount.co.il/api/v3.php/expense/search
                    var ExpenseSearchEndpoint = ExpenseSearchObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue; //+ "&supplier_id=" + supplierId.ToString();
                    HttpMethod methodexpenseserach = HttpMethod.Get;
                    var ReponsneExpenseSearch = await SendRequest(ExpenseSearchEndpoint, methodexpenseserach);
                    var jsonDocumentReponsneExpenseSearch = JsonDocument.Parse(ReponsneExpenseSearch);
                    var jsonDataExpenseSearch = jsonDocumentReponsneExpenseSearch.RootElement;
                    var bsonDocumentExpenseSearch = BsonDocument.Parse(jsonDataExpenseSearch.ToString());
                    // Add "internalcompanid" and "UserID" properties
                    bsonDocumentExpenseSearch.Add("internalcompanid", MainCompanyId);
                    bsonDocumentExpenseSearch.Add("UserID", UserID);


                    // Define the query to find and delete the existing document
                    var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                        Builders<BsonDocument>.Filter.Eq("UserID", UserID)
                    );

                    var ExpenseexistingDocument = await _IcountExpenses.Find(filterExpenseSearch).FirstOrDefaultAsync();
                    if (ExpenseexistingDocument != null)
                    {
                        // If the document exists, delete it
                        _IcountExpenses.DeleteOne(filterExpenseSearch);
                        _IcountExpenses.InsertOne(bsonDocumentExpenseSearch);
                    }
                    else
                    {
                        _IcountExpenses.InsertOne(bsonDocumentExpenseSearch);
                    }

                    /////end added logic eyal to add json to expensesmongodb



                    //start add logic add json to _IcountExpensesTypes
                    var AllExpensesTypeObj =await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 78);//api.icount.co.il/api/v3.php/expense/types
                    var AllExpensesEndpoint = AllExpensesTypeObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                    HttpMethod AllExpensesMethod = HttpMethod.Get;
                    var ReponsneAllExpensesTypes = await SendRequest(AllExpensesEndpoint, AllExpensesMethod);
                    var ReponsneExpenseTypesjsonDocument = JsonDocument.Parse(ReponsneAllExpensesTypes);
                    var jsonDataExpensetypes = ReponsneExpenseTypesjsonDocument.RootElement;
                    var bsonDocumentExpensetypes = BsonDocument.Parse(jsonDataExpensetypes.ToString());
                    bsonDocumentExpensetypes.Add("internalcompanid", MainCompanyId);
                    bsonDocumentExpensetypes.Add("UserID", UserID);

                    // Define the query to find and delete the existing document
                    var filterExpensetypes = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalcompanid", MainCompanyId),
                        Builders<BsonDocument>.Filter.Eq("UserID", UserID)
                    );

                    var ExpensetypesexistingDocument = await _IcountExpensesTypes.Find(filterExpensetypes).FirstOrDefaultAsync();
                    if (ExpensetypesexistingDocument != null)
                    {
                        // If the document exists, delete it
                        _IcountExpensesTypes.DeleteOne(filterExpensetypes);
                        _IcountExpensesTypes.InsertOne(bsonDocumentExpensetypes);
                    }
                    else
                    {
                        _IcountExpensesTypes.InsertOne(bsonDocumentExpensetypes);
                    }
                //end add logic add json to _IcountExpensesTypes





                //}


                //if (CheckUsermasterExist.IsMasterUser == true)
                //{
                //    FullName = ResListOfCompaniesRelatedToLogedinUser[0].FirstName + " " + ResListOfCompaniesRelatedToLogedinUser[0].LastName;
                //}
                //else
                //{
                var firstnameObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);
                var lastnameObj = await _repository.GetFirstObjectAsync<Businesses>(x => x.AdminUserid == UserID);

                FullName = firstnameObj.FirstName + " " + lastnameObj.LastName;
                //}
                var Objres = new DigitalDocumentToApproveObj
                {
                    listDigitalDocumentToApprove = resObj.listDigitalDocumentToApprove,
                    TotallistDigitalDocumentToApprove = totalCount,
                    ListOfSubCompaniesandNames = await PopulateListOfSubCompaniesandNames(ResListOfCompaniesRelatedToLogedinUser),
                    fullname = FullName
                };
                return Objres;
            }
            catch (Exception ex) { return null; }
        }

       
        public SupplierItem GetSupplierItemByVatId(List<SupplierItem> supplierList, int vatId)
        {
            return supplierList.FirstOrDefault(supplier => supplier.vat_id == vatId);
        }

        public async Task<RejectDocumenResponse> RejectDocument(RequestRejectDocument requestRejectDocument)
        {
            try
            {
                var businessDatarow =await   _repository.GetFirstObjectAsync<BusinessData>(x => x.BusinessVatId == requestRejectDocument.BusinessVatId && x.ClientVat_id == requestRejectDocument.ClientVat_id && x.JsonDocumentid== requestRejectDocument.Jsondocumentid);
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
            catch (Exception ex) {
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
                var CheckUsermasterExist =await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == userId);///if user id exist in column userid in table MainSubCopmaniesMasters than he is a master
                if (CheckUsermasterExist != null)
                {
                    UserexternalSystemDynamicFieldslist =await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == InternalCompanyId && x.Userid == userId && x.SubCompayId == SubCompanyId);
                }
                else
                {
                    var GetRelatedMasterId =await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == userId);
                    UserexternalSystemDynamicFieldslist =await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == GetRelatedMasterId.CompanyId && x.Userid == GetRelatedMasterId.Userid && x.SubCompayId == SubCompanyId);
                    userId = GetRelatedMasterId.Userid;
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
                var AddexpenseTypeEndpoint =await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 80);
                string EndpointAddexpenseType = AddexpenseTypeEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                HttpMethod method = HttpMethod.Post; // Change to HttpMethod.Get for a GET request






                // Set your POST data if needed
                //string postData = "{\"vat_to_expense\": " + addexpenseTypeRequest.vat_to_expense + ", \"expense_type_name\": " + addexpenseTypeRequest.expense_type_name + ", \"deductable_vat\": \"" + addexpenseTypeRequest.deductable_vat + "\", \"deductable_expense\": \"" + addexpenseTypeRequest.deductable_expense + "}";
                //string postData = Newtonsoft.Json.JsonConvert.SerializeObject(addexpenseTypeRequest);

                string postData = "{\"vat_to_expense\": " + addexpenseTypeRequest.vat_to_expense.ToString().ToLower() + ", \"expense_type_name\": \"" + addexpenseTypeRequest.expense_type_name + "\", \"deductable_vat\": " + addexpenseTypeRequest.deductable_vat + ", \"deductable_expense\": " + addexpenseTypeRequest.deductable_expense + "}";

                string result = await SendRequest(EndpointAddexpenseType, method, postData);
                AddexpenseTypeResponse response = JsonConvert.DeserializeObject<AddexpenseTypeResponse>(result);
                // Handle the result as needed
                List<ExpenseType> res = new List<ExpenseType>();
                if (response.status)
                {
                    //when return from  creating the new  expensetype  we need to call icount again to retrieve the new list of expensetype
                     res = await CreateExpenseCategorylist(userId, addexpenseTypeRequest.supplier_ID, addexpenseTypeRequest.tax_id.ToString(), cidvalue, uservalue, passvalue);

                    var res2= new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1 ? "The expense type was added successfully" : "סוג הוצאה התווסף בהצלחה",
                        ExpenseTypeList = res
                    };
                    return res2;



                }
                else
                {
                    var res2 = new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1 ? "Failed to Add Expense type" : "נכשל ביצירת הוצאה ",
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
                int SubCompanyId=0;


                if (companyClientRow != null)
                {
                    var companyInfo = companyClientRow["company_info"].AsBsonDocument;
                    if (companyInfo.Contains("SubCompanyId"))
                    {
                        var subCompanyId = companyInfo["SubCompanyId"].AsInt32;
                        SubCompanyId = subCompanyId;

                    }
                }


                var CheckUsermasterExist =await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.Userid == userId);///if user id exist in column userid in table MainSubCopmaniesMasters than he is a master






                if (CheckUsermasterExist != null)//user is a master
                {
                    UserexternalSystemDynamicFieldslist =await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == insertUserDigitalDocRequest.internalCompanyId && x.Userid == userId && x.SubCompayId == SubCompanyId);
                    
                }
                else
                {
                    var GetRelatedMasterId =await _repository.GetFirstObjectAsync<SubUserCredentials>(x => x.SubUserId == userId);
                    UserexternalSystemDynamicFieldslist = await _repository.GetListOfObjectsAsync<UsersExternalSystemDynamicFields>(x => x.Companyid == GetRelatedMasterId.CompanyId && x.Userid == GetRelatedMasterId.Userid && x.SubCompayId == SubCompanyId);
                    userId = GetRelatedMasterId.Userid;

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
                var ExpenseCreateEndpoint =await _repository.GetFirstObjectAsync<SystemsEndpoints>(x => x.Id == 79);//https://api.icount.co.il/api/v3.php/expense/create
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

                if (insertUserDigitalDocRequest.expense_doctype== "invrec"  || insertUserDigitalDocRequest.expense_doctype == "receipt")
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
                    var businessDatarow =await _repository.GetFirstObjectAsync<BusinessData>(x => x.JsonDocumentid == insertUserDigitalDocRequest.Jsondocumentid);
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
        private List<ExpenseType> GetExpenseTypeListWithDefault(string json, int expenseTypeId)
        {
            var bsonDoc = BsonDocument.Parse(json);
            var expenseTypes = bsonDoc["expense_types"].AsBsonDocument;

            var expenseTypeList = new List<ExpenseType>();

            foreach (var expenseType in expenseTypes.Elements)
            {
                var expenseTypeDoc = expenseType.Value.AsBsonDocument;
                var newExpenseType = new ExpenseType
                {
                    ExpenseTypeId = expenseTypeDoc["expense_type_id"].AsInt32,
                    ExpenseTypeDesc = expenseTypeDoc["expense_type_name"].AsString,
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
            List<ExpenseType> list = GetExpenseTypeListWithDefault(responseContent, Convert.ToInt32(ExpenseTypeId));
            return list;
        }

            private async Task<int> PostAndGetSupplierId(string endpoint, Dictionary<string, string> requestBody)
        {
            var requestContent = new FormUrlEncodedContent(requestBody);

            var httpClient = new HttpClient();
            var response = await httpClient.PostAsync(endpoint, requestContent);
            var responseContent = await response.Content.ReadAsStringAsync();

            // Parse the JSON response
            var jsonDocument = JsonDocument.Parse(responseContent);
            if (jsonDocument.RootElement.TryGetProperty("supplier_id", out var supplierIdProperty) &&
                supplierIdProperty.ValueKind == JsonValueKind.Number)
            {
                return supplierIdProperty.GetInt32();
            }

            return -1; // Return a default value if supplier_id is not found or cannot be parsed
        }




    }

}
