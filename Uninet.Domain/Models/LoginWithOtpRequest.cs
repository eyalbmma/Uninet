using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class LoginWithOtpRequest
    {
        [Required]
        [JsonPropertyName("Otp")]
        public string Otp { get; set; }


        [Required]
        [JsonPropertyName("EncryptedUser")]
        public string EncryptedUser { get; set; }

    }
}
