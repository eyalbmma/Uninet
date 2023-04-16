using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class RegisterUserRequest
    {
        // [Required]
        //[JsonPropertyName("FirstName")]
        //public string FirstName { get; set; }

        //[Required]
        //[JsonPropertyName("LastName")]
        //public string LastName { get; set; }


        //[Required]
        //[JsonPropertyName("PhoneNumber")]
        //public int PhoneNumber { get; set; }
        

        [Required]
        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("Password")]
        public string Password { get; set; }

        [Required]
        [JsonPropertyName("TemplateId")]
        public int TemplateId { get; set; }

        [Required]
        [JsonPropertyName("Lang")]
        public int Lang { get; set; }



    }
}
