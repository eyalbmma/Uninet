using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class CustomizedDataLIst
    {
        [JsonPropertyName("FieldLabelName")]
        public string FieldLabelName { get;set; }


        [JsonPropertyName("FieldLabelValue")]
        public string FieldLabelValue { get; set; }

        [JsonPropertyName("FiledType")]
        public int? FiledType { get; set; }

        [JsonPropertyName("FieldTypeDesc")]
        public string? FieldTypeDesc { get; set; }
        



    }
}
