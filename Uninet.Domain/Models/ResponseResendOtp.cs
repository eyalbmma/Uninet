using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class ResponseResendOtp
    {
        public bool Success { get; set; }

        public string  Desc { get; set; }
        public int userid { get; set; }
        public string otp { get; set; }
    }
}
