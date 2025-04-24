using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    [BsonIgnoreExtraElements]
    public class SendDoc
    {
        public string documentId { get; set; }
        public int type { get; set; }
        public int number { get; set; }
        public string documentDate { get; set; }
        public long creationDate { get; set; }
        public int? status { get; set; }
        public string lang { get; set; }
        public float? amountDueVat { get; set; }
        public float? amountExemptVat { get; set; }
        public float? amountExcludedVat { get; set; }
        public float? amountLocal { get; set; }
        public float? amountOpened { get; set; }
        public float? vat { get; set; }
        public float amount { get; set; }
        public string currency_code { get; set; }
        public string currency_name { get; set; }
        public float? currencyRate { get; set; }
        public int? vatType { get; set; }
        public IncomeItem[] income { get; set; }

        public float? totalwithvat { get; set; }
        public float? totalwithvat_currency { get; set; }
        public float? paid { get; set; }
        public float? paid_currency { get; set; }
        public float? totalwht { get; set; }
        public float? totalwht_currency { get; set; }
        public float? totalpaid { get; set; }
        public float? totalpaid_currency { get; set; }
        public float? remainingsum { get; set; }
        public float? remainingsum_currency { get; set; }
        public float? remainingsum_before_vat { get; set; }
        public float? remainingsum_before_vat_currency { get; set; }
        public int client_id { get; set; }
        public int custom_client_id { get; set; }
        public int client_idno { get; set; }
        public string client_name { get; set; }
        public string client_address { get; set; }
        public int? user_id { get; set; }
        public int? salesman_id { get; set; }
        public string paydate { get; set; }
        public string duedate { get; set; }
        public int? income_type_id { get; set; }
        public int? expense_type_id { get; set; }
        public string invoice_reference_number { get; set; }
        public bool? tax_exempt { get; set; }
        public float? discount { get; set; }
        public float? discount_currency { get; set; }
        public float? roundup { get; set; }
        public float? roundup_currency { get; set; }
        public float? afterdiscount { get; set; }
        public float? afterdiscount_currency { get; set; }
        public bool? is_cancelled { get; set; }
        public bool? is_cancellation { get; set; }
        public string cancellation_reason { get; set; }
        public string cancellation_doctype { get; set; }
        public int? cancellation_docnum { get; set; }
        public string cancelled_doctype { get; set; }
        public int? cancelled_docnum { get; set; }

        public string fis_id { get; set; }
        public string entity_id_internal { get; set; }
        public string entity_vat_number { get; set; }
    }
}
