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

    }
}
