using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class RequestRejectDocument
    {
        [JsonPropertyName("BusinessVatId")]
        public string BusinessVatId { get; set; }


        [JsonPropertyName("ClientVat_id")]
        public int ClientVat_id { get; set; }


        [JsonPropertyName("Lang")]
        public int Lang { get; set; }

    }
}
