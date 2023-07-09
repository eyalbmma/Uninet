using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.StoredProcedures.Responses
{
    public  class VerifyUserByOtpUserIdAndTimeStampResponse
    {
        public bool Verified { get; set;}
        public string? Email { get; set; }
    }
}
