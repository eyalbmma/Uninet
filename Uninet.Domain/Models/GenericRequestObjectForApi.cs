using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class GenericRequestObjectForApi
    {
        [Required]
        [JsonPropertyName("userid")]
        public int userid { get; set; }

        [Required]
        [JsonPropertyName("BussinesId")]
        public int BussinesId { get; set; }


        [Required]
        [JsonPropertyName("SystemId")]
        public int SystemId { get; set; }


        [Required]
        [JsonPropertyName("Apiid")]
        public int ApiId { get; set; }
    }
}
