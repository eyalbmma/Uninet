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
        [JsonPropertyName("authenticationToken")]
        public string authenticationToken { get; set; }
    }
}
