using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class IcountDocument
    {
        [BsonId]
        public ObjectId Id { get; set; }

        public string Doctype { get; set; }
        public string DocNum { get; set; }
        public DateTime DateIssued { get; set; }
        public DateTime TimeIssued { get; set; }
        public string ClientId { get; set; }
        public string CustomClientId { get; set; }
        public string CurrencyId { get; set; }
        public string CurrencyCode { get; set; }
        public string Currency { get; set; }
        public decimal Rate { get; set; }
        public decimal Total { get; set; }
        public int IsCancellation { get; set; }
        public int IsCancelled { get; set; }
        public int Status { get; set; }
        public string VatId { get; set; } // Added VatId property
    }
}
