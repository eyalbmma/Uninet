using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class VerifyEmailLinkRequest
    {
        public string EmailGuidVerification { get; set; }
        public int Lang { get; set; }
    }
}
