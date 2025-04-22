using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class RefreshTokenRequest
    {
        public string RefreshToken { get; set; }

        [JsonPropertyName("authenticationToken")]
        public string authenticationToken { get; set; }
    }
}
