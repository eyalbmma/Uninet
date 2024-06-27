using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class DigitalDocumentDInputRequest
    {
        [JsonPropertyName("JsonDocumentid")]
        public string? JsonDocumentid { get; set; } // Nullable string

        [JsonPropertyName("sendingDigitalDocumentBusinessID")]
        public int? sendingDigitalDocumentBusinessID { get; set; } // Nullable int

        [JsonPropertyName("ClientVat_id")]
        public string? ClientVat_id { get; set; } // Nullable string

        [JsonPropertyName("BusinessVatId")]
        public string? BusinessVatId { get; set; } // Nullable string

        [JsonPropertyName("Lang")]
        public int Lang { get; set; } // Lang is required
    }

}
