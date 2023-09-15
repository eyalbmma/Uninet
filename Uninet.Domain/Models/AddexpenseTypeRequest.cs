using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class AddexpenseTypeRequest
    {
        [JsonPropertyName("vat_to_expense")]
        public bool vat_to_expense { get; set; }

        [JsonPropertyName("expense_type_name")]
        public string expense_type_name { get; set; }

        [JsonPropertyName("deductable_vat")]
        public float deductable_vat { get; set; }

        [JsonPropertyName("deductable_expense")]
        public float deductable_expense { get; set; }

        [JsonPropertyName("internalCompanyId")]
        public int internalCompanyId { get; set; }


        [JsonPropertyName("supplier_ID")]
        public string supplier_ID { get; set; }




        [JsonPropertyName("Lang")]
        public int Lang { get; set; }


    }
}
