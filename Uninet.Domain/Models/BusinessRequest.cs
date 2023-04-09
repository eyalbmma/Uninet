using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class BusinessRequest
    {
       
        
        [Required]
        [JsonPropertyName("BusinessId")]
        public int BusinessId { get; set; }

        [Required]
        [JsonPropertyName("BusinessName")]
        public string BusinessName { get; set; }


        [Required]
        [JsonPropertyName("BusinessEmail")]
        public int BusinessEmail { get; set; }


        [Required]
        [JsonPropertyName("DelearType")]
        public int DelearType { get; set; }

        [Required]
        [JsonPropertyName("BusinessType")]
        public int BusinessType { get; set; }
        
    }
}
