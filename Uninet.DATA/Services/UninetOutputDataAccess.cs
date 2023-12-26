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
        //IcountWebhookData
        public UninetOutputDataAccess(IRepository<UninetContext> repository, IMongoClient client)//, IloginRepository loginRepository
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
        private async Task<List<SupplierItem>> GetClientSupplierList(int Userid)
        {
            var InternalCompanyId = _repository.GetFirstObject<Businesses>(x => x.AdminUserid == Userid);

            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("internalcompanid", InternalCompanyId.BusinessId),
                Builders<BsonDocument>.Filter.Eq("UserID", Userid)
            );

            var document = await _IcountClientSuppliers.Find(filter).FirstOrDefaultAsync();
            if (document != null)
            {
                var suppliersData = document["suppliers"].AsBsonDocument;
                var supplierList = new List<SupplierItem>();

                // Loop through the suppliers data and map it to SupplierItem objects
                foreach (var supplier in suppliersData)
                {
                    if (supplier.Value is BsonDocument supplierObject)
                    {
                        var vatIdValue = supplierObject.TryGetValue("vat_id", out var vatId) ? vatId.AsString : null;

                        var supplierItem = new SupplierItem
                        {
                            supplier_id = Convert.ToInt32(supplier.Name),
                            vat_id = string.IsNullOrEmpty(vatIdValue) ? 0 : Convert.ToInt32(vatIdValue),
                            supplier_name = supplierObject.TryGetValue("supplier_name", out var supplierName) ? supplierName.AsString : null,
                            company_name = supplierObject.TryGetValue("company_name", out var companyName) ? companyName.AsString : null
                        };

                        supplierList.Add(supplierItem);
                    }
                }

                return supplierList;
            }
            else
            {
                return null;
            }
        }



        private async Task<List<ExpenseType>> CreateExpenseCategorylist(int userId, string supplierId = null)
        {

            var InternalCompanyId = _repository.GetFirstObject<Businesses>(x => x.AdminUserid == userId);
            var filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq("internalcompanid", InternalCompanyId.BusinessId),
                Builders<BsonDocument>.Filter.Eq("UserID", userId)
            );

            var documents = _IcountExpenses.Find(filter).ToList();
            var expenseTypeList = new List<ExpenseType>();

            foreach (var document in documents)
            {
                // Get the "results_list" field as a BsonDocument
                var resultsList = document["results_list"].AsBsonDocument;

                foreach (var item in resultsList)
                {
                    try
                    {
                        // Extract "expense_type_id" and "expense_type_name" from the current item
                        var expenseTypeId = item.Value["expense_type_id"].AsString;
                        var expenseTypeName = item.Value["expense_type_name"].AsString;

                        var expenseType = new ExpenseType
                        {
                            ExpenseTypeId = Convert.ToInt32(expenseTypeId),
                            ExpenseTypeDesc = expenseTypeName
                        };

                        // Check if an ExpenseType with the same ExpenseTypeId already exists

                        //expenseType.IsDefault = true;
                        bool expenseTypeExists = expenseTypeList.Any(et => et.ExpenseTypeId == expenseType.ExpenseTypeId);

                        if (!expenseTypeExists)
                        {
                            expenseTypeList.Add(expenseType);
                        }
                    }
                    catch(Exception ex) 
                    { 
                    
                    }
               }

           }



            /////////////////////get all expenses into expenseTypeList
            var documentexpensetypes = _IcountExpensesTypes.Find(filter).ToList();

            foreach (var document in documentexpensetypes)
            {
                // Get the "results_list" field as a BsonDocument
                var resultsListexpensetypes = document["expense_types"].AsBsonDocument;

                foreach (var item in resultsListexpensetypes)
                {
                    try
                    {
                        var expenseTypeId = item.Value["expense_type_id"].AsInt32;
                        var expenseTypeName = item.Value["expense_type_name"].AsString;

                        bool itemExists = expenseTypeList.Any(expenseType => expenseType.ExpenseTypeId == Convert.ToInt32(expenseTypeId));

                        if (!itemExists)
                        {
                            var expenseType = new ExpenseType();
                            expenseType.ExpenseTypeId = Convert.ToInt32(expenseTypeId);
                            expenseType.ExpenseTypeDesc = expenseTypeName.ToString();

                            expenseTypeList.Add(expenseType);
                        }


                    }
                    catch (Exception ex) { }

                }

            }


         

            return expenseTypeList;
        }

        public async Task<ExpensesDigitalDocumentProp> ShowDigitalDocumentDetails(DigitalDocumentDInputRequest expensesUserDoRequest, int userId)
        {
            try
            {
                Int32 InternalCompanyId = 0;
           

                //we get all list of supliers for the user loged into uninet and get his suplierid and supliername

                //var resSUpplierLIst = await GetClientSupplierList(cidvalue, uservalue, passvalue);

                var resSUpplierLIst = await GetClientSupplierList(userId);

                //BusinessData
                var RowBusinessData = _repository.GetFirstObject<BusinessData>(x => x.JsonDocumentid == expensesUserDoRequest.JsonDocumentid);
                //extract doctype,DocDate,total from IcountDocInfo
                string docnum = "";
                string Doctype = "";
                string dateissuedstr = "";
                DateTime DocDate = DateTime.MinValue;
                string totalstr = "";
                double total = 0;
                double AmountBeforeVat = 0;
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



                            AmountBeforeVatstr = webhookdoc["doc_info"]["totalsum"].AsString;
                            AmountBeforeVat= Convert.ToDouble(AmountBeforeVatstr);



                            DoubleVatresultstr = webhookdoc["doc_info"]["totalvat"].AsString;
                            DoubleVatresult=Convert.ToDouble(DoubleVatresultstr);
                                



                            }
                    }

            
                        


                    
                }

                    //now we should loop on the resSUpplierLIst
                    //and find if the vatId exist in the suplier list
                    var ItemFound = GetSupplierItemByVatId(resSUpplierLIst, Convert.ToInt32(expensesUserDoRequest.BusinessVatId));
               
                if (ItemFound!=null)
                    {
                        List<ExpenseType> res = await CreateExpenseCategorylist(userId, ItemFound.supplier_id.ToString());
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
                            TaxId= expensesUserDoRequest.BusinessVatId,
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

                        List<ExpenseType> res=await CreateExpenseCategorylist(userId, supplierId.ToString());
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
        public async Task<DigitalDocumentToApproveObj> GetDigitalDocumentToApproveListByUser(int UserID, string Typelist)
        {
            try
            {
               // List<DigitalDocumentToApprove> List_DigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
                DigitalDocumentToApproveObj resObj = new DigitalDocumentToApproveObj();
                resObj.listDigitalDocumentToApprove = new List<DigitalDocumentToApprove>();
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
                                ResListOfClientCompaniesThatWasSentDigitalDocument = _repository
                                .GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == null)
                                .OrderByDescending(x => x.docDate)
                                .ToList();

                                // ResListOfClientCompaniesThatWasSentDigitalDocument = _repository.GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) );
                                break;
                            case "Rejected":
                                
                                ResListOfClientCompaniesThatWasSentDigitalDocument = _repository
                                .GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == false)
                                .OrderByDescending(x => x.docDate)
                                .ToList();

                                break;
                            case "Approved":
                                ResListOfClientCompaniesThatWasSentDigitalDocument = _repository
                               .GetListOfObjects<BusinessData>(x => x.ClientVat_id == Convert.ToUInt32(vatId) && x.DocumentApprovedtoUninet == true)
                               .OrderByDescending(x => x.docDate)
                               .ToList();
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
                            if (DigitalClientRow.DataSourceType == 2)
                            {
                                string JsonDocUrl = "";
                                var DocInfofilter = Builders<BsonDocument>.Filter.Eq("_id", new ObjectId(DigitalClientRow.JsonDocumentid));
                                var DocInfoprojection = Builders<BsonDocument>.Projection.Include("doc_info.doc_url_copy").Exclude("_id");
                                var DocInforesult = _IcountWebhookData.Find(DocInfofilter).Project(DocInfoprojection).FirstOrDefault();
                                if (DocInforesult != null)
                                {
                                    JsonDocUrl = DocInforesult["doc_info"]["doc_url_copy"].AsString;
                                }
                                resObj.listDigitalDocumentToApprove.Add(new DigitalDocumentToApprove() { JsonDocumentid = DigitalClientRow.JsonDocumentid, ClientVat_id = Convert.ToInt32(DigitalClientRow.ClientVat_id), SendingDigitalDocumentBusinessID = DigitalClientRow.BusinessId, BusinessVatId = DigitalClientRow.BusinessVatId, DocInfoUrl = JsonDocUrl, supplier_name_Sender = DigitalClientRow.supplier_name_Sender, docDate = DigitalClientRow.docDate, amountAV = DigitalClientRow.amountAV, currency_code = DigitalClientRow.currency_code });
                            }
                        }                    
                    }

                 }

                //here added logic eyal to add a  new collection to icountClientSuppliers mongodb collection
                foreach (var Company in ResListOfCompaniesRelatedToLogedinUser)
                {
                    var UserexternalSystemDynamicFieldslist = _repository.GetListOfObjects<UsersExternalSystemDynamicFields>(x => x.Companyid == Company.BusinessId && x.Userid == UserID);

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
                    var ClinetinfoEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 75);  ////api.icount.co.il/api/v3.php/supplier/get_list
                    var endpointClinetinfo = ClinetinfoEndpoint.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                    HttpMethod methodclientinfo = HttpMethod.Get;
                    var ReponsneClientInfo = await SendRequest(endpointClinetinfo, methodclientinfo);

                    // Parse the JSON response
                    var jsonDocument = JsonDocument.Parse(ReponsneClientInfo);
                    var jsonData = jsonDocument.RootElement;

                   

                    var bsonDocument = BsonDocument.Parse(jsonData.ToString());
                    // Add "internalcompanid" and "UserID" properties
                    bsonDocument.Add("internalcompanid", Company.BusinessId);
                    bsonDocument.Add("UserID", UserID);


                    // Define the query to find and delete the existing document
                    var filter = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalcompanid", Company.BusinessId),
                        Builders<BsonDocument>.Filter.Eq("UserID", UserID)
                    );

                    // Check if a document with the specified internalcompanid and UserID exists
                    var existingDocument = await _IcountClientSuppliers.Find(filter).FirstOrDefaultAsync();

                    if (existingDocument != null)
                    {
                        // If the document exists, delete it
                         _IcountClientSuppliers.DeleteOne(filter);
                        _IcountClientSuppliers.InsertOne(bsonDocument);
                    }
                    else
                    {
                        _IcountClientSuppliers.InsertOne(bsonDocument);
                    }
                    

                    

                    //added logic eyal to add json to expensesmongodb
                    var ExpenseSearchObj = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 77);//api.icount.co.il/api/v3.php/expense/search
                    var ExpenseSearchEndpoint = ExpenseSearchObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue; //+ "&supplier_id=" + supplierId.ToString();
                    HttpMethod methodexpenseserach = HttpMethod.Get;
                    var ReponsneExpenseSearch = await SendRequest(ExpenseSearchEndpoint, methodexpenseserach);
                    var jsonDocumentReponsneExpenseSearch = JsonDocument.Parse(ReponsneExpenseSearch);
                    var jsonDataExpenseSearch = jsonDocumentReponsneExpenseSearch.RootElement;
                    var bsonDocumentExpenseSearch = BsonDocument.Parse(jsonDataExpenseSearch.ToString());
                    // Add "internalcompanid" and "UserID" properties
                    bsonDocumentExpenseSearch.Add("internalcompanid", Company.BusinessId);
                    bsonDocumentExpenseSearch.Add("UserID", UserID);


                    // Define the query to find and delete the existing document
                    var filterExpenseSearch = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalcompanid", Company.BusinessId),
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
                    var AllExpensesTypeObj = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 78);//api.icount.co.il/api/v3.php/expense/types
                    var AllExpensesEndpoint = AllExpensesTypeObj.Endpoint + "?cid=" + cidvalue + "&user=" + uservalue + "&pass=" + passvalue;
                    HttpMethod AllExpensesMethod = HttpMethod.Get;
                    var ReponsneAllExpensesTypes = await SendRequest(AllExpensesEndpoint, AllExpensesMethod);
                    var ReponsneExpenseTypesjsonDocument = JsonDocument.Parse(ReponsneAllExpensesTypes);
                    var jsonDataExpensetypes = ReponsneExpenseTypesjsonDocument.RootElement;
                    var bsonDocumentExpensetypes = BsonDocument.Parse(jsonDataExpensetypes.ToString());
                    bsonDocumentExpensetypes.Add("internalcompanid", Company.BusinessId);
                    bsonDocumentExpensetypes.Add("UserID", UserID);

                    // Define the query to find and delete the existing document
                    var filterExpensetypes = Builders<BsonDocument>.Filter.And(
                        Builders<BsonDocument>.Filter.Eq("internalcompanid", Company.BusinessId),
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





                }


                var Objres = new DigitalDocumentToApproveObj
                {
                    listDigitalDocumentToApprove = resObj.listDigitalDocumentToApprove,
                    fullname = ResListOfCompaniesRelatedToLogedinUser[0].FirstName+" "+ ResListOfCompaniesRelatedToLogedinUser[0].LastName
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
                var businessDatarow = _repository.GetFirstObject<BusinessData>(x => x.BusinessVatId == requestRejectDocument.BusinessVatId && x.ClientVat_id == requestRejectDocument.ClientVat_id && x.JsonDocumentid== requestRejectDocument.Jsondocumentid);
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

        public async Task<AddGenericexpenseTypeResponse> AddexpenseType(AddexpenseTypeRequest addexpenseTypeRequest, int userId)
        {
            try
            {
                var UserexternalSystemDynamicFieldslist = _repository.GetListOfObjects<UsersExternalSystemDynamicFields>(x => x.Companyid == addexpenseTypeRequest.internalCompanyId && x.Userid == userId);

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
                var AddexpenseTypeEndpoint = _repository.GetFirstObject<SystemsEndpoints>(x => x.Id == 80);
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
                     res = await CreateExpenseCategorylist(userId, addexpenseTypeRequest.supplier_ID);

                    var res2= new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1 ? "expense type added succesfully" : "סוג הוצאה התווספה בהצלחה",
                        ExpenseTypeList = res
                    };
                    return res2;



                }
                else
                {
                    var res2 = new AddGenericexpenseTypeResponse
                    {
                        textResponse = addexpenseTypeRequest.Lang == 1 ? "failed to AddexpenseType from icount" : "נכשל ביצירת הוצאה באייקוינט",
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
