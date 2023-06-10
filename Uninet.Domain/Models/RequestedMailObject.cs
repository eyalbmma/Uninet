using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class RequestedMailObject
    {
        public string? Sendername { get; set; }
        public string? RecipientName { get; set; }

        public string? DocType { get; set; }

        public string? DocID { get; set; }
        public string? UserName { get; set; }
        public DateTime? CreatedDate { get; set; }

        public string? DocStatus { get; set;}

        public string? DocLink { get; set; }

    }
}
