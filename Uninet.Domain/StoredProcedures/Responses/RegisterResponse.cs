using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.StoredProcedures.Responses
{
    public class RegisterResponse
    {
       
        public bool? sucess { get; set; }
        public string? textResponse { get; set; }
        public string? encryptedUser { get; set; }
    }
}
