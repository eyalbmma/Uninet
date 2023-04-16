using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class SendOtpViaMailResponse
    {
        public bool result { get; set; }
        public string OTP { get; set; }
    }
}
