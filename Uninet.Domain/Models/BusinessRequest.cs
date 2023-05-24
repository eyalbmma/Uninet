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
       
        
     
        [JsonPropertyName("BusinessId")]
        public int BusinessId { get; set; }

        
        [JsonPropertyName("BusinessName")]
        public string? BusinessName { get; set; }


        
        [JsonPropertyName("BusinessEmail")]
        public string? BusinessEmail { get; set; }
        
        [Required]
        [JsonPropertyName("BusinessType")]
        public int? BusinessType { get; set; }


        
        [Required]
        [JsonPropertyName("OrganizationRole")]
        public string? OrganizationRole { get; set; }



        [Required]
        [JsonPropertyName("OrganizationName")]
        public string? OrganizationName { get; set; }
        
        [Required]
        [JsonPropertyName("OrganizationType")]
        public int? OrganizationType { get; set; }


        [Required]
        [JsonPropertyName("ExternalSystemId")]
        public int? ExternalSystemId { get; set; }

       


       
        




    }
}
