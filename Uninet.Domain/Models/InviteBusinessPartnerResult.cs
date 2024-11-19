using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uninet.Domain.Models
{
    public class InviteBusinessPartnerResult
    {
        public bool Success { get; set; }
        public string textResponse { get; set; }

        public List<EmailSentResult> EmailsSent { get; set; }

    }

    public class EmailSentResult
    {
        public string Email { get; set; }
        public bool EmailSent { get; set; }
        public string EntityType { get; set; }
        public int SubCompanyId { get; set; }
    }
}
