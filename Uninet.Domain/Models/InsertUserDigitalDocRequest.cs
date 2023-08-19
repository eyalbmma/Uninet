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

        [JsonPropertyName("supplier_id")]
        public int  supplier_id { get; set; }


        [JsonPropertyName("expense_type_id")]
        public int expense_type_id { get; set; }



        [JsonPropertyName("expense_doctype")]
        public string expense_doctype { get; set; }


        [JsonPropertyName("expense_docnum")]
        public string expense_docnum { get; set; }

        [JsonPropertyName("internalCompanyId")]
        public int internalCompanyId { get; set; }

        [JsonPropertyName("expense_sum")]
        public float expense_sum { get; set; }



        [Required]
        [JsonPropertyName("Lang")]
        public int Lang { get; set; }


    }
}
