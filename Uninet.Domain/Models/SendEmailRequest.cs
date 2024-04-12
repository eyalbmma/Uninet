using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public  class SendEmailRequest
    {
        public string VatId { get; set; }
        public int Lang { get; set; }
    }
}
