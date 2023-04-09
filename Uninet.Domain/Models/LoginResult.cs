using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class LoginResult
    {


        //[JsonPropertyName("role")]
        //public string Role { get; set; }



        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("refreshToken")]
        public string RefreshToken { get; set; }

        [JsonPropertyName("Success")]
        public bool Success { get; set; }


        [JsonPropertyName("Userid")]
        public int Userid { get; set; }

    }
}
