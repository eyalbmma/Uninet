using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Uninet.DATA.Interfaces;
using Uninet.DATA.Services.MultipleContext;
using Uninet.Domain.Interfaces;
using Uninet.Domain.Models;
using Uninet.Domain.StoredProcedures.Constants;
using Uninet.Domain.StoredProcedures.Responses;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Uninet.DATA.Services
{
    public class UninetInputDataAccess : IUninetInputDataAccess
    {
        private readonly IRepository<UninetContext> _repository;
        private readonly IMongoCollection<BsonDocument> _Uninetgreenvoicedocument;
        public UninetInputDataAccess(IRepository<UninetContext> repository, IMongoClient client)//, IloginRepository loginRepository
        {
            var database = client.GetDatabase("Uninet");
            var Uninetgreenvoicedocument = database.GetCollection<BsonDocument>("UninetGreenVoiceCollection");
            _Uninetgreenvoicedocument = Uninetgreenvoicedocument;
            _repository = repository;
           
        }


        public async Task<bool> SavegreenvoicedocumentIntoUninet(List<BsonDocument> InputData)
        {
            try
            {






                //insert Data into UninetGreenVoiceCollection MongoDB
                _Uninetgreenvoicedocument.InsertMany(InputData);





                //insert data into sql DB table BusinessData


                //extract documnet from uninet mongo
                List<string> EmailLIst = new List<string>();
                BsonDocument client = InputData[0]["client"].AsBsonDocument;
                BsonArray emails = client["emails"].AsBsonArray;
                foreach (BsonValue email in emails)
                {
                    EmailLIst.Add(email.AsString);
                }
                var EmailListstr = String.Join(",", EmailLIst);

                BsonDocument business = InputData[0]["business"].AsBsonDocument;
                string taxId = business["taxId"].AsString;



                string JsonDocumentid = InputData[0]["id"].AsString;

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
                var res = result.ToList();
                return res[0].Result;
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
