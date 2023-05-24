using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class ExternalSystemCustomizeFieldResult
    {
        [JsonPropertyName("BusinessLogUrl")]
        public string BusinessLogUrl { get; set; }

        [JsonPropertyName("listdata")]
        public  List<CustomizedDataLIst> listdata { get; set; }  
        

        public string VideoLink { get; set; }

    }
}
