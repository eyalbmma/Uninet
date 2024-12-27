using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.DATA.Services
{
    public class ExternalSystemConfig
    {
        public IMongoCollection<BsonDocument> CompaniesInfoCollection { get; set; }
        public IMongoCollection<BsonDocument> WebhookCollection { get; set; }

        public IMongoCollection<BsonDocument> ClientSupplierCollection { get; set; }
        
        public string CompanyFieldPath { get; set; }
        public string SubCompanyFieldPath { get; set; }
        public string VatFieldPath { get; set; }
        public string SupplierFieldPath { get; set; }
        public string UrlFieldPath { get; set; } // New property for document URL paths
    }


}
