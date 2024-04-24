using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class SendEmailRequest
    {
        public string Email { get; set; }
        public string Vatid { get; set; }   
        public int Lang { get; set; }
        public int SubCompanyId { get; set; }
    }
}
