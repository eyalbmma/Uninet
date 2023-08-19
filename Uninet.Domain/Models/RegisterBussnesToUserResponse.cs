using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Uninet.Domain.StoredProcedures.Responses;

namespace Uninet.Domain.Models
{
    public  class RegisterBussnesToUserResponse
    {
        
        public AddBusinessToUserResult addBusinessToUserResult { get; set; }
        public string textResponse { get; set; } // Add this property
    }
}
