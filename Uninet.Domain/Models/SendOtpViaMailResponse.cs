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

    public class SendEmailResult
    {
        public List<SendEmailResponse> EmailResponses { get; set; } = new List<SendEmailResponse>();
        public string TextResponse { get; set; } // Summary of the entire result
    }


    public class SendEmailResponse
    {
        public string Email { get; set; }
        public string Vatid { get; set; }
        public int Lang { get; set; }
        public int SubCompanyId { get; set; }
        public bool result { get; set; }
     
    }

}
