using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Entities;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using static System.Net.Mime.MediaTypeNames;

namespace Uninet.DATA.Services
{
    public class UninetOutputDataAccess: IUninetOutputDataAccess
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IMongoCollection<BsonDocument> _ICountCollection;
        private readonly IMongoCollection<BsonDocument> _ICountDocInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountClientInfoCollection;
        private readonly IMongoCollection<BsonDocument> _IcountCompaniesInfoCollection;
        public UninetOutputDataAccess(IRepository<UninetContext> repository, IMongoClient client)//, IloginRepository loginRepository
        {
            var database = client.GetDatabase("Uninet");
            _ICountCollection = database.GetCollection<BsonDocument>("Icount");
            _ICountDocInfoCollection = database.GetCollection<BsonDocument>("IcountDocInfo");
            _IcountClientInfoCollection = database.GetCollection<BsonDocument>("icountClientInfo");
            _IcountCompaniesInfoCollection= database.GetCollection<BsonDocument>("IcountCompanisInfo");
            _repository = repository;
            
        }
        private async Task<string> SendRequest(string endpointUrl, HttpMethod method, string postData=null)
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
        private async Task<List<SupplierItem>> GetClientSupplierList(string cidvalue, string uservalue, string passvalue)
        {
            var ClinetinfoEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 75);
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



        private async Task<List<ExpenseType>> CreateExpenseCategorylist(string cidvalue, string uservalue,string  passvalue,string supplierId= null)
        {
            var expenseTypeList = new List<ExpenseType>();
            var ExpenseSearchObj = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 77);
            var ExpenseSearchEndpoint = ExpenseSearchObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue + "&supplier_id=" + supplierId.ToString();
            HttpMethod methodexpenseserach = HttpMethod.Get;
            var ReponsneExpenseSearch = await SendRequest(ExpenseSearchEndpoint, methodexpenseserach);
            var ExpenseSearchjsonDocument = JsonDocument.Parse(ReponsneExpenseSearch);
             expenseTypeList = new List<ExpenseType>();
            
            
            if (ExpenseSearchjsonDocument.RootElement.TryGetProperty("results_list", out var resultsListElement))
            {
                foreach (var expenseTypeElement in resultsListElement.EnumerateObject())
                {
                    try
                    {
                        var expenseType = new ExpenseType();
                        var expenseTypeIdProperty = expenseTypeElement.Value.GetProperty("expense_type_id");

                        if (expenseTypeIdProperty.ValueKind == JsonValueKind.String &&
                            int.TryParse(expenseTypeIdProperty.GetString(), out int expenseTypeId))
                        {
                            expenseType.ExpenseTypeId = expenseTypeId;
                        }
                        else if (expenseTypeIdProperty.ValueKind == JsonValueKind.Number)
                        {
                            expenseType.ExpenseTypeId = expenseTypeIdProperty.GetInt32();
                        }
                        else
                        {
                            // Handle the case when the property value is neither a string nor a number
                        }

                        expenseType.ExpenseTypeDesc = expenseTypeElement.Value.GetProperty("expense_type_name").GetString();
                        expenseType.IsDefault = true;
                        expenseTypeList.Add(expenseType);
                    }
                    catch (Exception ex)
                    {
                        // Handle the exception or log the error if needed
                    }
                }

            }
            /////////////////////get all expenses into expenseTypeList
            var AllExpensesTypeObj = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 78);
            var AllExpensesEndpoint = AllExpensesTypeObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
            HttpMethod AllExpensesMethod = HttpMethod.Get;
            var ReponsneAllExpenses = await SendRequest(AllExpensesEndpoint, AllExpensesMethod);
            var ReponsneExpenseSearchjsonDocument = JsonDocument.Parse(ReponsneAllExpenses);
            if (ReponsneExpenseSearchjsonDocument.RootElement.TryGetProperty("expense_types", out var expenseTypesElement))
            {
                foreach (var expenseTypeElement in expenseTypesElement.EnumerateObject())
                {
                    int expenseTypeId = expenseTypeElement.Value.GetProperty("expense_type_id").GetInt32();
                    string expenseTypeName = expenseTypeElement.Value.GetProperty("expense_type_name").GetString();

                    // Check if the item already exists in the expenseTypeList
                    bool itemExists = expenseTypeList.Any(expenseType => expenseType.ExpenseTypeId == expenseTypeId);

                    if (!itemExists)
                    {
                        var expenseType = new ExpenseType();
                        expenseType.ExpenseTypeId = expenseTypeId;
                        expenseType.ExpenseTypeDesc = expenseTypeName;
                        expenseTypeList.Add(expenseType);
                    }
                }
            }

            return expenseTypeList;
        }

        public async Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId)
        {
            try
            {
                Int32 InternalCompanyId = 0;
                //we need to extract InternalCompanyId from IcountCompanisInfo in order to query table UsersExternalSystemDynamicFields with companyid(InternalCompanyId) and userid
                var filter = Builders<BsonDocument>.Filter.Eq("company_info.vat_id", expensesUserDoRequest.ClientVat_id);
                var projection = Builders<BsonDocument>.Projection.Include("company_info.InternalCompanyId").Exclude("_id");

                var InternalCompanyIdresult = _IcountCompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();

                if (InternalCompanyIdresult != null)
                {
                     InternalCompanyId = InternalCompanyIdresult["company_info"]["InternalCompanyId"].AsInt32;


                    //from this table UsersExternalSystemDynamicFields we extract the user credentials for i count api query
                    var UserexternalSystemDynamicFieldslist = _repository.GetListOfObjects<UsersExternalSystemDynamicFields>(x => x.Companyid == InternalCompanyId && x.Userid == userId);

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


                    //we get all list of supliers for the user loged into uninet and get his suplierid and supliername

                    var resSUpplierLIst = await GetClientSupplierList(cidvalue, uservalue, passvalue);



                    //extract doctype,DocDate,total from IcountDocInfo
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

                    string TaxId = "";
                    if (Taxresult != null)
                    {

                        TaxId = Taxresult["doc_info"]["vat_id"].AsString;
                    }





                    var AmountBeforeVatprojection = Builders<BsonDocument>.Projection.Include("doc_info.totalsum").Exclude("_id");
                    var AmountBeforeVatresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(AmountBeforeVatprojection).FirstOrDefault();
                    double AmountBeforeVat = 0;
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


                    var Vatprojection = Builders<BsonDocument>.Projection.Include("doc_info.vat_percent").Exclude("_id");
                    var Vatresult = _ICountDocInfoCollection.Find(Documentidfilter).Project(Vatprojection).FirstOrDefault();


                    double DoubleVatresult = 0;
                    if (Vatresult != null)
                    {
                        BsonValue totalValue = Vatresult["doc_info"]["vat_percent"];
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





                    double total = 0;
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



                    



                    DateTime DocDate = DateTime.MinValue; // Set a default value if needed
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



                    string Doctype = "";
                    if (doctyperesult != null)
                    {

                         Doctype = doctyperesult["doctype"].AsString;
                    }


                    string docnum = "";
                    if (docnumresult != null)
                    {
                        docnum = docnumresult["doc_info"]["docnum"].AsString;

                    }


                    //now we should loop on the resSUpplierLIst
                    //and find if the vatId exist in the suplier list
                    var ItemFound = GetSupplierItemByVatId(resSUpplierLIst, Convert.ToInt32(expensesUserDoRequest.BusinessVatId));
                    if (ItemFound!=null)
                    {
                        List<ExpenseType> res = await CreateExpenseCategorylist(cidvalue, uservalue, passvalue, ItemFound.supplier_id.ToString());
                        var expensesDigitalDocumentProp = new ExpensesDigitalDocumentProp
                        {
                            Supplier_name_Sender = ItemFound.supplier_name,
                            Supplier_ID = ItemFound.supplier_id,
                            DocNumber= docnum,
                            Doctype = Doctype,
                            DocDate = DocDate,
                            AmountAV = total,
                            ExpenseTypeList = res,
                            internalCompanyId=InternalCompanyId,
                            Jsondocumentid= expensesUserDoRequest.JsonDocumentid,
                            TaxId=TaxId,
                            AmountBeforeVat=AmountBeforeVat,
                            Vat= DoubleVatresult


                        };
                        return expensesDigitalDocumentProp;
                    }
                    else//if not found call  // https://api.icount.co.il/api/v3.php/supplier/add
                    {
                        var ClinetinfoEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 76);
                        var endpointClinetinfo = ClinetinfoEndpoint.Endpoint;

                        var requestBody = new Dictionary<string, string>
                        {
                            { "cid", cidvalue },
                            { "user", uservalue },
                            { "pass", passvalue },
                            { "supplier_name", "provider" },
                            { "vat_id", expensesUserDoRequest.BusinessVatId },
                            { "fname", "" },
                            { "lname", "" },
                            { "email", "" },
                            { "phone", "" },
                            { "mobile", "" },
                            { "fax", "" },
                            { "bus_country", "" },
                            { "bus_city", "" },
                            { "bus_zip", "" },
                            { "bus_street", "" },
                            { "bus_no", "" },
                            { "bank", "" },
                            { "branch", "" },
                            { "account", "" },
                            { "faccount", "" },
                            { "wht_percent", "" },
                            { "wht_validity", "" },
                            { "notes", "" }
                        };

                        var supplierId = await PostAndGetSupplierId(endpointClinetinfo, requestBody);

                        List<ExpenseType> res=await CreateExpenseCategorylist(cidvalue, uservalue, passvalue, supplierId.ToString());
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
                            TaxId = TaxId,
                            AmountBeforeVat = AmountBeforeVat,
                            Vat = DoubleVatresult
                        };
                        return expensesDigitalDocumentProp;

                    }


                }
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
                var ResListOfCompaniesRelatedToLogedinUser = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == UserID);
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
        public async Task<List<DigitalDocumentToApprove>> GetDigitalDocumentToApproveListByUser(int UserID, string Typelist)
        {
            try
            {
                List<DigitalDocumentToApprove> List_DigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
                var ResListOfCompaniesRelatedToLogedinUser = _repository.GetListOfObjects<Businesses>(x => x.AdminUserid == UserID);

                //loop on the list of Companies for each company attached to user we need to extract her vat_id from IcountCompanisInfo 
                foreach (var Company in ResListOfCompaniesRelatedToLogedinUser)
                {

                    var filter = Builders<BsonDocument>.Filter.Eq("company_info.InternalCompanyId", Company.BusinessId);
                    var projection = Builders<BsonDocument>.Projection.Include("company_info.vat_id").Exclude("_id");

                    var result = _IcountCompaniesInfoCollection.Find(filter).Project(projection).FirstOrDefault();

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

                            //remark eyal need to get 
                            //supplier_name_Sender
                            //docDate
                            //amountAV

                            case "notApproveOrRejected":
                                 ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == null);
                               // ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) );
                                break;
                            case "Rejected":
                                ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == false);
                                break;
                            case "Approved":
                                ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == true);
                                break;
                        }

                      //  string jsonResult = System.Text.Json.JsonSerializer.Serialize(ResListOfClientCompaniesThatWasSentDigitalDocument);


                        foreach (var DigitalClientRow in ResListOfClientCompaniesThatWasSentDigitalDocument)
                        {
                            string JsonDocUrl = "";
                            var DocInfofilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(DigitalClientRow.JsonDocumentid));
                            var DocInfoprojection = Builders<BsonDocument>.Projection.Include("doc_info.doc_url").Exclude("_id");
                            var DocInforesult = _ICountDocInfoCollection.Find(DocInfofilter).Project(DocInfoprojection).FirstOrDefault();
                            if (DocInforesult != null)
                            {
                                 JsonDocUrl = DocInforesult["doc_info"]["doc_url"].AsString;
                            }
                            List_DigitalDocumentToApprove.Add(new DigitalDocumentToApprove() { JsonDocumentid = DigitalClientRow.JsonDocumentid, ClientVat_id = Convert.ToInt32(DigitalClientRow.ClientVat_id), SendingDigitalDocumentBusinessID = DigitalClientRow.BusinessId , BusinessVatId= DigitalClientRow.BusinessVatId ,DocInfoUrl= JsonDocUrl, supplier_name_Sender= DigitalClientRow.supplier_name_Sender, docDate= DigitalClientRow.docDate, amountAV = DigitalClientRow.amountAV  });

                        }                    
                    }

                 }

                return List_DigitalDocumentToApprove;
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
                var businessDatarow = _repository.GetFirstObject<BusinessData>(x => x.BusinessVatId == requestRejectDocument.BusinessVatId && x.ClientVat_id == requestRejectDocument.ClientVat_id);
                if (businessDatarow != null) 
                {
                    businessDatarow.DocumentApprovedtoUninet = false;
                    await _repository.UpdateAsync(businessDatarow);
                    var res = new RejectDocumenResponse
                    {
                        Success = true,
                        textResponse = requestRejectDocument.Lang == 1 ? "Document rejected Succesfully" : "המסמך נדחה בהצלחה"
                    };
                    return res;

                }
                else
                {
                    var res = new RejectDocumenResponse
                    {
                        Success = true,
                        textResponse = requestRejectDocument.Lang == 1 ? "error occured Document wasnt rejected " : "ארעה שגיאה המבמך לא נדחה "
                    };
                    return res;
                }

               
                
            }
            catch (Exception ex) {
                var res = new RejectDocumenResponse
                {
                    Success = false,
                    textResponse = requestRejectDocument.Lang == 1 ? "error occured Document wasnt rejected " : "ארעה שגיאה המבמך לא נדחה "
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
        //        var ExpenseCreateEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 79);
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


        public async Task<createExpenseApiResponse> InsertUserDigitalDocToUninetSystem(InsertUserDigitalDocRequest insertUserDigitalDocRequest, int userId)
        {
            try
            {
                var UserexternalSystemDynamicFieldslist = _repository.GetListOfObjects<UsersExternalSystemDynamicFields>(x => x.Companyid == insertUserDigitalDocRequest.internalCompanyId && x.Userid == userId);

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
                var ExpenseCreateEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 79);
                string endpointExpenseCreate = ExpenseCreateEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                HttpMethod method = HttpMethod.Post; // Change to HttpMethod.Get for a GET request

                /////need to add logic search supplier in the 
                /////
                ////we get all list of supliers for the user loged into uninet and get his suplierid and supliername
                //var resSUpplierLIst = await GetClientSupplierList(cidvalue, uservalue, passvalue);
                ////now we should loop on the resSUpplierLIst
                ////and find if the vatId exist in the suplier list
                //var BusinessVatId = _repository.GetFirstObject<BusinessData>(x => x.BusinessId == insertUserDigitalDocRequest.internalCompanyId);
                //var ItemFound = GetSupplierItemByVatId(resSUpplierLIst, Convert.ToInt32(BusinessVatId));
               





                // Set your POST data if needed
                string postData = "{\"supplier_id\": " + insertUserDigitalDocRequest.supplier_id + ", \"expense_type_id\": " + insertUserDigitalDocRequest.expense_type_id + ", \"expense_doctype\": \"" + insertUserDigitalDocRequest.expense_doctype + "\", \"expense_docnum\": \"" + insertUserDigitalDocRequest.expense_docnum + "\", \"internalCompanyId\": " + insertUserDigitalDocRequest.internalCompanyId + ", \"expense_sum\": " + insertUserDigitalDocRequest.expense_sum + "}";


                string result = await SendRequest(endpointExpenseCreate, method, postData);
                createExpenseApiResponse response = JsonConvert.DeserializeObject<createExpenseApiResponse>(result);
                // Handle the result as needed
               if (response.status)
                {
                    var businessDatarow = _repository.GetFirstObject<BusinessData>(x => x.JsonDocumentid == insertUserDigitalDocRequest.Jsondocumentid);
                    if (businessDatarow != null)
                    {
                        businessDatarow.DocumentApprovedtoUninet = true;
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
