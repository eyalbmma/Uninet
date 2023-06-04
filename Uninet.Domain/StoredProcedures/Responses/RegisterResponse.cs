using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.StoredProcedures.Responses
{
    public class RegisterResponse
    {
       
        public bool? Success { get; set; }
        public string? TextResponse { get; set; }
        public string? EncryptedUserid { get; set; }
    }
}
