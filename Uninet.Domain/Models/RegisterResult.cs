using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class RegisterResult
    {


        //[JsonPropertyName("role")]
        //public string Role { get; set; }



        [JsonPropertyName("accessToken")]
        public string accessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string refreshToken { get; set; }

        [JsonPropertyName("Success")]
        public bool success { get; set; }


        [JsonPropertyName("Userid")]
        public int Userid { get; set; }

        [JsonPropertyName("Q1_Q2_InidicationRes")]
        public bool Q1_Q2_InidicationRes { get; set; }



        [JsonPropertyName("Q3_InidicationRes")]
        public bool Q3_InidicationRes { get; set; }


        [JsonPropertyName("verified")]
        public bool? verified { get; set; }


        [JsonPropertyName("EncryptedUserId")]
        public string? EncryptedUserId { get; set; }
        

    }
}
