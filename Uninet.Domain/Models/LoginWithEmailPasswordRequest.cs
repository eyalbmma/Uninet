using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
   

    public class LoginWithEmailPasswordRequest
    {
        [Required]
        [JsonPropertyName("Email")]
        public string Email { get; set; }

        [Required]
        [JsonPropertyName("Password")]
        public string Password { get; set; }

        [Required]
        [JsonPropertyName("Lang")]
        public int Lang { get; set; }


    }
}
