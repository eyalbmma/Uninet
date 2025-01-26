using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class InsertUserDigitalDocRequest
    {
        [JsonPropertyName("supplier_ID")]
        public string supplier_id { get; set; }

        [JsonPropertyName("expense_type_id")]
        public string expense_type_id { get; set; } // Change to `int` to match the payload

        [JsonPropertyName("expense_doctype")]
        public string expense_doctype { get; set; }

        [JsonPropertyName("expense_docnum")]
        public string expense_docnum { get; set; }

        [JsonPropertyName("internalCompanyId")]
        public int internalCompanyId { get; set; }

        [JsonPropertyName("expense_sum")]
        public float expense_sum { get; set; }

        [JsonPropertyName("Jsondocumentid")]
        public string Jsondocumentid { get; set; }

        [Required]
        [JsonPropertyName("Lang")]
        public int Lang { get; set; }

        [JsonPropertyName("expense_vat_sum")]
        public float expense_vat_sum { get; set; } // Change to `float` to match potential decimals

        [JsonPropertyName("expense_net_sum")]
        public float expense_net_sum { get; set; } // Change to `float` to match potential decimals

        [JsonPropertyName("tax_id")]
        public string tax_id { get; set; }
    }

}
