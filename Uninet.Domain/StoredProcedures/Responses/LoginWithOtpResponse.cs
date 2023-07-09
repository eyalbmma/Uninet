using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.StoredProcedures.Responses
{
    public class LoginWithOtpResponse
    {
        public bool verified { get; set; }
        
        public string userId { get; set; }

        public string description { get; set; }
        // public int Role { get; set; }


    }
}
