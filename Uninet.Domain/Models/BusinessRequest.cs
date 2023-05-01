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
        public string BusinessEmail { get; set; }
        
        [Required]
        [JsonPropertyName("BusinessType")]
        public int BusinessType { get; set; }

        [Required]
        [JsonPropertyName("OrganizationName")]
        public string OrganizationName { get; set; }
        
        [Required]
        [JsonPropertyName("OrganizationType")]
        public int OrganizationType { get; set; }


        [Required]
        [JsonPropertyName("ExternalSystemId")]
        public int ExternalSystemId { get; set; }

        [Required]
        [JsonPropertyName("Apikey")]
        public string Apikey { get; set; }


        [Required]
        [JsonPropertyName("Username")]
        public string Username { get; set; }

        [Required]
        [JsonPropertyName("Password")]
        public string Password { get; set; }


        [Required]
        [JsonPropertyName("Endpoint")]
        public string Endpoint { get; set; }
        




    }
}
