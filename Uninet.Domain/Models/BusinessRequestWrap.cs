using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class BusinessRequestWrap
    {
        [Required]
        [JsonPropertyName("RegisterUserReq")]
        public List<BusinessRequest> RegisterUserReq { get; set; }

        [Required]
        [JsonPropertyName("Lang")]
        public int Lang { get; set; }
    }
}
